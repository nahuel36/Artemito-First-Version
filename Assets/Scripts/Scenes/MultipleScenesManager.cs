using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using System.Threading.Tasks;
public class MultipleScenesManager : MonoBehaviour
{
    public class ZoneScene
    {
        public Scene scene;
        public bool isInitialized = false;
    }
    ScenesConfiguration scenesConfiguration;
    static MultipleScenesManager instance;
    public static MultipleScenesManager Instance {
        get { return instance; }
    }

    [HideInInspector]public bool allZoneScenesInitialized = false;
    private List<ZoneScene> zone_scenes;
    bool charactersInitialized = false;
    Scene transitionScene;
    bool transitionLoaded;
    // Start is called before the first frame update
    async void Start()
    {
        if (instance != null)
        {
            if (this != instance)
            { 
                Destroy(this.gameObject);
                return;
            }
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(instance.gameObject);
        }


        transitionLoaded = false;
        charactersInitialized = false;


        scenesConfiguration = Resources.Load<ScenesConfiguration>("Scenes");

        int actualZone = CheckZone(scenesConfiguration);
        if (actualZone < 0)
        {
            actualZone = 0;
            Debug.Log("Add this scene to Build Settings and Scenes Configuration");
        }


        allZoneScenesInitialized = false;

        Scene actualScene = SceneManager.GetActiveScene();

        if (!scenesConfiguration.zones[actualZone].zoneScenes.Contains(actualScene.path))
        {
            allZoneScenesInitialized = true;
            Debug.Log("Add this scene to Build Settings and Scenes Configuration");
        }
        else
        {
            allZoneScenesInitialized = false;
            zone_scenes = new List<ZoneScene>();
            for (int i = 0; i < scenesConfiguration.zones[actualZone].zoneScenes.Count; i++)
            {
                if (actualScene.path != scenesConfiguration.zones[actualZone].zoneScenes[i])
                {
                    LoadSceneParameters zoneParameters = new LoadSceneParameters();
                    zoneParameters.loadSceneMode = LoadSceneMode.Additive;
                    Scene scene = SceneManager.LoadScene(Path.GetFileName(scenesConfiguration.zones[actualZone].zoneScenes[i]).Replace(".unity", ""), zoneParameters);
                    ZoneScene zoneScene = new ZoneScene();
                    zoneScene.scene = scene;
                    zoneScene.isInitialized = false;
                    zone_scenes.Add(zoneScene);
                }
                else
                {
                    ZoneScene zoneScene = new ZoneScene();
                    zoneScene.scene = actualScene;
                    zoneScene.isInitialized = true;
                    zone_scenes.Add(zoneScene);
                }
            }
        }

        LoadSceneParameters parameters = new LoadSceneParameters();
        parameters.loadSceneMode = LoadSceneMode.Additive;
        SceneManager.LoadScene(Path.GetFileName(scenesConfiguration.canvas).Replace(".unity", ""), parameters);

        transitionScene = SceneManager.LoadScene(Path.GetFileName(scenesConfiguration.transition).Replace(".unity", ""), parameters);


        PNCCharacter[] characters = FindObjectsOfType<PNCCharacter>();
        foreach (PNCCharacter character in characters)
        {
            await character.Initialize();
        }
        charactersInitialized = true;
    }

    private void Update()
    {
        bool AllInitialized = true;
        foreach(ZoneScene zoneScene in zone_scenes)
        {
            if (zoneScene.scene.isLoaded && !zoneScene.isInitialized)
            {
                AllInitialized = false;
                foreach (GameObject actualGameObject in zoneScene.scene.GetRootGameObjects())
                {
                    actualGameObject.SetActive(false);
                    //guardar si inicialmente esta activo o no
                }
                zoneScene.isInitialized = true;
            }
        }

        if (AllInitialized == true)
        {
            allZoneScenesInitialized = true;
        }

        SceneTransitionAnimation sceneTransitionAnimation = null;
        SceneEvents sceneEvents = null;

        if (transitionScene != null && transitionScene.isLoaded && !transitionLoaded && allZoneScenesInitialized)
        {
            foreach (GameObject actualGameObject in transitionScene.GetRootGameObjects())
            {
                SceneTransitionAnimation actualSceneTransitionAnimation = actualGameObject.GetComponent<SceneTransitionAnimation>();
                if (actualSceneTransitionAnimation != null)
                {
                    sceneTransitionAnimation = actualSceneTransitionAnimation;
                }
            }
            foreach (GameObject actualGameObject in SceneManager.GetActiveScene().GetRootGameObjects())
            {
                SceneEvents actualSceneEvents = actualGameObject.GetComponent<SceneEvents>();
                if (actualSceneEvents != null)
                {
                    sceneEvents = actualSceneEvents;
                }
            }
            RunTransitionEvents(sceneTransitionAnimation, sceneEvents);
            transitionLoaded = true;
        }
    }

