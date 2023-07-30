using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
public class ScenesManager : MonoBehaviour
{
    ScenesConfiguration scenes;

    static ScenesManager instance;
    
    // Start is called before the first frame update
    void Start()
    {
        if (instance != null)
        {
            if (this != instance)
                Destroy(this.gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(instance.gameObject);
        }

        scenes = Resources.Load<ScenesConfiguration>("Scenes");

        string actualScenePath = SceneManager.GetActiveScene().path;

        for (int i = 0; i < scenes.zones[0].zoneScenes.Count; i++)
        {
            if(actualScenePath != scenes.zones[0].zoneScenes[i])
                SceneManager.LoadScene(Path.GetFileName(scenes.zones[0].zoneScenes[i]).Replace(".unity", ""), LoadSceneMode.Additive);
        }

        SceneManager.LoadScene(Path.GetFileName(scenes.canvas).Replace(".unity", ""), LoadSceneMode.Additive);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
