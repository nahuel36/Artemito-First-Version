using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class Zone
{
    public List<string> zoneScenes;
}

[CreateAssetMenu(fileName = "Scenes", menuName = "Pnc/ScenesConfiguration", order = 1)]
public class ScenesConfiguration : ScriptableObject
{
    public string canvas;
    public string mainMenu;
    public string options;
    public string saveAndLoad;
    public List<Zone> zones;
}
