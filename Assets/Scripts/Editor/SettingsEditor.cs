using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

[CustomEditor(typeof(Settings))]
public class SettingsEditor : Editor
{
    ReorderableList modesList;
    ReorderableList global_variables_list;

    private void OnEnable()
    {
        modesList = new ReorderableList(serializedObject, serializedObject.FindProperty("modes"), true, true, true, true);
        modesList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
        {
            EditorGUI.PropertyField(rect, modesList.serializedProperty.GetArrayElementAtIndex(index), GUIContent.none);
        };
        modesList.drawHeaderCallback = (Rect rect) =>
        {
            EditorGUI.LabelField(rect, "Modes");
        };
        modesList.onAddCallback = (ReorderableList list) =>
        {
            var index = list.serializedProperty.arraySize;
            list.serializedProperty.arraySize++;
            list.index = index;
            var element = list.serializedProperty.GetArrayElementAtIndex(index);
            element.stringValue = "New Mode " + index;
        };
        modesList.onCanRemoveCallback = (ReorderableList list) =>
        {
            return list.count > 1;
        };

        global_variables_list = new ReorderableList(serializedObject, serializedObject.FindProperty("global_variables"), true, true, true, true);
        global_variables_list.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
        {
            EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width / 2, EditorGUIUtility.singleLineHeight), global_variables_list.serializedProperty.GetArrayElementAtIndex(index).FindPropertyRelative("name"), GUIContent.none);

            EditorGUI.PropertyField(new Rect(rect.x + rect.width / 2, rect.y, rect.width / 2, EditorGUIUtility.singleLineHeight), global_variables_list.serializedProperty.GetArrayElementAtIndex(index).FindPropertyRelative("type"), GUIContent.none);
        };
        global_variables_list.drawHeaderCallback = (Rect rect) =>
        {
            EditorGUI.LabelField(rect, "Global Variables");
        };
        global_variables_list.onAddCallback = (ReorderableList list) =>
        {
            var index = list.serializedProperty.arraySize;
            list.serializedProperty.arraySize++;
            list.index = index;
            var element = list.serializedProperty.GetArrayElementAtIndex(index);
            element.FindPropertyRelative("name").stringValue = "New Global Variable " + index;
            element.FindPropertyRelative("ID").intValue = -1;
            element.FindPropertyRelative("type").intValue = 0;
        };
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        modesList.DoLayoutList();
        global_variables_list.DoLayoutList();
        serializedObject.ApplyModifiedProperties();
        if (GUI.changed)
        {
            EditorUtility.SetDirty(target);
        }
    }
}

