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
        public bool hasBoolean;
        public bool hasInteger;
        public bool hasString;
    }

    ReorderableList verbsList;
    ReorderableList global_variables_list;

    [System.Flags]
    enum VariableType
    { 
        boolean = 1 << 0,
        integer = 1 << 2,
        String = 1 << 3
    }
    Dictionary<int,VariableType> variablesType;

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

    private void InitializeGlobalVariableTypes()
    {
        variablesType = new Dictionary<int, VariableType>();
        for (int i = 0; i < serializedObject.FindProperty("global_variables").arraySize; i++)
        {
            VariableType actualVariableType = 0;
            if (serializedObject.FindProperty("global_variables").GetArrayElementAtIndex(i).FindPropertyRelative("hasBoolean").boolValue)
                actualVariableType |= VariableType.boolean;
            if (serializedObject.FindProperty("global_variables").GetArrayElementAtIndex(i).FindPropertyRelative("hasString").boolValue)
                actualVariableType |= VariableType.String;
            if (serializedObject.FindProperty("global_variables").GetArrayElementAtIndex(i).FindPropertyRelative("hasInteger").boolValue)
                actualVariableType |= VariableType.integer;
            variablesType.Add(serializedObject.FindProperty("global_variables").GetArrayElementAtIndex(i).FindPropertyRelative("ID").intValue, actualVariableType);
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

        InitializeGlobalVariableTypes();

        global_variables_list = new ReorderableList(serializedObject, serializedObject.FindProperty("global_variables"), true, true, true, true);
        global_variables_list.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
        {
            EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width / 3, EditorGUIUtility.singleLineHeight), global_variables_list.serializedProperty.GetArrayElementAtIndex(index).FindPropertyRelative("name"), GUIContent.none);

            EditorGUI.PropertyField(new Rect(rect.x + rect.width / 3, rect.y, rect.width / 3, EditorGUIUtility.singleLineHeight), global_variables_list.serializedProperty.GetArrayElementAtIndex(index).FindPropertyRelative("object_type"), GUIContent.none);

            //EditorGUI.PropertyField(new Rect(rect.x + rect.width / 3 * 2, rect.y, rect.width / 3, EditorGUIUtility.singleLineHeight), global_variables_list.serializedProperty.GetArrayElementAtIndex(index).FindPropertyRelative("variable_type"), GUIContent.none);
            int globalvariableIndex = global_variables_list.serializedProperty.GetArrayElementAtIndex(index).FindPropertyRelative("ID").intValue;
            variablesType[globalvariableIndex] = (VariableType)EditorGUI.EnumFlagsField(new Rect(rect.x + rect.width / 3 * 2, rect.y, rect.width / 3, EditorGUIUtility.singleLineHeight), variablesType[globalvariableIndex]);
            global_variables_list.serializedProperty.GetArrayElementAtIndex(index).FindPropertyRelative("hasBoolean").boolValue = variablesType[globalvariableIndex].HasFlag(VariableType.boolean);
            global_variables_list.serializedProperty.GetArrayElementAtIndex(index).FindPropertyRelative("hasInteger").boolValue = variablesType[globalvariableIndex].HasFlag(VariableType.integer);
            global_variables_list.serializedProperty.GetArrayElementAtIndex(index).FindPropertyRelative("hasString").boolValue = variablesType[globalvariableIndex].HasFlag(VariableType.String);
        };
        global_variables_list.drawHeaderCallback = (Rect rect) =>
        {
            EditorGUI.LabelField(rect, "Global Variables");
        };
        global_variables_list.onAddDropdownCallback = (Rect buttonRect, ReorderableList l) =>
        {
            var menu = new GenericMenu();
            menu.AddItem(new GUIContent("characters/boolean"), false, OnAddNewGlobalVar, new NewGlobalVariableParam() { object_type = GlobalVariableProperty.object_types.characters, hasBoolean = true});
            menu.AddItem(new GUIContent("inventory/boolean"), false, OnAddNewGlobalVar, new NewGlobalVariableParam() { object_type = GlobalVariableProperty.object_types.inventory, hasBoolean = true});
            menu.AddItem(new GUIContent("object/boolean"), false, OnAddNewGlobalVar, new NewGlobalVariableParam() { object_type = GlobalVariableProperty.object_types.objects, hasBoolean = true});
            menu.AddItem(new GUIContent("variable container/boolean"), false, OnAddNewGlobalVar, new NewGlobalVariableParam() { object_type = GlobalVariableProperty.object_types.variableContainer, hasBoolean = true });
            menu.AddItem(new GUIContent("characters/integer"), false, OnAddNewGlobalVar, new NewGlobalVariableParam() { object_type = GlobalVariableProperty.object_types.characters, hasInteger = true});
            menu.AddItem(new GUIContent("inventory/integer"), false, OnAddNewGlobalVar, new NewGlobalVariableParam() { object_type = GlobalVariableProperty.object_types.inventory, hasInteger = true });
            menu.AddItem(new GUIContent("object/integer"), false, OnAddNewGlobalVar, new NewGlobalVariableParam() { object_type = GlobalVariableProperty.object_types.objects, hasInteger = true });
            menu.AddItem(new GUIContent("variable container/integer"), false, OnAddNewGlobalVar, new NewGlobalVariableParam() { object_type = GlobalVariableProperty.object_types.variableContainer, hasInteger = true });
            menu.AddItem(new GUIContent("characters/string"), false, OnAddNewGlobalVar, new NewGlobalVariableParam() { object_type = GlobalVariableProperty.object_types.characters, hasString = true});
            menu.AddItem(new GUIContent("inventory/string"), false, OnAddNewGlobalVar, new NewGlobalVariableParam() { object_type = GlobalVariableProperty.object_types.inventory, hasString = true });
            menu.AddItem(new GUIContent("object/string"), false, OnAddNewGlobalVar, new NewGlobalVariableParam() { object_type = GlobalVariableProperty.object_types.objects, hasString = true });
            menu.AddItem(new GUIContent("variable container/string"), false, OnAddNewGlobalVar, new NewGlobalVariableParam() { object_type = GlobalVariableProperty.object_types.variableContainer, hasString = true });
            menu.ShowAsContext();
        };
    }

    private void OnAddNewGlobalVar(object target)
    {
        NewGlobalVariableParam newGlobalVariable = (NewGlobalVariableParam)target;
        var index = global_variables_list.serializedProperty.arraySize;
        global_variables_list.serializedProperty.arraySize++;
        global_variables_list.index = index;
        serializedObject.FindProperty("global_variableIndex").intValue++;
        int elementID = serializedObject.FindProperty("global_variableIndex").intValue;
        var element = global_variables_list.serializedProperty.GetArrayElementAtIndex(index);
        element.FindPropertyRelative("name").stringValue = "New Global Variable " + index;
        element.FindPropertyRelative("ID").intValue = elementID;
        element.FindPropertyRelative("object_type").intValue = (int)newGlobalVariable.object_type;
        element.FindPropertyRelative("hasBoolean").boolValue = newGlobalVariable.hasBoolean;
        element.FindPropertyRelative("hasInteger").boolValue = newGlobalVariable.hasInteger;
        element.FindPropertyRelative("hasString").boolValue = newGlobalVariable.hasString;
        serializedObject.ApplyModifiedProperties();
        InitializeGlobalVariableTypes();
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

