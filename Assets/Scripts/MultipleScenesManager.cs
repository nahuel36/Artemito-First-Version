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

    ScenesConfiguration scenes;
    private int actualZone = 0;
    static MultipleScenesManager instance;
    public static MultipleScenesManager Instance {
        get { return instance; }
    }

    public bool allZoneScenesInitialized = false;
    private List<ZoneScene> zone_scenes;
    // Start is called before the first frame update
    void Start()
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

        scenes = Resources.Load<ScenesConfiguration>("Scenes");

        allZoneScenesInitialized = false;

        string actualScenePath = SceneManager.GetActiveScene().path;

        if (!scenes.zones[actualZone].zoneScenes.Contains(actualScenePath))
        {
            allZoneScenesInitialized = true;
            Debug.Log("Add this scene to build settings and pnc scene settings");
            return;
        }
        else
        {
            allZoneScenesInitialized = false;
            zone_scenes = new List<ZoneScene>();
            for (int i = 0; i < scenes.zones[actualZone].zoneScenes.Count; i++)
            {
                if (actualScenePath != scenes.zones[actualZone].zoneScenes[i])
                {
                    LoadSceneParameters parameters = new LoadSceneParameters();
                    parameters.loadSceneMode = LoadSceneMode.Additive;
                    Scene scene = SceneManager.LoadScene(Path.GetFileName(scenes.zones[actualZone].zoneScenes[i]).Replace(".unity", ""), parameters);
                    ZoneScene zoneScene = new ZoneScene();
                    zoneScene.scene = scene;
                    zoneScene.isInitialized = false;
                    zone_scenes.Add(zoneScene);
                }
            }
        }
        SceneManager.LoadScene(Path.GetFileName(scenes.canvas).Replace(".unity", ""), LoadSceneMode.Additive);
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


}
