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
                            SceneEditorUtils.ShowPopupForScenePath(rectS, serializedObject.FindProperty("zones").GetArrayElementAtIndex(index).FindPropertyRelative("zoneScenes").GetArrayElementAtIndex(indexS), "game scene n" + (indexS+1).ToString(), true, EditorBuildSettings.scenes);
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

        SceneEditorUtils.ShowPopupForScenePath(rect, serializedObject.FindProperty("canvas"), "Canvas ingame Scene",false, EditorBuildSettings.scenes);

        SceneEditorUtils.ShowPopupForScenePath(rect,serializedObject.FindProperty("mainMenu"), "Main menu scene", false, EditorBuildSettings.scenes);

        SceneEditorUtils.ShowPopupForScenePath(rect,serializedObject.FindProperty("options"), "Options menu scene", false, EditorBuildSettings.scenes);

        SceneEditorUtils.ShowPopupForScenePath(rect,serializedObject.FindProperty("saveAndLoad"), "Save And Load", false, EditorBuildSettings.scenes);

        SceneEditorUtils.ShowPopupForScenePath(rect, serializedObject.FindProperty("transition"), "Scene transition", false, EditorBuildSettings.scenes);

        Zones.DoLayoutList();
       

        serializedObject.ApplyModifiedProperties();

        if (GUI.changed)
        {
            EditorUtility.SetDirty(target);
        }
    }
}
