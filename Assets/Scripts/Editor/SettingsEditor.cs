using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Linq;
using System;

[CustomEditor(typeof(Settings))]
public class SettingsEditor : Editor
{
    private struct NewGlobalVariableParam{
        
        public GlobalVariableProperty.object_types object_type;
        public GlobalVariableProperty.variable_types variable_type;
    }

    ReorderableList verbsList;
    ReorderableList global_variables_list;
 

    private void OnEnable()
    {
        Settings myTarget = (Settings)target;
                
        verbsList = new ReorderableList(serializedObject, serializedObject.FindProperty("verbs"), true, true, true, true);
        verbsList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
        {
            EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width/3, rect.height), verbsList.serializedProperty.GetArrayElementAtIndex(index).FindPropertyRelative("name"), GUIContent.none);
            GUIStyle style = new GUIStyle();
            style.alignment = TextAnchor.MiddleLeft;
            if(EditorGUIUtility.isProSkin)
                style.normal.textColor = Color.white;
            else
                style.normal.textColor = Color.black;
            EditorGUI.PropertyField(new Rect(rect.x + rect.width/3 + 1, rect.y, rect.width/3, rect.height), verbsList.serializedProperty.GetArrayElementAtIndex(index).FindPropertyRelative("isLikeUse"), GUIContent.none);
            EditorGUI.LabelField(new Rect(rect.x + rect.width / 3 + 17, rect.y, rect.width / 3, rect.height), "isLikeUse", style);
            EditorGUI.PropertyField(new Rect(rect.x + rect.width / 3 * 2 + 1, rect.y, rect.width / 3, rect.height), verbsList.serializedProperty.GetArrayElementAtIndex(index).FindPropertyRelative("isLikeGive"), GUIContent.none);
            EditorGUI.LabelField(new Rect(rect.x + rect.width /3 * 2 + 17, rect.y, rect.width / 3, rect.height), "isLikeGive", style);
        };
        verbsList.drawHeaderCallback = (Rect rect) =>
        {
            EditorGUI.LabelField(rect, "Verbs");
        };
        verbsList.onAddCallback = (ReorderableList list) =>
        {
            var index = list.serializedProperty.arraySize;
            list.serializedProperty.arraySize++;
            list.index = index;
            var element = list.serializedProperty.GetArrayElementAtIndex(index);
            element.stringValue = "New Verb " + index;
        };
        
        verbsList.onCanRemoveCallback = (ReorderableList list) =>
        {
            return list.count > 1;
        };

        global_variables_list = new ReorderableList(serializedObject, serializedObject.FindProperty("global_variables"), true, true, true, true);
        global_variables_list.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
        {
            EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width / 3, EditorGUIUtility.singleLineHeight), global_variables_list.serializedProperty.GetArrayElementAtIndex(index).FindPropertyRelative("name"), GUIContent.none);

            EditorGUI.PropertyField(new Rect(rect.x + rect.width / 3, rect.y, rect.width / 3, EditorGUIUtility.singleLineHeight), global_variables_list.serializedProperty.GetArrayElementAtIndex(index).FindPropertyRelative("object_type"), GUIContent.none);

            EditorGUI.PropertyField(new Rect(rect.x + rect.width / 3 * 2, rect.y, rect.width / 3, EditorGUIUtility.singleLineHeight), global_variables_list.serializedProperty.GetArrayElementAtIndex(index).FindPropertyRelative("variable_type"), GUIContent.none);
        };
        global_variables_list.drawHeaderCallback = (Rect rect) =>
        {
            EditorGUI.LabelField(rect, "Global Variables");
        };
        global_variables_list.onAddDropdownCallback = (Rect buttonRect, ReorderableList l) =>
        {
            var menu = new GenericMenu();
            menu.AddItem(new GUIContent("characters/boolean"), false, OnAddNewGlobalVar, new NewGlobalVariableParam() { object_type = GlobalVariableProperty.object_types.characters, variable_type = GlobalVariableProperty.variable_types.boolean });
            menu.AddItem(new GUIContent("inventory/boolean"), false, OnAddNewGlobalVar, new NewGlobalVariableParam() { object_type = GlobalVariableProperty.object_types.inventory, variable_type = GlobalVariableProperty.variable_types.boolean });
            menu.AddItem(new GUIContent("object/boolean"), false, OnAddNewGlobalVar, new NewGlobalVariableParam() { object_type = GlobalVariableProperty.object_types.objects, variable_type = GlobalVariableProperty.variable_types.boolean });
            menu.AddItem(new GUIContent("characters/integer"), false, OnAddNewGlobalVar, new NewGlobalVariableParam() { object_type = GlobalVariableProperty.object_types.characters, variable_type = GlobalVariableProperty.variable_types.integer });
            menu.AddItem(new GUIContent("inventory/integer"), false, OnAddNewGlobalVar, new NewGlobalVariableParam() { object_type = GlobalVariableProperty.object_types.inventory, variable_type = GlobalVariableProperty.variable_types.integer});
            menu.AddItem(new GUIContent("object/integer"), false, OnAddNewGlobalVar, new NewGlobalVariableParam() { object_type = GlobalVariableProperty.object_types.objects, variable_type = GlobalVariableProperty.variable_types.integer });
            menu.AddItem(new GUIContent("characters/string"), false, OnAddNewGlobalVar, new NewGlobalVariableParam() { object_type = GlobalVariableProperty.object_types.characters, variable_type = GlobalVariableProperty.variable_types.String});
            menu.AddItem(new GUIContent("inventory/string"), false, OnAddNewGlobalVar, new NewGlobalVariableParam() { object_type = GlobalVariableProperty.object_types.inventory, variable_type = GlobalVariableProperty.variable_types.String });
            menu.AddItem(new GUIContent("object/string"), false, OnAddNewGlobalVar, new NewGlobalVariableParam() { object_type = GlobalVariableProperty.object_types.objects, variable_type = GlobalVariableProperty.variable_types.String});
            menu.ShowAsContext();
        };
    }

    private void OnAddNewGlobalVar(object target)
    {
        NewGlobalVariableParam newGlobalVariable = (NewGlobalVariableParam)target;
        var index = global_variables_list.serializedProperty.arraySize;
        global_variables_list.serializedProperty.arraySize++;
        global_variables_list.index = index;
        var element = global_variables_list.serializedProperty.GetArrayElementAtIndex(index);
        element.FindPropertyRelative("name").stringValue = "New Global Variable " + index;
        element.FindPropertyRelative("ID").intValue = -1;
        element.FindPropertyRelative("object_type").intValue = (int)newGlobalVariable.object_type;
        element.FindPropertyRelative("variable_type").intValue = (int)newGlobalVariable.variable_type;
        serializedObject.ApplyModifiedProperties();
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        verbsList.DoLayoutList();
        global_variables_list.DoLayoutList();

        Dictionary<string, int> tempDict = new Dictionary<string, int>();

        for (int i = 0; i < serializedObject.FindProperty("global_variables").arraySize; i++)
        {
            string name = serializedObject.FindProperty("global_variables").GetArrayElementAtIndex(i).FindPropertyRelative("name").stringValue;
            if (tempDict.ContainsKey(name))
                tempDict[name]++;
            else
                tempDict.Add(name, 1);
        }

        bool repeated = false;
        foreach(var tempElement in tempDict.Keys)
        {
            if (tempDict[tempElement] > 1)
                repeated = true;
        }

        if (repeated)
            GUILayout.Label("There are more than one variable with the same name", EditorStyles.boldLabel);

        GUILayout.Label("Path Finding Type");
        ((Settings)target).pathFindingType = (Settings.PathFindingType)EditorGUILayout.EnumPopup(((Settings)target).pathFindingType);

        GUILayout.Label("Speech Style");
        ((Settings)target).speechStyle = (Settings.SpeechStyle)EditorGUILayout.EnumPopup(((Settings)target).speechStyle);

        GUILayout.Label("Interaction execute method");
        ((Settings)target).interactionExecuteMethod = (Settings.InteractionExecuteMethod)EditorGUILayout.EnumPopup(((Settings)target).interactionExecuteMethod);

        GUILayout.Label("Objetive position");
        ((Settings)target).objetivePosition = (Settings.ObjetivePosition)EditorGUILayout.EnumPopup(((Settings)target).objetivePosition);

        serializedObject.ApplyModifiedProperties();
        if (GUI.changed)
        {
            EditorUtility.SetDirty(target);
        }
    }
}

