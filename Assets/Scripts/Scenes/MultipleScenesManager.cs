using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
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

    // Start is called before the first frame update
    void Start()
    {
        PNCCharacter[] characters = FindObjectsOfType<PNCCharacter>();
        foreach (PNCCharacter character in characters)
        {
            StartCoroutine(character.Initialize());
        }

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
                    LoadSceneParameters parameters = new LoadSceneParameters();
                    parameters.loadSceneMode = LoadSceneMode.Additive;
                    Scene scene = SceneManager.LoadScene(Path.GetFileName(scenesConfiguration.zones[actualZone].zoneScenes[i]).Replace(".unity", ""), parameters);
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
        SceneManager.LoadScene(Path.GetFileName(scenesConfiguration.canvas).Replace(".unity", ""), LoadSceneMode.Additive);
    }

    private void Update()
    {
        bool AllInitialized = true;
        foreach(ZoneScene zoneScene in zone_scenes)
        {
            if (zoneScene.scene.isLoaded && !zoneScene.isInitialized)
            {
                AllInitialized = false;
                foreach (GameObject gameObject in zoneScene.scene.GetRootGameObjects())
                {
                    gameObject.SetActive(false);
                    //guardar si inicialmente esta activo o no
                }
                zoneScene.isInitialized = true;
            }
        }
        if (AllInitialized == true)
        {
            allZoneScenesInitialized = true;
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
    public void LoadZoneSceneInmediate(string scenePath, string playerSpawnPoint)
    {
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

                }
                SceneManager.SetActiveScene(zone_scenes[i].scene);
                break;
            }
        }
        if(player != null && point != null)
            player.transform.position = point.transform.position;

        StartCoroutine(InitializeScene(player, walkableArea));
    }

    private IEnumerator InitializeScene(PNCCharacter player, WalkableArea2D walkableArea)
    {
        yield return new WaitForEndOfFrame();
        yield return StartCoroutine(walkableArea.Start());
        yield return StartCoroutine(player.Initialize());
        FindObjectOfType<UI_PNC_Manager>().ReInitialize();
    }

}
