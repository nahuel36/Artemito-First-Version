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
    private struct NewGlobalPropertyParam{
        
        public PropertyObjectType objectType;
        public VariableType variableType;
    }

    ReorderableList verbsList;
    ReorderableList global_properties_list;

    public void CheckVerbsSameIndex() 
    {
        for (int i = 0; i < serializedObject.FindProperty("verbs").arraySize; i++)
        {
            bool areSame = false;
            int sameIndex = -1;
            for (int j = 0; j < serializedObject.FindProperty("verbs").arraySize; j++)
            {
                if (i != j && 
                    serializedObject.FindProperty("verbs").GetArrayElementAtIndex(i).FindPropertyRelative("index").intValue 
                    == serializedObject.FindProperty("verbs").GetArrayElementAtIndex(j).FindPropertyRelative("index").intValue)
                {
                    areSame = true;
                    sameIndex = j;
                }
            }

            if (areSame)
            { 
                serializedObject.FindProperty("verbIndex").intValue++;
                int newIndex = serializedObject.FindProperty("verbIndex").intValue;
                serializedObject.FindProperty("verbs").GetArrayElementAtIndex(sameIndex).FindPropertyRelative("index").intValue = newIndex;
            }

        }
    }

    

    private void OnEnable()
    {
        CheckVerbsSameIndex();           
            
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
            element.FindPropertyRelative("name").stringValue = "New Verb " + index;
            CheckVerbsSameIndex();
        };
        
        verbsList.onCanRemoveCallback = (ReorderableList list) =>
        {
            return list.count > 1;
        };

        global_properties_list = new ReorderableList(serializedObject, serializedObject.FindProperty("globalPropertiesConfig"), true, true, true, true);
        global_properties_list.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
        {
            EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width / 3, EditorGUIUtility.singleLineHeight), global_properties_list.serializedProperty.GetArrayElementAtIndex(index).FindPropertyRelative("name"), GUIContent.none);

            EditorGUI.PropertyField(new Rect(rect.x + rect.width / 3, rect.y, rect.width / 3, EditorGUIUtility.singleLineHeight), global_properties_list.serializedProperty.GetArrayElementAtIndex(index).FindPropertyRelative("objectTypes"), GUIContent.none);

            EditorGUI.PropertyField(new Rect(rect.x + rect.width / 3 * 2, rect.y, rect.width / 3, EditorGUIUtility.singleLineHeight), global_properties_list.serializedProperty.GetArrayElementAtIndex(index).FindPropertyRelative("variableTypes"), GUIContent.none);
        };
        global_properties_list.drawHeaderCallback = (Rect rect) =>
        {
            EditorGUI.LabelField(rect, "Global Properties");
        };
        global_properties_list.onAddDropdownCallback = (Rect buttonRect, ReorderableList l) =>
        {
            var menu = new GenericMenu();
            menu.AddItem(new GUIContent("characters/boolean"), false, OnAddNewGlobalVar, new NewGlobalPropertyParam() { objectType = PropertyObjectType.character, variableType = VariableType.boolean_type});
            menu.AddItem(new GUIContent("inventory/boolean"), false, OnAddNewGlobalVar, new NewGlobalPropertyParam() { objectType = PropertyObjectType.inventory, variableType = VariableType.boolean_type });
            menu.AddItem(new GUIContent("object/boolean"), false, OnAddNewGlobalVar, new NewGlobalPropertyParam() { objectType = PropertyObjectType.room_object, variableType = VariableType.boolean_type });
            menu.AddItem(new GUIContent("properties container/boolean"), false, OnAddNewGlobalVar, new NewGlobalPropertyParam() { objectType = PropertyObjectType.propertiesContainer, variableType = VariableType.boolean_type });
            menu.AddItem(new GUIContent("dialog option/boolean"), false, OnAddNewGlobalVar, new NewGlobalPropertyParam() { objectType = PropertyObjectType.dialogOption, variableType = VariableType.boolean_type });
            menu.AddItem(new GUIContent("characters/integer"), false, OnAddNewGlobalVar, new NewGlobalPropertyParam() { objectType = PropertyObjectType.character, variableType = VariableType.integer_type});
            menu.AddItem(new GUIContent("inventory/integer"), false, OnAddNewGlobalVar, new NewGlobalPropertyParam() { objectType = PropertyObjectType.inventory, variableType = VariableType.integer_type });
            menu.AddItem(new GUIContent("object/integer"), false, OnAddNewGlobalVar, new NewGlobalPropertyParam() { objectType = PropertyObjectType.room_object, variableType = VariableType.integer_type });
            menu.AddItem(new GUIContent("properties container/integer"), false, OnAddNewGlobalVar, new NewGlobalPropertyParam() { objectType = PropertyObjectType.propertiesContainer, variableType = VariableType.integer_type });
            menu.AddItem(new GUIContent("dialog option/integer"), false, OnAddNewGlobalVar, new NewGlobalPropertyParam() { objectType = PropertyObjectType.dialogOption, variableType = VariableType.integer_type });
            menu.AddItem(new GUIContent("characters/string"), false, OnAddNewGlobalVar, new NewGlobalPropertyParam() { objectType = PropertyObjectType.character, variableType = VariableType.string_type});
            menu.AddItem(new GUIContent("inventory/string"), false, OnAddNewGlobalVar, new NewGlobalPropertyParam() { objectType = PropertyObjectType.inventory, variableType = VariableType.string_type });
            menu.AddItem(new GUIContent("object/string"), false, OnAddNewGlobalVar, new NewGlobalPropertyParam() { objectType = PropertyObjectType.room_object, variableType = VariableType.string_type });
            menu.AddItem(new GUIContent("properties container/string"), false, OnAddNewGlobalVar, new NewGlobalPropertyParam() { objectType = PropertyObjectType.propertiesContainer, variableType = VariableType.string_type });
            menu.AddItem(new GUIContent("dialog option/string"), false, OnAddNewGlobalVar, new NewGlobalPropertyParam() { objectType = PropertyObjectType.dialogOption, variableType = VariableType.string_type });
            menu.ShowAsContext();
        };
    }

    private void OnAddNewGlobalVar(object target)
    {
        NewGlobalPropertyParam newGlobalProperty = (NewGlobalPropertyParam)target;
        var index = global_properties_list.serializedProperty.arraySize;
        global_properties_list.serializedProperty.arraySize++;
        global_properties_list.index = index;
        serializedObject.FindProperty("global_propertiesIndex").intValue++;
        int elementID = serializedObject.FindProperty("global_propertiesIndex").intValue;
        var element = global_properties_list.serializedProperty.GetArrayElementAtIndex(index);
        element.FindPropertyRelative("name").stringValue = "New Global Property " + index;
        element.FindPropertyRelative("ID").intValue = elementID;
        element.FindPropertyRelative("objectTypes").intValue = (int)newGlobalProperty.objectType;
        element.FindPropertyRelative("variableTypes").intValue = (int)newGlobalProperty.variableType;
        serializedObject.ApplyModifiedProperties();
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        verbsList.DoLayoutList();
        global_properties_list.DoLayoutList();

        Dictionary<string, int> tempDict = new Dictionary<string, int>();

        for (int i = 0; i < serializedObject.FindProperty("globalPropertiesConfig").arraySize; i++)
        {
            string name = serializedObject.FindProperty("globalPropertiesConfig").GetArrayElementAtIndex(i).FindPropertyRelative("name").stringValue;
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
        {
            GUIStyle styleRepeated = new GUIStyle();
            styleRepeated.normal.textColor = Color.red;
            styleRepeated.fontSize = 13;
            GUILayout.Label("There are more than one property with the same name", styleRepeated);
        }


        GUILayout.Label("Path Finding Type");
        ((Settings)target).pathFindingType = (Settings.PathFindingType)EditorGUILayout.EnumPopup(((Settings)target).pathFindingType);

        GUILayout.Label("Speech Style");
        ((Settings)target).speechStyle = (Settings.SpeechStyle)EditorGUILayout.EnumPopup(((Settings)target).speechStyle);

        GUILayout.Label("Interaction execute method");
        ((Settings)target).interactionExecuteMethod = (Settings.InteractionExecuteMethod)EditorGUILayout.EnumPopup(((Settings)target).interactionExecuteMethod);

        GUILayout.Label("Objetive position");
        ((Settings)target).objetivePosition = (Settings.ObjetivePosition)EditorGUILayout.EnumPopup(((Settings)target).objetivePosition);

        GUILayout.Label("Show numbers on dialog options");
        ((Settings)target).showNumbersInDialogOptions = EditorGUILayout.Toggle(((Settings)target).showNumbersInDialogOptions);

        GUILayout.Label("Always show all verbs");
        ((Settings)target).alwaysShowAllVerbs = EditorGUILayout.Toggle(((Settings)target).alwaysShowAllVerbs);
        

        serializedObject.ApplyModifiedProperties();
        if (GUI.changed)
        {
            EditorUtility.SetDirty(target);
        }
    }
}

