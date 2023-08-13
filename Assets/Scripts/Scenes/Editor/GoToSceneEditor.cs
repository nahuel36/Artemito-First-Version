using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using System.Linq;

[CustomEditor(typeof(GoToScene))]
public class GoToSceneEditor : Editor
{
    ScenesConfiguration scenesConfiguration;


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

        EditorGUILayout.PropertyField(serializedObject.FindProperty("cursorName"));

        int zone = CheckZone(scenesConfiguration);

        if (zone > -1)
            SceneEditorUtils.ShowPopupForScenePath(new Rect(), serializedObject.FindProperty("scenePath"), "Scene to go", false, scenesConfiguration.zones[zone].zoneScenes);
        else
            EditorGUILayout.LabelField("You must add this scene to Build Settings and in a Zone in Scenes Configuration");
                
        EditorGUILayout.PropertyField(serializedObject.FindProperty("entryPointName"));

        EditorGUILayout.PropertyField(serializedObject.FindProperty("goToPoint"));

        serializedObject.ApplyModifiedProperties();

        if (GUI.changed)
        {
            EditorUtility.SetDirty(target);
        }
    }
}
