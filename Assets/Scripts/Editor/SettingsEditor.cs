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
        
        public GlobalPropertyConfig.object_types object_type;
        public bool hasBoolean;
        public bool hasInteger;
        public bool hasString;
    }

    ReorderableList verbsList;
    ReorderableList global_properties_list;

    [System.Flags]
    enum PropertyType
    { 
        boolean = 1 << 0,
        integer = 1 << 2,
        String = 1 << 3
    }
    Dictionary<int,PropertyType> propertiesType;

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

    private void InitializeGlobalPropertiesTypes()
    {
        propertiesType = new Dictionary<int, PropertyType>();
        for (int i = 0; i < serializedObject.FindProperty("globalPropertiesConfig").arraySize; i++)
        {
            PropertyType actualPropertyType = 0;
            if (serializedObject.FindProperty("globalPropertiesConfig").GetArrayElementAtIndex(i).FindPropertyRelative("hasBoolean").boolValue)
                actualPropertyType |= PropertyType.boolean;
            if (serializedObject.FindProperty("globalPropertiesConfig").GetArrayElementAtIndex(i).FindPropertyRelative("hasString").boolValue)
                actualPropertyType |= PropertyType.String;
            if (serializedObject.FindProperty("globalPropertiesConfig").GetArrayElementAtIndex(i).FindPropertyRelative("hasInteger").boolValue)
                actualPropertyType |= PropertyType.integer;
            propertiesType.Add(serializedObject.FindProperty("globalPropertiesConfig").GetArrayElementAtIndex(i).FindPropertyRelative("ID").intValue, actualPropertyType);
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

        InitializeGlobalPropertiesTypes();

        global_properties_list = new ReorderableList(serializedObject, serializedObject.FindProperty("globalPropertiesConfig"), true, true, true, true);
        global_properties_list.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
        {
            EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width / 3, EditorGUIUtility.singleLineHeight), global_properties_list.serializedProperty.GetArrayElementAtIndex(index).FindPropertyRelative("name"), GUIContent.none);

            EditorGUI.PropertyField(new Rect(rect.x + rect.width / 3, rect.y, rect.width / 3, EditorGUIUtility.singleLineHeight), global_properties_list.serializedProperty.GetArrayElementAtIndex(index).FindPropertyRelative("object_type"), GUIContent.none);

            //EditorGUI.PropertyField(new Rect(rect.x + rect.width / 3 * 2, rect.y, rect.width / 3, EditorGUIUtility.singleLineHeight), global_properties_list.serializedProperty.GetArrayElementAtIndex(index).FindPropertyRelative("property_type"), GUIContent.none);
            int globalpropertyIndex = global_properties_list.serializedProperty.GetArrayElementAtIndex(index).FindPropertyRelative("ID").intValue;
            propertiesType[globalpropertyIndex] = (PropertyType)EditorGUI.EnumFlagsField(new Rect(rect.x + rect.width / 3 * 2, rect.y, rect.width / 3, EditorGUIUtility.singleLineHeight), propertiesType[globalpropertyIndex]);
            global_properties_list.serializedProperty.GetArrayElementAtIndex(index).FindPropertyRelative("hasBoolean").boolValue = propertiesType[globalpropertyIndex].HasFlag(PropertyType.boolean);
            global_properties_list.serializedProperty.GetArrayElementAtIndex(index).FindPropertyRelative("hasInteger").boolValue = propertiesType[globalpropertyIndex].HasFlag(PropertyType.integer);
            global_properties_list.serializedProperty.GetArrayElementAtIndex(index).FindPropertyRelative("hasString").boolValue = propertiesType[globalpropertyIndex].HasFlag(PropertyType.String);
        };
        global_properties_list.drawHeaderCallback = (Rect rect) =>
        {
            EditorGUI.LabelField(rect, "Global Properties");
        };
        global_properties_list.onAddDropdownCallback = (Rect buttonRect, ReorderableList l) =>
        {
            var menu = new GenericMenu();
            menu.AddItem(new GUIContent("characters/boolean"), false, OnAddNewGlobalVar, new NewGlobalPropertyParam() { object_type = GlobalPropertyConfig.object_types.characters, hasBoolean = true});
            menu.AddItem(new GUIContent("inventory/boolean"), false, OnAddNewGlobalVar, new NewGlobalPropertyParam() { object_type = GlobalPropertyConfig.object_types.inventory, hasBoolean = true});
            menu.AddItem(new GUIContent("object/boolean"), false, OnAddNewGlobalVar, new NewGlobalPropertyParam() { object_type = GlobalPropertyConfig.object_types.objects, hasBoolean = true});
            menu.AddItem(new GUIContent("properties container/boolean"), false, OnAddNewGlobalVar, new NewGlobalPropertyParam() { object_type = GlobalPropertyConfig.object_types.propertiesContainer, hasBoolean = true });
            menu.AddItem(new GUIContent("dialog option/boolean"), false, OnAddNewGlobalVar, new NewGlobalPropertyParam() { object_type = GlobalPropertyConfig.object_types.dialogOption, hasBoolean = true });
            menu.AddItem(new GUIContent("characters/integer"), false, OnAddNewGlobalVar, new NewGlobalPropertyParam() { object_type = GlobalPropertyConfig.object_types.characters, hasInteger = true});
            menu.AddItem(new GUIContent("inventory/integer"), false, OnAddNewGlobalVar, new NewGlobalPropertyParam() { object_type = GlobalPropertyConfig.object_types.inventory, hasInteger = true });
            menu.AddItem(new GUIContent("object/integer"), false, OnAddNewGlobalVar, new NewGlobalPropertyParam() { object_type = GlobalPropertyConfig.object_types.objects, hasInteger = true });
            menu.AddItem(new GUIContent("properties container/integer"), false, OnAddNewGlobalVar, new NewGlobalPropertyParam() { object_type = GlobalPropertyConfig.object_types.propertiesContainer, hasInteger = true });
            menu.AddItem(new GUIContent("dialog option/integer"), false, OnAddNewGlobalVar, new NewGlobalPropertyParam() { object_type = GlobalPropertyConfig.object_types.dialogOption, hasInteger = true });
            menu.AddItem(new GUIContent("characters/string"), false, OnAddNewGlobalVar, new NewGlobalPropertyParam() { object_type = GlobalPropertyConfig.object_types.characters, hasString = true});
            menu.AddItem(new GUIContent("inventory/string"), false, OnAddNewGlobalVar, new NewGlobalPropertyParam() { object_type = GlobalPropertyConfig.object_types.inventory, hasString = true });
            menu.AddItem(new GUIContent("object/string"), false, OnAddNewGlobalVar, new NewGlobalPropertyParam() { object_type = GlobalPropertyConfig.object_types.objects, hasString = true });
            menu.AddItem(new GUIContent("properties container/string"), false, OnAddNewGlobalVar, new NewGlobalPropertyParam() { object_type = GlobalPropertyConfig.object_types.propertiesContainer, hasString = true });
            menu.AddItem(new GUIContent("dialog option/string"), false, OnAddNewGlobalVar, new NewGlobalPropertyParam() { object_type = GlobalPropertyConfig.object_types.dialogOption, hasString = true });
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
        element.FindPropertyRelative("object_type").intValue = (int)newGlobalProperty.object_type;
        element.FindPropertyRelative("hasBoolean").boolValue = newGlobalProperty.hasBoolean;
        element.FindPropertyRelative("hasInteger").boolValue = newGlobalProperty.hasInteger;
        element.FindPropertyRelative("hasString").boolValue = newGlobalProperty.hasString;
        serializedObject.ApplyModifiedProperties();
        InitializeGlobalPropertiesTypes();
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

