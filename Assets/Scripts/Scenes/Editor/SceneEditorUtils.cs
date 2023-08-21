using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public class SceneEditorUtils : Editor
{
    // Start is called before the first frame update

    public static void ShowPopupForScenePath(Rect rect, SerializedProperty property, string description, bool useRect, string[] scenes)
    {
        int selected = 0;

        for (int i = 0; i < scenes.Length; i++)
        {
            if (scenes[i] == property.stringValue)
            {
                selected = i;
            }
        }
        if (useRect)
            selected = EditorGUI.Popup(rect, description, selected, scenes);
        else
            selected = EditorGUILayout.Popup(description, selected, scenes);
        
        property.stringValue = scenes[selected];
    }

    public static void ShowPopupForScenePath(Rect rect, SerializedProperty property, string description, bool useRect, UnityEditor.EditorBuildSettingsScene[] editorScenes)
    {
        string[] scenes = new string[editorScenes.Length];
        for (int i = 0; i < editorScenes.Length; i++)
        {
            scenes[i] = editorScenes[i].path;
        }
        ShowPopupForScenePath(rect, property, description, useRect, scenes);
    }
}
