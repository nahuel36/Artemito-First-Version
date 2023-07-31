using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEditorInternal;

[CustomEditor(typeof(ScenesConfiguration))]
public class ScenesConfigurationEditor : Editor
{

    ReorderableList Zones;
    Dictionary<string, ReorderableList> ZonesScenes = new Dictionary<string, ReorderableList>();
    private void ShowPopupForScenePath(Rect rect, SerializedProperty property, string description, bool useRect)
    {
        int selected = 0;

        string[] paths = new string[EditorBuildSettings.scenes.Length];
        for (int i = 0; i < EditorBuildSettings.scenes.Length; i++)
        {
            paths[i] = EditorBuildSettings.scenes[i].path;
            if (paths[i] == property.stringValue)
            {
                selected = i;
            }
        }
        if(useRect)
            selected = EditorGUI.Popup(rect, description, selected, paths);
        else
            selected = EditorGUILayout.Popup(description, selected, paths);

        property.stringValue = paths[selected];
    }

    private void OnEnable()
    {
        ScenesConfiguration myTarget = (ScenesConfiguration)target;
        Zones = new ReorderableList(serializedObject.FindProperty("zones").serializedObject, serializedObject.FindProperty("zones")) { 
            drawElementCallback = (rect, index, active, focus) =>
            {
                string key = serializedObject.FindProperty("zones").GetArrayElementAtIndex(index).propertyPath;

                if (!ZonesScenes.ContainsKey(key))
                {
                    ReorderableList list = new ReorderableList(serializedObject.FindProperty("zones").GetArrayElementAtIndex(index).FindPropertyRelative("zoneScenes").serializedObject, serializedObject.FindProperty("zones").GetArrayElementAtIndex(index).FindPropertyRelative("zoneScenes"))
                    {
                        drawElementCallback = (rectS, indexS, activeS, focusS) =>
                        {
                            ShowPopupForScenePath(rectS, serializedObject.FindProperty("zones").GetArrayElementAtIndex(index).FindPropertyRelative("zoneScenes").GetArrayElementAtIndex(indexS), "game scene n" + (indexS+1).ToString(), true);
                        },
                        drawHeaderCallback = (rect) =>
                        {
                            EditorGUI.LabelField(rect, "Ingame zone " + (index+1).ToString() + " scenes");
                        }
                    };
                    ZonesScenes.Add(key, list);
                }
                ZonesScenes[key].DoList(rect);

            },
            elementHeightCallback = (index) =>
            {
                return EditorGUIUtility.singleLineHeight * (4 + serializedObject.FindProperty("zones").GetArrayElementAtIndex(index).FindPropertyRelative("zoneScenes").arraySize * 1.35f);
            },
            drawHeaderCallback = (rect) =>
            {
                EditorGUI.LabelField(rect, "Ingame zones");
            }
        };
    }

    public override void OnInspectorGUI()
    {
        Rect rect = new Rect();

        ShowPopupForScenePath(rect, serializedObject.FindProperty("canvas"), "Canvas Ingame Scene",false);

        ShowPopupForScenePath(rect,serializedObject.FindProperty("mainMenu"), "mainMenu", false);

        ShowPopupForScenePath(rect,serializedObject.FindProperty("options"), "options", false);

        ShowPopupForScenePath(rect,serializedObject.FindProperty("saveAndLoad"), "saveAndLoad", false);

        Zones.DoLayoutList();
       

        serializedObject.ApplyModifiedProperties();

        if (GUI.changed)
        {
            EditorUtility.SetDirty(target);
        }
    }
}