    public async Task RunTransitionEvents(SceneTransitionAnimation sceneTransitionAnimation, SceneEvents sceneEvents)
    {
        if(sceneEvents)
            for (int j = 0; j < sceneEvents.events.Count; j++)
            {
                if (sceneEvents.events[j].sceneEvent == SceneEventInteraction.SceneEvent.beforeFadeIn)
                {
                    await InteractionUtils.RunAttempsInteraction(sceneEvents.events[j].attempsContainer, InteractionObjectsType.sceneEvent, "", -1, -1);
                }
            }

        await sceneTransitionAnimation.Out();

        if(sceneEvents)
            for (int j = 0; j < sceneEvents.events.Count; j++)
            {
                if (sceneEvents.events[j].sceneEvent == SceneEventInteraction.SceneEvent.afterFadeIn)
                {
                    await InteractionUtils.RunAttempsInteraction(sceneEvents.events[j].attempsContainer, InteractionObjectsType.sceneEvent, "", -1, -1);
                }
            }
    }


    private int CheckZone(ScenesConfiguration scenesConfig)
    {
        for (int i = 0; i < scenesConfig.zones.Count; i++)
        {
            if (scenesConfig.zones[i].zoneScenes.Contains(SceneManager.GetActiveScene().path))
                return i;
        }
        return -1;
    }

    public void LoadZoneScene(string scenePath, string playerSpawnPoint)
    {
        CommandLoadZoneScene command = new CommandLoadZoneScene();
        command.Queue(scenePath, playerSpawnPoint);
    }
    public async Task LoadZoneSceneInmediate(string scenePath, string playerSpawnPoint)
    {
        await FindObjectOfType<SceneTransitionAnimation>().In();

        PNCCharacter player = null;
        foreach (GameObject actualGameObject in SceneManager.GetActiveScene().GetRootGameObjects())
        {
            PNCCharacter actualCharacter = actualGameObject.GetComponent<PNCCharacter>();
            if (actualCharacter != null && actualCharacter.isPlayerCharacter)
            {
                actualCharacter.ErasePathFinder();
                player = actualCharacter;
            }

            WalkableArea2D actualWalkable = actualGameObject.GetComponent<WalkableArea2D>();
            if (actualWalkable != null)
            {
                actualWalkable.Disable();
            }

            WalkObstacle actualObstacle = actualGameObject.GetComponent<WalkObstacle>();
            if(actualObstacle != null){
                actualObstacle.ErasePathFinder();
            }

            actualGameObject.SetActive(false);
        }
        int sceneZone = CheckZone(scenesConfiguration);

        if (sceneZone < 0 || !scenesConfiguration.zones[sceneZone].zoneScenes.Contains(scenePath))
        {
            Debug.Log("Add this scene to Build Settings and Scenes Configuration");
            return;
        }
        ScenePoint point = null;
        WalkableArea2D walkableArea = null;
        SceneEvents sceneEvents = null;
        for (int i = 0; i < zone_scenes.Count; i++)
        {
            if (zone_scenes[i].scene.path == scenePath)
            {
                foreach (GameObject actualGameObject in zone_scenes[i].scene.GetRootGameObjects())
                {
                    actualGameObject.SetActive(true);

                    ScenePoint actualPoint = actualGameObject.GetComponent<ScenePoint>();
                    if (actualPoint != null && actualPoint.PointName == playerSpawnPoint)
                    {
                        point = actualPoint;
                    }

                    PNCCharacter actualCharacter = actualGameObject.GetComponent<PNCCharacter>();
                    if (actualCharacter != null && actualCharacter.isPlayerCharacter)
                    {
                        player = actualCharacter;
                    }

                    WalkableArea2D actualWalkable = actualGameObject.GetComponent<WalkableArea2D>();
                    if (actualWalkable != null)
                    {
                        walkableArea = actualWalkable;
                    }

                    SceneEvents actualSceneEvents = actualGameObject.GetComponent<SceneEvents>();
                    if (actualSceneEvents != null && actualSceneEvents.events != null)
                    {
                        sceneEvents = actualSceneEvents;
                        for (int j = 0; j < sceneEvents.events.Count; j++)
                        {
                            if (sceneEvents.events[j].sceneEvent == SceneEventInteraction.SceneEvent.beforeFadeIn)
                            {
                                InteractionUtils.RunAttempsInteraction(sceneEvents.events[j].attempsContainer, InteractionObjectsType.sceneEvent, "", -1, -1,null, true);
                            }
                        }

                        
                    }

                }
                SceneManager.SetActiveScene(zone_scenes[i].scene);
                break;
            }
        }
        
            

        await Task.Yield();
        walkableArea.Start();
        await player.Initialize();
        FindObjectOfType<UI_PNC_Manager>().ReInitialize();

        if (player != null && point != null)
        {
            player.DisablePathFinder();
            player.transform.position = point.transform.position;
        }

        await FindObjectOfType<SceneTransitionAnimation>().Out();

        if (sceneEvents != null && sceneEvents.events != null)
        {
            for (int j = 0; j < sceneEvents.events.Count; j++)
            {
                if (sceneEvents.events[j].sceneEvent == SceneEventInteraction.SceneEvent.afterFadeIn)
                {
                    //nunca poner un await, sino espera a que termine para ejecutar la proxima y no termina nunca
                    InteractionUtils.RunAttempsInteraction(sceneEvents.events[j].attempsContainer, InteractionObjectsType.sceneEvent, "", -1, -1);
                }
            }
        }
    }

}
