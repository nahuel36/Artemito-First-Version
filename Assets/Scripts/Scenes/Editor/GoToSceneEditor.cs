using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;

[CustomEditor(typeof(GoToScene))]
public class GoToSceneEditor : Editor
{
    ScenesConfiguration scenesConfiguration;
    private void ShowPopupForScenePath(Rect rect, SerializedProperty property, string description, bool useRect, ScenesConfiguration scenesConfig, int zoneIndex)
    {
        int selected = 0;

        string activeScenePath = SceneManager.GetActiveScene().path;

        string[] paths = new string[scenesConfig.zones[zoneIndex].zoneScenes.Count];
        for (int i = 0; i < scenesConfig.zones[zoneIndex].zoneScenes.Count; i++)
        {
            if (activeScenePath != scenesConfig.zones[zoneIndex].zoneScenes[i])
            {  
                paths[i] = scenesConfig.zones[zoneIndex].zoneScenes[i];
                if (paths[i] == property.stringValue)
                {
                    selected = i;
                }
            }
        }
        if (useRect)
            selected = EditorGUI.Popup(rect, description, selected, paths);
        else
            selected = EditorGUILayout.Popup(description, selected, paths);

        property.stringValue = paths[selected];
    }

    private int CheckZone(ScenesConfiguration scenesConfig) 
    { 
        for (int i = 0; i < scenesConfiguration.zones.Count; i++)
        {
            if (scenesConfiguration.zones[i].zoneScenes.Contains(SceneManager.GetActiveScene().path))
                return i;
        }
        return -1;
    }
    public override void OnInspectorGUI()
    {
        scenesConfiguration = Resources.Load<ScenesConfiguration>("Scenes");

        int zone = CheckZone(scenesConfiguration);

        if (zone > -1)
            ShowPopupForScenePath(new Rect(), serializedObject.FindProperty("scenePath"), "Scene to go", false, scenesConfiguration, zone);
        else
            EditorGUILayout.LabelField("You must add this scene to Build Settings and in a Zone in Scenes Configuration");

        EditorGUILayout.PropertyField(serializedObject.FindProperty("entryPointName"));

        serializedObject.ApplyModifiedProperties();

        if (GUI.changed)
        {
            EditorUtility.SetDirty(target);
        }
    }
}
