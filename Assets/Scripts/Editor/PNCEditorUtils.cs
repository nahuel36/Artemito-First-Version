using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;



public static class PNCEditorUtils
{

    


    public static void InitializeGlobalProperties(System.Enum type, ref GlobalProperty[] globalProperties)
    {
        Settings settings = Resources.Load<Settings>("Settings/Settings");

        List<GlobalProperty> tempGlobalVarList = new List<GlobalProperty>();
        for (int i = 0; i < settings.globalPropertiesConfig.Length; i++)
        {
            bool founded = false;
            for (int j = 0; j < globalProperties.Length; j++)
            {
                if ((settings.globalPropertiesConfig[i].ID == -1 && globalProperties[j].name == settings.globalPropertiesConfig[i].name) ||
                    (settings.globalPropertiesConfig[i].ID != -1 && globalProperties[j].globalID == -1 && globalProperties[j].name == settings.globalPropertiesConfig[i].name) ||
                    (settings.globalPropertiesConfig[i].ID != -1 && globalProperties[j].globalID != -1 && globalProperties[j].globalID == settings.globalPropertiesConfig[i].ID))
                {
                    globalProperties[j].name = settings.globalPropertiesConfig[i].name;

                    if (globalProperties[j].globalID == -1)
                    {
                        globalProperties[j].globalID = settings.globalPropertiesConfig[i].ID;
                    }

                    globalProperties[j].config = settings.globalPropertiesConfig[i];

                    if (settings.globalPropertiesConfig[i].objectTypes.HasFlag(type))
                        tempGlobalVarList.Add(globalProperties[j]);
                    founded = true;
                }
            }
            if (founded == false)
            {
                GlobalProperty tempVar = new GlobalProperty();
                tempVar.name = settings.globalPropertiesConfig[i].name;
                tempVar.globalID = settings.globalPropertiesConfig[i].ID;
                tempVar.config = settings.globalPropertiesConfig[i];
                tempVar.expandedInInspector = true;
                if (settings.globalPropertiesConfig[i].objectTypes.HasFlag(type))
                    tempGlobalVarList.Add(tempVar);
            }
        }

        globalProperties = tempGlobalVarList.ToArray();
    }

    public static void InitializeLocalProperties(out ReorderableList list, SerializedObject serializedObject, SerializedProperty property)
    {
        list = new ReorderableList(serializedObject, property, true, true, true, true)
        {
            drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                SerializedProperty element = property.GetArrayElementAtIndex(index);

                rect.height = EditorGUIUtility.singleLineHeight;
                EditorGUI.PropertyField(rect, element.FindPropertyRelative("name"));

                rect.y += EditorGUIUtility.singleLineHeight;
                EditorGUI.PropertyField(rect, element.FindPropertyRelative("variableTypes"));

                ShowProperty(ref rect, element, PropertyActionType.anyLocal ,  VariableType.boolean_type, true);

                ShowProperty(ref rect, element, PropertyActionType.anyLocal, VariableType.integer_type, true);

                ShowProperty(ref rect, element, PropertyActionType.anyLocal, VariableType.string_type, true);

                

            },
            elementHeightCallback = (int index) =>
            {
                SerializedProperty element = property.GetArrayElementAtIndex(index);

                float height = 4f;
                if ((element.FindPropertyRelative("variableTypes").enumValueFlag & (int)VariableType.boolean_type)!=0)
                    height += 2;
                if ((element.FindPropertyRelative("variableTypes").enumValueFlag & (int)VariableType.integer_type) != 0)
                    height += 2;
                if ((element.FindPropertyRelative("variableTypes").enumValueFlag & (int)VariableType.string_type) != 0)
                    height += 2;
                return height * EditorGUIUtility.singleLineHeight;
            },
            drawHeaderCallback = (rect) =>
            {
                EditorGUI.LabelField(rect, "Local Properties");
            }

        };

    }

    public static bool VerificateLocalPropertiesOnRect(ref LocalProperty[] properties, ref SerializedProperty properties_serialized, Rect? rect = null)
    {

        var group = properties.GroupBy(prop => prop.name, (vari) => new { Count = vari.name.Count() });
        bool repeated = false;

        foreach (var vari in group)
        {
            if (vari.Count() > 1)
                repeated = true;

        }
        if (repeated)
        {
            GUIStyle style = new GUIStyle();
            style.normal.textColor = Color.red;
            style.fontSize = 12;

            if (rect == null)
                GUILayout.Label("<b>There are more than one local property with the same name</b>", style);
            else
                GUI.Label(rect.Value, "<b>There are more than one local property with the same name</b>", style);

            return true;
        }
        return false;

    }

    public static void ShowLocalPropertiesOnRect(ReorderableList list, ref LocalProperty[] local_properties, ref SerializedProperty local_properties_serialized, Rect? rect = null)
    {
        if (rect == null)
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUIStyle tittleStyle = new GUIStyle();
            tittleStyle.normal.textColor = Color.white;
            tittleStyle.fontSize = 14;
            GUILayout.Label("<b>Local Properties</b>", tittleStyle);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            list.DoLayoutList();

            PNCEditorUtils.VerificateLocalPropertiesOnRect(ref local_properties, ref local_properties_serialized, rect);
        }
        else
        {
            Rect newRect = rect.Value;

            if (PNCEditorUtils.VerificateLocalPropertiesOnRect(ref local_properties, ref local_properties_serialized, rect))
                newRect.y += EditorGUIUtility.singleLineHeight;

            list.DoList(newRect);
        }
    }


    private static void ShowProperty(ref Rect rect, SerializedProperty element, PropertyActionType actionType, VariableType variableType, bool useRect = false)
    {
        Rect getRect(Rect rectParam)
        {
            if (useRect == false) return EditorGUILayout.GetControlRect();
            else return rectParam;
        }

        bool hasProperty = false;
        string typeString = "";

        if ((variableType & VariableType.boolean_type) != 0)
            typeString = "boolean";
        else if ((variableType & VariableType.integer_type) != 0)
            typeString = "integer";
        else if ((variableType & VariableType.string_type) != 0)
            typeString = "string";
        else if ((variableType & VariableType.float_type) != 0)
            typeString = "float";

        if ((actionType & PropertyActionType.anyGlobal)!=0)
        {
            hasProperty = ((variableType & (VariableType)element.FindPropertyRelative("config").FindPropertyRelative("variableTypes").enumValueFlag)!=0);
        }
        else if((actionType & PropertyActionType.anyLocal)!=0)
        {
            hasProperty = ((variableType & (VariableType)element.FindPropertyRelative("variableTypes").enumValueFlag) != 0);
        }

        if (hasProperty)
        {
            rect.y += EditorGUIUtility.singleLineHeight;
            if (element.FindPropertyRelative(typeString + "Default").boolValue)
            {
                EditorGUI.LabelField(getRect(rect), typeString + " value: default");
                rect.y += EditorGUIUtility.singleLineHeight;
                if (GUI.Button(getRect(rect), "set " + typeString + " value"))
                {
                    element.FindPropertyRelative(typeString + "Default").boolValue = false;
                }
            }
            else
            {
                if((variableType & VariableType.boolean_type) != 0)
                    element.FindPropertyRelative("boolean").boolValue =
                        EditorGUI.Toggle(getRect(rect), "boolean value:", element.FindPropertyRelative("boolean").boolValue);
                else if((variableType & VariableType.integer_type)!=0)
                    element.FindPropertyRelative("integer").intValue =
                        EditorGUI.IntField(getRect(rect), "integer value:", element.FindPropertyRelative("integer").intValue);
                else if((variableType & VariableType.string_type) != 0)
                    element.FindPropertyRelative("String").stringValue =
                        EditorGUI.TextField(getRect(rect), "string value:", element.FindPropertyRelative("String").stringValue);
                else if((variableType & VariableType.float_type) != 0)
                    element.FindPropertyRelative("float").floatValue =
                        EditorGUI.FloatField(getRect(rect), "float value:", element.FindPropertyRelative("float").floatValue);

                rect.y += EditorGUIUtility.singleLineHeight;
                if (GUI.Button(getRect(rect), "set " + typeString + " default value"))
                {
                    element.FindPropertyRelative(typeString + "Default").boolValue = true;
                }
            }
        }
    
    }

    private static void PropertyInteraction(ref Rect rect, SerializedProperty interactionSerialized, PNCPropertyInterface propertyObject, PropertyActionType propertyActionType, VariableType variableType)
    {
        string propertyTypeString = "";
        if ((propertyActionType & PropertyActionType.anyGlobal)!=0)
        {
            propertyTypeString = "global";
        }
        else if((propertyActionType & PropertyActionType.anyLocal) != 0)
        {
            propertyTypeString = "local";
        }

        int index = interactionSerialized.FindPropertyRelative(propertyTypeString + "PropertySelected").intValue;
        bool hasProperty = false;
        string variableTypeString = "";
        if ((propertyActionType & PropertyActionType.anyGlobal)!=0)
        {
            hasProperty = (variableType & propertyObject.GlobalProperties[index].config.variableTypes) != 0;
        }
        else if ((propertyActionType & PropertyActionType.anyLocal)!=0)
        {
            hasProperty = (variableType & propertyObject.LocalProperties[index].variableTypes)!=0;
            
        }

        if (variableType == VariableType.boolean_type)
            variableTypeString = "Boolean";
        else if (variableType == VariableType.integer_type)
            variableTypeString = "Integer";
        else if (variableType == VariableType.string_type)
            variableTypeString = "String";
        else if (variableType == VariableType.float_type)
            variableTypeString = "Float";


        if (hasProperty)
        {
            if (CheckArePropertyInteraction(PropertyObjectType.any, PropertyActionType.anySet, interactionSerialized) || CheckArePropertyInteraction(PropertyObjectType.any, PropertyActionType.anyGet, interactionSerialized))
            {
                string actionString = "";
                string editorDescription = "";

                if (CheckArePropertyInteraction(PropertyObjectType.any, PropertyActionType.anySet, interactionSerialized))
                {
                    actionString = "change";
                    if ((variableType & (VariableType.integer_type | VariableType.float_type))!=0)
                    {
                        if (interactionSerialized.FindPropertyRelative("changeIntegerOrFloatOperation").enumValueIndex == (int)Interaction.ChangeIntegerOrFloatOperation.add)
                            editorDescription = "value to add";
                        if (interactionSerialized.FindPropertyRelative("changeIntegerOrFloatOperation").enumValueIndex == (int)Interaction.ChangeIntegerOrFloatOperation.subtract)
                            editorDescription = "value to subtract";
                        if (interactionSerialized.FindPropertyRelative("changeIntegerOrFloatOperation").enumValueIndex == (int)Interaction.ChangeIntegerOrFloatOperation.set)
                            editorDescription = "value to set";
                    }
                    else if ((variableType & VariableType.string_type)!=0)
                    {
                        if (interactionSerialized.FindPropertyRelative("changeStringOperation").enumValueIndex == (int)Interaction.ChangeStringOperation.change)
                            editorDescription = "value to set";
                        else if (interactionSerialized.FindPropertyRelative("changeStringOperation").enumValueIndex == (int)Interaction.ChangeStringOperation.replace)
                            editorDescription = "replace with";
                    }
                    else 
                        editorDescription = "value to set";
                }
                else if (CheckArePropertyInteraction(PropertyObjectType.any, PropertyActionType.anyGet, interactionSerialized))
                {
                    actionString = "compare";
                    if ((variableType & (VariableType.integer_type | VariableType.float_type))!=0)
                    {
                        if (interactionSerialized.FindPropertyRelative("compareIntegerOrFloatOperation").enumValueIndex == (int)Interaction.CompareIntegerOrFloatOperation.areEqual)
                            editorDescription = "value to compare";
                        if (interactionSerialized.FindPropertyRelative("compareIntegerOrFloatOperation").enumValueIndex == (int)Interaction.CompareIntegerOrFloatOperation.isGreaterThan)
                            editorDescription = "property is greater than value";
                        if (interactionSerialized.FindPropertyRelative("compareIntegerOrFloatOperation").enumValueIndex == (int)Interaction.CompareIntegerOrFloatOperation.isLessThan)
                            editorDescription = "property is less than value";
                    }
                    else if ((variableType & VariableType.string_type)!=0)
                    { 
                        if(interactionSerialized.FindPropertyRelative("compareStringOperation").enumValueIndex == (int)Interaction.CompareStringOperation.areEqualCaseSensitive
                        ||interactionSerialized.FindPropertyRelative("compareStringOperation").enumValueIndex == (int)Interaction.CompareStringOperation.areEqualCaseInsensitive)
                            editorDescription = "property are equal to value";
                        else
                            editorDescription = "property contains value";
                    }
                    else
                        editorDescription = "value to compare";
                }

                rect.y += EditorGUIUtility.singleLineHeight;
                interactionSerialized.FindPropertyRelative(propertyTypeString + "_" + actionString + variableTypeString + "Value").boolValue = EditorGUI.Toggle(rect, actionString + " " + variableTypeString.ToLower() + " value", interactionSerialized.FindPropertyRelative(propertyTypeString + "_" + actionString + variableTypeString + "Value").boolValue);
                if (interactionSerialized.FindPropertyRelative(propertyTypeString + "_" + actionString + variableTypeString + "Value").boolValue)
                {
                    if (CheckArePropertyInteraction(PropertyObjectType.any, PropertyActionType.anyGet, interactionSerialized)
                        && ((variableType & (VariableType.integer_type | VariableType.float_type))!=0))
                    {
                        rect.y += EditorGUIUtility.singleLineHeight;
                        interactionSerialized.FindPropertyRelative("compareIntegerOrFloatOperation").enumValueIndex = EditorGUI.Popup(rect, interactionSerialized.FindPropertyRelative("compareIntegerOrFloatOperation").enumValueIndex, interactionSerialized.FindPropertyRelative("compareIntegerOrFloatOperation").enumDisplayNames);
                    }

                    if (CheckArePropertyInteraction(PropertyObjectType.any, PropertyActionType.anyGet , interactionSerialized)
                        && ((variableType & VariableType.string_type)!=0))
                    {
                        rect.y += EditorGUIUtility.singleLineHeight;
                        interactionSerialized.FindPropertyRelative("compareStringOperation").enumValueIndex = EditorGUI.Popup(rect, interactionSerialized.FindPropertyRelative("compareStringOperation").enumValueIndex, interactionSerialized.FindPropertyRelative("compareStringOperation").enumDisplayNames);
                    }

                    if (CheckArePropertyInteraction(PropertyObjectType.any, PropertyActionType.anySet, interactionSerialized)
                    && ((variableType & VariableType.boolean_type)!=0))
                    {
                        rect.y += EditorGUIUtility.singleLineHeight;
                        interactionSerialized.FindPropertyRelative("changeBooleanOperation").enumValueIndex = EditorGUI.Popup(rect, interactionSerialized.FindPropertyRelative("changeBooleanOperation").enumValueIndex, interactionSerialized.FindPropertyRelative("changeBooleanOperation").enumDisplayNames);
                    }

                    if (CheckArePropertyInteraction(PropertyObjectType.any, PropertyActionType.anySet, interactionSerialized)
                    && ((variableType & (VariableType.integer_type | VariableType.float_type)) != 0))
                    {
                        rect.y += EditorGUIUtility.singleLineHeight;
                        interactionSerialized.FindPropertyRelative("changeIntegerOrFloatOperation").enumValueIndex = EditorGUI.Popup(rect, interactionSerialized.FindPropertyRelative("changeIntegerOrFloatOperation").enumValueIndex, interactionSerialized.FindPropertyRelative("changeIntegerOrFloatOperation").enumDisplayNames);
                    }

                    if (CheckArePropertyInteraction(PropertyObjectType.any, PropertyActionType.anySet, interactionSerialized)
                        && ((variableType & VariableType.string_type) != 0))
                    {
                        rect.y += EditorGUIUtility.singleLineHeight;
                        interactionSerialized.FindPropertyRelative("changeStringOperation").enumValueIndex = EditorGUI.Popup(rect, interactionSerialized.FindPropertyRelative("changeStringOperation").enumValueIndex, interactionSerialized.FindPropertyRelative("changeStringOperation").enumDisplayNames);
                        if (interactionSerialized.FindPropertyRelative("changeStringOperation").enumValueIndex == (int)Interaction.ChangeStringOperation.replace)
                        { 
                            rect.y += EditorGUIUtility.singleLineHeight;
                            interactionSerialized.FindPropertyRelative("replaceValueToFind").stringValue = EditorGUI.TextField(rect, "value to find", interactionSerialized.FindPropertyRelative("replaceValueToFind").stringValue);
                        }
                    }

                    rect.y += EditorGUIUtility.singleLineHeight;
                    if (((variableType & VariableType.boolean_type) != 0) && interactionSerialized.FindPropertyRelative("changeBooleanOperation").enumValueIndex != (int)Interaction.ChangeBooleanOperation.toggle)
                        interactionSerialized.FindPropertyRelative(propertyTypeString + "_BooleanValue").boolValue = EditorGUI.Toggle(rect, editorDescription, interactionSerialized.FindPropertyRelative(propertyTypeString + "_BooleanValue").boolValue);
                    else if (((variableType & VariableType.integer_type)!=0))
                        interactionSerialized.FindPropertyRelative(propertyTypeString + "_IntegerValue").intValue = EditorGUI.IntField(rect, editorDescription, interactionSerialized.FindPropertyRelative(propertyTypeString + "_IntegerValue").intValue);
                    else if (((variableType & VariableType.string_type) != 0))
                        interactionSerialized.FindPropertyRelative(propertyTypeString + "_StringValue").stringValue = EditorGUI.TextField(rect, editorDescription, interactionSerialized.FindPropertyRelative(propertyTypeString + "_StringValue").stringValue);
                    else if (((variableType & VariableType.float_type) != 0))
                        interactionSerialized.FindPropertyRelative(propertyTypeString + "_FloatValue").floatValue = EditorGUI.FloatField(rect, editorDescription,interactionSerialized.FindPropertyRelative(propertyTypeString + "_FloatValue").floatValue);

                    if (((variableType & VariableType.boolean_type) != 0) && interactionSerialized.FindPropertyRelative("changeBooleanOperation").enumValueIndex == (int)Interaction.ChangeBooleanOperation.toggle)
                        rect.y -= EditorGUIUtility.singleLineHeight;

                    if (CheckArePropertyInteraction(PropertyObjectType.any, PropertyActionType.anyGet, interactionSerialized))
                    {
                        rect.y += EditorGUIUtility.singleLineHeight;
                        if (((variableType & VariableType.boolean_type)!=0))
                            interactionSerialized.FindPropertyRelative(propertyTypeString + "_defaultBooleanValue").boolValue = EditorGUI.Toggle(rect, "default value", interactionSerialized.FindPropertyRelative(propertyTypeString + "_defaultBooleanValue").boolValue);
                        else if (((variableType & VariableType.integer_type) != 0))
                            interactionSerialized.FindPropertyRelative(propertyTypeString + "_defaultIntegerValue").intValue = EditorGUI.IntField(rect, "default value", interactionSerialized.FindPropertyRelative(propertyTypeString + "_defaultIntegerValue").intValue);
                        else if (((variableType & VariableType.string_type) != 0))
                            interactionSerialized.FindPropertyRelative(propertyTypeString + "_defaultStringValue").stringValue = EditorGUI.TextField(rect, "default value", interactionSerialized.FindPropertyRelative(propertyTypeString + "_defaultStringValue").stringValue);
                        else if (((variableType & VariableType.float_type) != 0))
                            interactionSerialized.FindPropertyRelative(propertyTypeString + "_defaultFloatValue").floatValue = EditorGUI.FloatField(rect, "default value", interactionSerialized.FindPropertyRelative("global_defaultFloatValue").floatValue);
                    }
                }

                
            }
        }

    }

    public static void ShowGlobalPropertiesOnRect(System.Enum type, ref GlobalProperty[] properties, ref SerializedProperty properties_serialized, Rect? rect = null)
    {
        Rect newRect = new Rect();

        if (rect != null)
        {
            newRect = rect.Value;
            newRect.height = EditorGUIUtility.singleLineHeight;
        }
                       
        
        Settings settings = Resources.Load<Settings>("Settings/Settings");

        GUIStyle style = new GUIStyle();
        style.normal.textColor = Color.white;
        style.fontSize = 14;

        if (rect == null)
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("<b>Global Properties</b>", style);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }
        else 
        {
            GUI.Label(newRect, "<b>Global Properties</b>", style);
            newRect.y += EditorGUIUtility.singleLineHeight;
        }

        for (int i = 0; i < properties.Length; i++)
        {
            bool areType = true;

            for (int j = 0; j < settings.globalPropertiesConfig.Length; j++)
            {
                if (properties[i].globalID != -1 && settings.globalPropertiesConfig[j].ID == properties[i].globalID)
                {
                    properties[i].name = settings.globalPropertiesConfig[j].name;
                    if (!settings.globalPropertiesConfig[j].objectTypes.HasFlag(type))
                        areType = false;
                }
            }
            if (!areType)
                continue;

            properties[i].expandedInInspector = rect == null? EditorGUILayout.Foldout(properties[i].expandedInInspector, properties[i].name) : EditorGUI.Foldout(newRect, properties[i].expandedInInspector, properties[i].name);

            if (properties[i].expandedInInspector)
            {
                if(rect == null)
                    EditorGUILayout.BeginVertical("GroupBox");

                SerializedProperty element = properties_serialized.GetArrayElementAtIndex(i);

                ShowProperty(ref newRect, element, PropertyActionType.anyGlobal, VariableType.boolean_type , rect != null);

                ShowProperty(ref newRect, element, PropertyActionType.anyGlobal, VariableType.integer_type, rect != null);

                ShowProperty(ref newRect, element, PropertyActionType.anyGlobal, VariableType.string_type, rect != null);

                if(rect == null)
                    GUILayout.EndVertical();

                newRect.y += EditorGUIUtility.singleLineHeight;
            }
        }


        

        if (rect == null? GUILayout.Button("Edit global properties"): GUI.Button(newRect,"Edit global properties"))
        {
            Selection.objects = new UnityEngine.Object[] { settings };
            EditorGUIUtility.PingObject(settings);
        }


        var group = properties.GroupBy(vari => vari.name, (vari) => new { Count = vari.name.Count() });
        bool repeated = false;

        foreach (var vari in group)
        {
            if (vari.Count() > 1)
                repeated = true;

        }
        if (repeated)
        {
            newRect.y += EditorGUIUtility.singleLineHeight;

            GUIStyle styleRepeated = new GUIStyle();
            styleRepeated.normal.textColor = Color.red;
            styleRepeated.fontSize = 5;

            if (rect == null)
                GUILayout.Label("<b>There are more than one global property with the same name</b>", styleRepeated);
            else
            {
                styleRepeated.fontSize = 12;
                GUI.Label(newRect,"<b>There are more than one global property with the same name</b>", styleRepeated);
            }
        }

    }

    public static float GetAttempsContainerHeight(SerializedProperty serializedVerb, int indexC)
    {

        if (serializedVerb.GetArrayElementAtIndex(indexC).FindPropertyRelative("attempsContainer").FindPropertyRelative("expandedInInspector").boolValue)
        {
            float heightM = 7f * EditorGUIUtility.singleLineHeight;
            var attemps = serializedVerb.GetArrayElementAtIndex(indexC).FindPropertyRelative("attempsContainer").FindPropertyRelative("attemps");
            for (int i = 0; i < attemps.arraySize; i++)
            {
                heightM += GetAttempHeight(attemps.GetArrayElementAtIndex(i)) * 1.025f;

            }
            return heightM;
        }
        return EditorGUIUtility.singleLineHeight;
    }

    public static float GetAttempHeight(SerializedProperty attempSerialized)
    {
        if (attempSerialized.FindPropertyRelative("expandedInInspector").boolValue)
        {
            float height = 5 * EditorGUIUtility.singleLineHeight;
            for (int i = 0; i < attempSerialized.FindPropertyRelative("interactions").arraySize; i++)
            {
                height += GetInteractionHeight(attempSerialized.FindPropertyRelative("interactions").GetArrayElementAtIndex(i), i) * 1.025f;
            }
            return height;
        }

        return EditorGUIUtility.singleLineHeight;

    }


    public static float GetGlobalPropertiesHeight(SerializedProperty serializedGlobalProperties)
    {
        float height = 4 * EditorGUIUtility.singleLineHeight;

        height += EditorGUIUtility.singleLineHeight * 1 * serializedGlobalProperties.arraySize;

        for (int i = 0; i < serializedGlobalProperties.arraySize; i++)
        {
            if(serializedGlobalProperties.GetArrayElementAtIndex(i).FindPropertyRelative("expandedInInspector").boolValue)
            {
                
                if((serializedGlobalProperties.GetArrayElementAtIndex(i).FindPropertyRelative("config").FindPropertyRelative("variableTypes").enumValueFlag & (int)VariableType.boolean_type)!=0)
                    height += 2 * EditorGUIUtility.singleLineHeight;

                if ((serializedGlobalProperties.GetArrayElementAtIndex(i).FindPropertyRelative("config").FindPropertyRelative("variableTypes").enumValueFlag & (int)VariableType.string_type) != 0)
                    height += 2 * EditorGUIUtility.singleLineHeight;

                if ((serializedGlobalProperties.GetArrayElementAtIndex(i).FindPropertyRelative("config").FindPropertyRelative("variableTypes").enumValueFlag & (int)VariableType.integer_type) != 0)
                    height += 2 * EditorGUIUtility.singleLineHeight;
            }
        }


        return height;
    }

    public static float GetLocalPropertiesHeight(SerializedProperty serializedLocalProperties)
    {
        float height = 4 * EditorGUIUtility.singleLineHeight;

        height += EditorGUIUtility.singleLineHeight * 5 * serializedLocalProperties.arraySize;

        for (int i = 0; i < serializedLocalProperties.arraySize; i++)
        {

            if ((serializedLocalProperties.GetArrayElementAtIndex(i).FindPropertyRelative("variableTypes").enumValueFlag & (int)VariableType.boolean_type) != 0)
                height += 2 * EditorGUIUtility.singleLineHeight;

            if ((serializedLocalProperties.GetArrayElementAtIndex(i).FindPropertyRelative("variableTypes").enumValueFlag & (int)VariableType.string_type) != 0)
                height += 2 * EditorGUIUtility.singleLineHeight;

            if ((serializedLocalProperties.GetArrayElementAtIndex(i).FindPropertyRelative("variableTypes").enumValueFlag & (int)VariableType.integer_type) != 0)
                height += 2 * EditorGUIUtility.singleLineHeight;
        }


        return height;
    }

    public static float GetInteractionHeight(SerializedProperty interactionSerialized, int indexint = 0)
    {

        if (interactionSerialized.FindPropertyRelative("expandedInInspector").boolValue)
        {
            if (CheckArePropertyInteraction(PropertyObjectType.any, PropertyActionType.any, interactionSerialized))
            {
                float height = 4.25f;

                PNCPropertyInterface propertiesObject = null;

                if (CheckArePropertyInteraction(PropertyObjectType.inventory, PropertyActionType.any, interactionSerialized))
                {
                    InventoryList inventory = Resources.Load<InventoryList>("Inventory");
                    int specialIndex = interactionSerialized.FindPropertyRelative("inventorySelected").intValue;
                    if (specialIndex > 0)
                    {
                        for (int i = 0; i < inventory.items.Length; i++)
                        {
                            if(inventory.items[i].specialIndex == specialIndex)
                                propertiesObject = inventory.items[i];
                        }
                    }
                }
                else if (CheckArePropertyInteraction(PropertyObjectType.dialogOption, PropertyActionType.any, interactionSerialized))
                {
                    Dialog currentDialog = (Dialog) interactionSerialized.FindPropertyRelative("dialogSelected").objectReferenceValue;

                    if (currentDialog != null)
                    {
                        for (int i = 0; i < currentDialog.subDialogs.Count; i++)
                        {
                            if (currentDialog.subDialogs[i].index == interactionSerialized.FindPropertyRelative("subDialogIndex").intValue)
                            {
                                for (int j = 0; j < currentDialog.subDialogs[i].options.Count; j++)
                                {
                                    if (currentDialog.subDialogs[i].options[j].index == interactionSerialized.FindPropertyRelative("optionIndex").intValue)
                                    {
                                        propertiesObject = currentDialog.subDialogs[i].options[j];
                                    }
                                }
                            }
                        }
                    }
                    
                    height += 2;
                }
                else
                {
                    propertiesObject = (PNCPropertyInterface)interactionSerialized.FindPropertyRelative("propertyObject").objectReferenceValue;
                }

                if (propertiesObject != null)
                {
                    height += 1;
                    int index = -1;
                    int lenght = -1;
                    bool hasInteger = false;
                    bool hasBoolean = false;
                    bool hasString = false;
                    string propertyType = "";

                    if (CheckArePropertyInteraction(PropertyObjectType.any, PropertyActionType.anyGlobal, interactionSerialized))
                    { 
                        index = interactionSerialized.FindPropertyRelative("globalPropertySelected").intValue;
                        propertyType = "global";
                        lenght = propertiesObject.GlobalProperties.Length;
                        if (lenght > index)
                        { 
                            hasBoolean = (propertiesObject.GlobalProperties[index].config.variableTypes & VariableType.boolean_type)!=0;
                            hasInteger = (propertiesObject.GlobalProperties[index].config.variableTypes & VariableType.integer_type)!= 0;
                            hasString = (propertiesObject.GlobalProperties[index].config.variableTypes & VariableType.string_type)!= 0;
                        }
                    }
                    else if(CheckArePropertyInteraction(PropertyObjectType.any, PropertyActionType.anyLocal, interactionSerialized))
                    {
                        index = interactionSerialized.FindPropertyRelative("localPropertySelected").intValue;
                        propertyType = "local";
                        lenght = propertiesObject.LocalProperties.Length;
                        if (lenght > index)
                        { 
                            hasBoolean = (propertiesObject.LocalProperties[index].variableTypes & VariableType.boolean_type)!=0;
                            hasInteger = (propertiesObject.LocalProperties[index].variableTypes & VariableType.integer_type)!= 0;
                            hasString = (propertiesObject.LocalProperties[index].variableTypes & VariableType.string_type)!= 0;
                        }
                    }

                    if(lenght > index)
                    { 
                        if (hasBoolean)
                        {
                            height += 1;
                            if (interactionSerialized.FindPropertyRelative(propertyType + "_changeBooleanValue").boolValue
                                && CheckArePropertyInteraction(PropertyObjectType.any, PropertyActionType.anySet, interactionSerialized))
                            {
                                height += 1;
                                if(interactionSerialized.FindPropertyRelative("changeBooleanOperation").enumValueIndex != (int)Interaction.ChangeBooleanOperation.toggle)
                                    height += 1;
                            }
                        }
                        if (hasInteger)
                        {
                            height += 1;
                            if (interactionSerialized.FindPropertyRelative(propertyType + "_changeIntegerValue").boolValue
                                && CheckArePropertyInteraction(PropertyObjectType.any, PropertyActionType.anySet, interactionSerialized))
                                height += 2;
                        }
                        if (hasString)
                        {
                            height += 1;
                            if (interactionSerialized.FindPropertyRelative(propertyType + "_changeStringValue").boolValue
                                && CheckArePropertyInteraction(PropertyObjectType.any, PropertyActionType.anySet, interactionSerialized))
                            {
                                height += 2;
                                if (interactionSerialized.FindPropertyRelative("changeStringOperation").enumValueIndex == (int)Interaction.ChangeStringOperation.replace)
                                    height += 1;
                            }
                        }
                        if (CheckArePropertyInteraction(PropertyObjectType.any, PropertyActionType.anyGet, interactionSerialized))
                        {
                            if (interactionSerialized.FindPropertyRelative(propertyType + "_compareBooleanValue").boolValue && hasBoolean)
                                height += 2;
                            if (interactionSerialized.FindPropertyRelative(propertyType + "_compareIntegerValue").boolValue && hasInteger)
                                height += 3;
                            if (interactionSerialized.FindPropertyRelative(propertyType + "_compareStringValue").boolValue && hasString)
                                height += 3;
                            if ((interactionSerialized.FindPropertyRelative(propertyType + "_compareBooleanValue").boolValue && hasBoolean) ||
                                (interactionSerialized.FindPropertyRelative(propertyType + "_compareIntegerValue").boolValue && hasInteger) ||
                                (interactionSerialized.FindPropertyRelative(propertyType + "_compareStringValue").boolValue && hasString))
                                height += GetGoToLineHeight(interactionSerialized);
                        }
                    }
                    
                }
                return EditorGUIUtility.singleLineHeight * height;
            }


            if (interactionSerialized.FindPropertyRelative("type").enumValueIndex == (int)Interaction.InteractionType.character)
            {
                float height = 5.25f;
                if (interactionSerialized.FindPropertyRelative("characterAction").enumValueIndex == (int)Interaction.CharacterAction.say ||
                    interactionSerialized.FindPropertyRelative("characterAction").enumValueIndex == (int)Interaction.CharacterAction.sayWithScript)
                    height++;
                if (interactionSerialized.FindPropertyRelative("characterAction").enumValueIndex == (int)Interaction.CharacterAction.sayWithScript)
                {
                    height += 3;
                    for (int i = 0; i < interactionSerialized.FindPropertyRelative("customActionArguments").arraySize; i++)
                    {
                        height += GetCustomArgumentHeight(interactionSerialized, i) / EditorGUIUtility.singleLineHeight;
                    }
                }
                return EditorGUIUtility.singleLineHeight * height;
            }
            if (interactionSerialized.FindPropertyRelative("type").enumValueIndex == (int)Interaction.InteractionType.inventory)
            {
                float height = 4.25f;
                return EditorGUIUtility.singleLineHeight * height;
            }
            if (interactionSerialized.FindPropertyRelative("type").enumValueIndex == (int)Interaction.InteractionType.dialog)
            {
                float height = 4.25f;
                if (interactionSerialized.FindPropertyRelative("dialogAction").enumValueIndex == (int)Interaction.DialogAction.changeEntry)
                    height += 1;
                if (interactionSerialized.FindPropertyRelative("dialogAction").enumValueIndex == (int)Interaction.DialogAction.changeOptionState)
                    height += 3;
                if (interactionSerialized.FindPropertyRelative("dialogAction").enumValueIndex == (int)Interaction.DialogAction.changeOptionText)
                    height += 3;
                return EditorGUIUtility.singleLineHeight * height;
            }
            
            if (interactionSerialized.FindPropertyRelative("type").enumValueIndex == (int)Interaction.InteractionType.custom)
            { 
                float height = EditorGUIUtility.singleLineHeight * (12 + (interactionSerialized.FindPropertyRelative("action.m_PersistentCalls.m_Calls").arraySize * 2.5f));

                for (int i = 0; i < interactionSerialized.FindPropertyRelative("customActionArguments").arraySize; i++)
                {
                    height += GetCustomArgumentHeight(interactionSerialized, i);
                }
                
                if(interactionSerialized.FindPropertyRelative("customScriptAction").enumValueIndex == (int)Interaction.CustomScriptAction.customBoolean)
                    height += GetGoToLineHeight(interactionSerialized) * EditorGUIUtility.singleLineHeight;

                return height;
            }
        }
        return EditorGUIUtility.singleLineHeight;

    }

    private static float GetCustomArgumentHeight(SerializedProperty interactionSerialized, int indexCS)
    {
        bool expanded = interactionSerialized.FindPropertyRelative("customActionArguments").GetArrayElementAtIndex(indexCS).FindPropertyRelative("expandedInInspector").boolValue;
        if (expanded)
            return 3 * EditorGUIUtility.singleLineHeight;
        else
            return EditorGUIUtility.singleLineHeight;
    }

    public static float GetGoToLineHeight(SerializedProperty interactionSerialized)
    {
        float height = 2;
        if (interactionSerialized.FindPropertyRelative("OnCompareResultFalseAction").enumValueIndex == (int)Conditional.GetPropertyAction.GoToSpecificLine)
            height += 1;

        if (interactionSerialized.FindPropertyRelative("OnCompareResultTrueAction").enumValueIndex == (int)Conditional.GetPropertyAction.GoToSpecificLine)
            height += 1;

        return height;
    }

    public static void ShowLineToGo(ref Rect interactRect, SerializedProperty interactionSerialized, SerializedProperty attemps, int indexA)
    {
        interactionSerialized.FindPropertyRelative("OnCompareResultTrueAction").enumValueIndex = EditorGUI.Popup(interactRect, "action if result is true", interactionSerialized.FindPropertyRelative("OnCompareResultTrueAction").enumValueIndex, interactionSerialized.FindPropertyRelative("OnCompareResultTrueAction").enumDisplayNames);
        interactRect.y += EditorGUIUtility.singleLineHeight;
        if (interactionSerialized.FindPropertyRelative("OnCompareResultTrueAction").enumValueIndex == (int)Conditional.GetPropertyAction.GoToSpecificLine)
        {
            interactionSerialized.FindPropertyRelative("LineToGoOnTrueResult").intValue = EditorGUI.Popup(interactRect, "line to go", interactionSerialized.FindPropertyRelative("LineToGoOnTrueResult").intValue, PNCEditorUtils.GetInteractionsText(attemps.GetArrayElementAtIndex(indexA).FindPropertyRelative("interactions")));
            interactRect.y += EditorGUIUtility.singleLineHeight;
        }
        interactionSerialized.FindPropertyRelative("OnCompareResultFalseAction").enumValueIndex = EditorGUI.Popup(interactRect, "action if result is false", interactionSerialized.FindPropertyRelative("OnCompareResultFalseAction").enumValueIndex, interactionSerialized.FindPropertyRelative("OnCompareResultFalseAction").enumDisplayNames);
        interactRect.y += EditorGUIUtility.singleLineHeight;
        if (interactionSerialized.FindPropertyRelative("OnCompareResultFalseAction").enumValueIndex == (int)Conditional.GetPropertyAction.GoToSpecificLine)
        {
            interactionSerialized.FindPropertyRelative("LineToGoOnFalseResult").intValue = EditorGUI.Popup(interactRect, "line to go", interactionSerialized.FindPropertyRelative("LineToGoOnFalseResult").intValue, PNCEditorUtils.GetInteractionsText(attemps.GetArrayElementAtIndex(indexA).FindPropertyRelative("interactions")));
            interactRect.y += EditorGUIUtility.singleLineHeight;
        }
    }

    public static int GetInventoryWithPopUp(Rect rect, InventoryList inventory, int valueToSet, bool hasSceneObject, int inventoryIndexToExclude = -1)
    {
        List<string> content = new List<string>();
        if(hasSceneObject)
            content.Add("(Scene Object)");
        List<int> itemsIndexes = new List<int>();
        for (int i = 0; i < inventory.items.Length; i++)
        {
            if (inventoryIndexToExclude == -1 || inventory.items[i].specialIndex != inventoryIndexToExclude)
            { 
                content.Add(inventory.items[i].itemName);
                itemsIndexes.Add(inventory.items[i].specialIndex);
            }
        }

        int selected = 0;
        if (valueToSet != -1 && valueToSet != 0)
        {
            for (int i = 0; i < itemsIndexes.Count; i++)
            {
                if (itemsIndexes[i] == valueToSet)
                    selected = (hasSceneObject? i + 1 : i);
            }
        }
        rect.height = EditorGUIUtility.singleLineHeight;

        selected = EditorGUI.Popup(new Rect(rect.x + rect.width / 2.25f, rect.y, rect.width / 2, rect.height), "", selected, content.ToArray());

        if (hasSceneObject)
        {
            if (selected != 0)
                return itemsIndexes[selected - 1];
            else
            {
                return 0;
            }
        }
        else
        {
            return itemsIndexes[selected];
        }

    }


    public static int SetVerbWithPopUp(Rect rect,Verb[] verbs, int valueToSet)
    {
        List<string> verbsContent = new List<string>();
        for (int i = 0; i < verbs.Length; i++)
        {
            if (verbs[i].isLikeGive || verbs[i].isLikeUse)
                verbsContent.Add(verbs[i].name);
        }
        int verbSelected = 0;
        if (valueToSet >= 0)
        {
            for (int i = 0; i < verbs.Length; i++)
            {
                if (verbs[i].index == valueToSet)
                    verbSelected = i;
            }
        }
        verbSelected = EditorGUI.Popup(new Rect(rect.x + 7, rect.y, rect.width / 2.5f, EditorGUIUtility.singleLineHeight), "", verbSelected, verbsContent.ToArray());

        return verbs[verbSelected].index;
    }

    public static void DrawArrayWithAttempContainer(SerializedProperty containerProperty, int indexC, Rect rect, Dictionary<string, ReorderableList> attempsListDict, Dictionary<string, ReorderableList> interactionsListDict, Dictionary<string, ReorderableList> customScriptArgumentsDict, List<InteractionsAttemp> noSerializedAttemps, bool isInventoryItem = false, bool isDialogOption = false)
    {
        var attempContainer = containerProperty.GetArrayElementAtIndex(indexC).FindPropertyRelative("attempsContainer");
        var attemps = attempContainer.FindPropertyRelative("attemps");
        var verbRect = new Rect(rect);
        var verbExpanded = attempContainer.FindPropertyRelative("expandedInInspector");
        verbRect.x += 8;

        InventoryList inventory = null;

        //verbExpanded.boolValue = EditorGUI.Foldout(new Rect(verbRect.x, verbRect.y, verbRect.width, EditorGUIUtility.singleLineHeight), verbExpanded.boolValue, containerProperty.GetArrayElementAtIndex(indexC).FindPropertyRelative("name").stringValue);
        if(isInventoryItem)
            verbExpanded.boolValue = EditorGUI.Foldout(new Rect(verbRect.x, verbRect.y, verbRect.width, EditorGUIUtility.singleLineHeight), verbExpanded.boolValue, GUIContent.none);
        else if(isDialogOption)
            verbExpanded.boolValue = EditorGUI.Foldout(new Rect(verbRect.x, verbExpanded.boolValue?verbRect.y-EditorGUIUtility.singleLineHeight *2 : verbRect.y, verbRect.width, EditorGUIUtility.singleLineHeight), verbExpanded.boolValue, GUIContent.none);
        else
            verbExpanded.boolValue = EditorGUI.Foldout(new Rect(verbRect.x, verbRect.y, verbRect.width, EditorGUIUtility.singleLineHeight), verbExpanded.boolValue, containerProperty.GetArrayElementAtIndex(indexC).FindPropertyRelative("verb").FindPropertyRelative("name").stringValue);
                
        verbRect.y += EditorGUIUtility.singleLineHeight;

        if (verbExpanded.boolValue)
        {
            
            EditorGUI.PropertyField(new Rect(verbRect.x, verbRect.y, EditorGUIUtility.currentViewWidth, EditorGUIUtility.singleLineHeight), attempContainer.FindPropertyRelative("attempsIteration"));

            verbRect.y += EditorGUIUtility.singleLineHeight;

            var attempKey = attempContainer.propertyPath;

            if (!attempsListDict.ContainsKey(attempKey))
            {
                var attempsList = new ReorderableList(attemps.serializedObject, attemps, true, true, true, true)
                {
                    drawHeaderCallback = (rectA) =>
                    {
                        EditorGUI.LabelField(rectA, "attemps");
                    },
                    elementHeightCallback = (indexA) =>
                    {
                        //return PNCEditorUtils.GetAttempHeight(serializedObject.FindProperty("verbs").GetArrayElementAtIndex(indexV).FindPropertyRelative("attemps").GetArrayElementAtIndex(indexA), myTarget.verbs[indexV].attemps[indexA]);
                        return PNCEditorUtils.GetAttempHeight(attemps.GetArrayElementAtIndex(indexA));
                    },
                    drawElementCallback = (rectA, indexA, activeA, focusA) =>
                    {
                        var attemp = attempsListDict[attempKey].serializedProperty.GetArrayElementAtIndex(indexA);

                        var interactionKey = attemp.propertyPath;
                        var interactions = attemp.FindPropertyRelative("interactions");
                        var attempRect = new Rect(rectA);
                        var attempExpanded = attemp.FindPropertyRelative("expandedInInspector");

                        attempExpanded.boolValue = EditorGUI.Foldout(new Rect(attempRect.x, attempRect.y, attempRect.width, EditorGUIUtility.singleLineHeight), attempExpanded.boolValue, (indexA + 1).ToString() + "° attemp");
                        attempRect.y += EditorGUIUtility.singleLineHeight;

                        if (attempExpanded.boolValue)
                        {
                            if (!interactionsListDict.ContainsKey(interactionKey))
                            {
                                var interactionList = new ReorderableList(interactions.serializedObject, interactions, true, true, true, true)
                                {
                                    onMouseUpCallback = (ReorderableList list) =>
                                    {
                                        InteractionData data = new InteractionData();
                                        data.indexA = indexA;
                                        data.indexV = indexC;
                                        data.attemps = noSerializedAttemps;
                                        data.list = interactionsListDict[interactionKey];
                                        onMouse(data);
                                    },
                                    drawHeaderCallback = (rectI) =>
                                    {
                                        EditorGUI.LabelField(rectI, "interactions");
                                    },
                                    elementHeightCallback = (indexI) =>
                                    {
                                        var interactionSerialized = interactionsListDict[interactionKey].serializedProperty.GetArrayElementAtIndex(indexI);


                                        return PNCEditorUtils.GetInteractionHeight(interactionSerialized, indexI);
                                    }
                                    ,
                                    drawElementCallback = (rectI, indexI, activeI, focusI) =>
                                    {
                                        var interactionSerialized = interactionsListDict[interactionKey].serializedProperty.GetArrayElementAtIndex(indexI);
                                        var interactRect = new Rect(rectI);
                                        var interactExpanded = interactionSerialized.FindPropertyRelative("expandedInInspector");
                                        interactRect.height = EditorGUIUtility.singleLineHeight;

                                        interactExpanded.boolValue = EditorGUI.Foldout(interactRect, interactExpanded.boolValue, (indexI + 1).ToString() + "° interaction");
                                        interactRect.y += EditorGUIUtility.singleLineHeight;

                                        float propertyRectPosition = interactRect.y;
                                        PNCPropertyInterface propertyObject = null;

                                        if (interactExpanded.boolValue)
                                        {
                                            EditorGUI.PropertyField(interactRect, interactionSerialized.FindPropertyRelative("type"));

                                            if (CheckArePropertyInteraction(PropertyObjectType.any, PropertyActionType.any, interactionSerialized))
                                            {
                                                if (interactionSerialized.FindPropertyRelative("type").enumValueIndex == (int)Interaction.InteractionType.propertiesContainer)
                                                {
                                                    interactRect.y += EditorGUIUtility.singleLineHeight;
                                                    EditorGUI.PropertyField(interactRect, interactionSerialized.FindPropertyRelative("propertiesAction"));

                                                    interactRect.y += EditorGUIUtility.singleLineHeight;
                                                    EditorGUI.PropertyField(interactRect, interactionSerialized.FindPropertyRelative("propertyObject"));

                                                    if (interactionSerialized.FindPropertyRelative("propertyObject").objectReferenceValue != null && interactionSerialized.FindPropertyRelative("propertyObject").objectReferenceValue.GetType() != typeof(PNCPropertiesContainer))
                                                        interactionSerialized.FindPropertyRelative("propertyObject").objectReferenceValue = null;

                                                    propertyObject = (PNCPropertyInterface)interactionSerialized.FindPropertyRelative("propertyObject").objectReferenceValue;
                                                }
                                                else if (interactionSerialized.FindPropertyRelative("type").enumValueIndex == (int)Interaction.InteractionType.inventory)
                                                {
                                                    interactRect.y += EditorGUIUtility.singleLineHeight * 2;

                                                    if(inventory == null )
                                                        inventory = Resources.Load<InventoryList>("Inventory");

                                                    int specialIndex = interactionSerialized.FindPropertyRelative("inventorySelected").intValue;
                                                    if (specialIndex > 0)
                                                    {

                                                        for (int i = 0; i < inventory.items.Length; i++)
                                                        {
                                                            if (inventory.items[i].specialIndex == specialIndex)
                                                            {
                                                                propertyObject = inventory.items[i];
                                                            }

                                                        }
                                                    }
                                                }
                                                else if (interactionSerialized.FindPropertyRelative("type").enumValueIndex == (int)Interaction.InteractionType.dialog)
                                                {
                                                    interactRect.y += EditorGUIUtility.singleLineHeight * 4;

                                                    Dialog currentDialog = (Dialog)interactionSerialized.FindPropertyRelative("dialogSelected").objectReferenceValue;

                                                    if (currentDialog != null)
                                                    {
                                                        for (int i = 0; i < currentDialog.subDialogs.Count; i++)
                                                        {
                                                            if (currentDialog.subDialogs[i].index == interactionSerialized.FindPropertyRelative("subDialogIndex").intValue)
                                                            {
                                                                for (int j = 0; j < currentDialog.subDialogs[i].options.Count; j++)
                                                                {
                                                                    if (currentDialog.subDialogs[i].options[j].index == interactionSerialized.FindPropertyRelative("optionIndex").intValue)
                                                                    {
                                                                        propertyObject = currentDialog.subDialogs[i].options[j];
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                                else if (interactionSerialized.FindPropertyRelative("type").enumValueIndex == (int)Interaction.InteractionType.character)
                                                {
                                                    propertyRectPosition = interactRect.y;
                                                    interactRect.y += EditorGUIUtility.singleLineHeight * 2;

                                                    interactionSerialized.FindPropertyRelative("propertyObject").objectReferenceValue = interactionSerialized.FindPropertyRelative("character").objectReferenceValue;

                                                    propertyObject = (PNCPropertyInterface)interactionSerialized.FindPropertyRelative("propertyObject").objectReferenceValue;
                                                }


                                                    interactRect.y += EditorGUIUtility.singleLineHeight;
                                                if (CheckArePropertyInteraction(PropertyObjectType.any, PropertyActionType.anyGlobal,interactionSerialized))
                                                {
                                                    if (propertyObject != null)
                                                    {
                                                        GlobalProperty[] properties = propertyObject.GlobalProperties;
                                                        string[] content = new string[properties.Length];

                                                        for (int z = 0; z < properties.Length; z++)
                                                        {
                                                            content[z] = propertyObject.GlobalProperties[z].name;
                                                        }

                                                        interactionSerialized.FindPropertyRelative("globalPropertySelected").intValue = EditorGUI.Popup(interactRect, "Property", interactionSerialized.FindPropertyRelative("globalPropertySelected").intValue, content);

                                                        int index = interactionSerialized.FindPropertyRelative("globalPropertySelected").intValue;
                                                        if (propertyObject.GlobalProperties.Length > index)
                                                        {
                                                            PropertyInteraction(ref interactRect, interactionSerialized, propertyObject, PropertyActionType.anyGlobal, VariableType.boolean_type);

                                                            PropertyInteraction(ref interactRect, interactionSerialized, propertyObject, PropertyActionType.anyGlobal, VariableType.integer_type);

                                                            PropertyInteraction(ref interactRect, interactionSerialized, propertyObject, PropertyActionType.anyGlobal, VariableType.string_type);
                                                            
                                                            if (CheckArePropertyInteraction(PropertyObjectType.any, PropertyActionType.getGlobalProperty, interactionSerialized) &&
                                                                    ((interactionSerialized.FindPropertyRelative("global_compareBooleanValue").boolValue && (propertyObject.GlobalProperties[index].config.variableTypes & VariableType.boolean_type) != 0)||
                                                                    (interactionSerialized.FindPropertyRelative("global_compareIntegerValue").boolValue && (propertyObject.GlobalProperties[index].config.variableTypes & VariableType.integer_type) != 0)||
                                                                    (interactionSerialized.FindPropertyRelative("global_compareStringValue").boolValue && (propertyObject.GlobalProperties[index].config.variableTypes & VariableType.string_type)!=0)))
                                                            {
                                                                interactRect.y += EditorGUIUtility.singleLineHeight;
                                                                ShowLineToGo(ref interactRect, interactionSerialized, attemps, indexA);
                                                            }
                                                        }

                                                    }
                                                }
                                                else if (CheckArePropertyInteraction(PropertyObjectType.any, PropertyActionType.anyLocal, interactionSerialized))
                                                {
                                                    if (propertyObject != null)
                                                    {
                                                        LocalProperty[] properties = propertyObject.LocalProperties;
                                                        string[] content = new string[properties.Length];

                                                        for (int z = 0; z < properties.Length; z++)
                                                        {
                                                            content[z] = propertyObject.LocalProperties[z].name;
                                                        }

                                                        interactionSerialized.FindPropertyRelative("localPropertySelected").intValue = EditorGUI.Popup(interactRect, "Property", interactionSerialized.FindPropertyRelative("localPropertySelected").intValue, content);
                                                        int index = interactionSerialized.FindPropertyRelative("localPropertySelected").intValue;
                                                        if (propertyObject.LocalProperties.Length > index)
                                                        {
                                                            PropertyInteraction(ref interactRect, interactionSerialized, propertyObject, PropertyActionType.anyLocal, VariableType.boolean_type);

                                                            PropertyInteraction(ref interactRect, interactionSerialized, propertyObject, PropertyActionType.anyLocal, VariableType.integer_type);

                                                            PropertyInteraction(ref interactRect, interactionSerialized, propertyObject, PropertyActionType.anyLocal, VariableType.string_type);

                                                            if (CheckArePropertyInteraction(PropertyObjectType.any, PropertyActionType.getLocalProperty, interactionSerialized) &&
                                                                    ((interactionSerialized.FindPropertyRelative("local_compareBooleanValue").boolValue && (propertyObject.LocalProperties[index].variableTypes & VariableType.boolean_type) != 0)||
                                                                    (interactionSerialized.FindPropertyRelative("local_compareIntegerValue").boolValue && (propertyObject.LocalProperties[index].variableTypes & VariableType.integer_type)!=0) ||
                                                                    (interactionSerialized.FindPropertyRelative("local_compareStringValue").boolValue && (propertyObject.LocalProperties[index].variableTypes & VariableType.string_type)!=0)))
                                                            {
                                                                interactRect.y += EditorGUIUtility.singleLineHeight;
                                                                ShowLineToGo(ref interactRect, interactionSerialized, attemps, indexA);
                                                            }
                                                        }

                                                    }
                                                }
                                            }
                                            if (interactionSerialized.FindPropertyRelative("type").enumValueIndex == (int)Interaction.InteractionType.character)
                                            {
                                                if (CheckArePropertyInteraction(PropertyObjectType.character, PropertyActionType.any, interactionSerialized))
                                                    interactRect.y = propertyRectPosition;

                                                interactRect.y = interactRect.y + EditorGUIUtility.singleLineHeight;

                                                EditorGUI.PropertyField(interactRect, interactionSerialized.FindPropertyRelative("character"));
                                                interactRect.y += EditorGUIUtility.singleLineHeight;
                                                EditorGUI.PropertyField(interactRect, interactionSerialized.FindPropertyRelative("characterAction"));
                                                interactRect.y += EditorGUIUtility.singleLineHeight;
                                                if (interactionSerialized.FindPropertyRelative("characterAction").enumValueIndex == (int)Interaction.CharacterAction.say)
                                                {
                                                    EditorGUI.PropertyField(interactRect, interactionSerialized.FindPropertyRelative("WhatToSay"));
                                                    interactRect.y += EditorGUIUtility.singleLineHeight;
                                                    EditorGUI.PropertyField(interactRect, interactionSerialized.FindPropertyRelative("CanSkip"));
                                                }
                                                if (interactionSerialized.FindPropertyRelative("characterAction").enumValueIndex == (int)Interaction.CharacterAction.sayWithScript)
                                                {
                                                    EditorGUI.PropertyField(interactRect, interactionSerialized.FindPropertyRelative("SayScript"));
                                                    if (!(interactionSerialized.FindPropertyRelative("SayScript").objectReferenceValue is SayScript))
                                                    {
                                                        interactionSerialized.FindPropertyRelative("SayScript").objectReferenceValue = null;
                                                    }
                                                    interactRect.y += EditorGUIUtility.singleLineHeight;
                                                    EditorGUI.PropertyField(interactRect, interactionSerialized.FindPropertyRelative("CanSkip"));

                                                    ShowCustomArguments(ref interactRect, interactionSerialized, customScriptArgumentsDict);
                                                }
                                                else if (interactionSerialized.FindPropertyRelative("characterAction").enumValueIndex == (int)Interaction.CharacterAction.walk ||
                                                interactionSerialized.FindPropertyRelative("characterAction").enumValueIndex == (int)Interaction.CharacterAction.walkStraight)
                                                    EditorGUI.PropertyField(interactRect, interactionSerialized.FindPropertyRelative("WhereToWalk"));
                                            }
                                            else if (interactionSerialized.FindPropertyRelative("type").enumValueIndex == (int)Interaction.InteractionType.dialog)
                                            {
                                                if (CheckArePropertyInteraction(PropertyObjectType.dialogOption, PropertyActionType.any, interactionSerialized))
                                                    interactRect.y = propertyRectPosition;

                                                interactRect.y += EditorGUIUtility.singleLineHeight;
                                                EditorGUI.PropertyField(interactRect, interactionSerialized.FindPropertyRelative("dialogAction"));
                                                interactRect.y += EditorGUIUtility.singleLineHeight;
                                                if (interactionSerialized.FindPropertyRelative("dialogAction").enumValueIndex != (int)Interaction.DialogAction.endCurrentDialog)
                                                    EditorGUI.PropertyField(interactRect, interactionSerialized.FindPropertyRelative("dialogSelected"));

                                                Dialog currentDialog = ((Dialog)interactionSerialized.FindPropertyRelative("dialogSelected").objectReferenceValue);

                                                if (currentDialog != null || interactionSerialized.FindPropertyRelative("dialogAction").enumValueIndex == (int)Interaction.DialogAction.endCurrentDialog)
                                                {
                                                    if (interactionSerialized.FindPropertyRelative("dialogAction").enumValueIndex == (int)Interaction.DialogAction.changeEntry  
                                                    || interactionSerialized.FindPropertyRelative("dialogAction").enumValueIndex == (int)Interaction.DialogAction.changeOptionState 
                                                    || interactionSerialized.FindPropertyRelative("dialogAction").enumValueIndex == (int)Interaction.DialogAction.changeOptionText
                                                    || CheckArePropertyInteraction(PropertyObjectType.dialogOption, PropertyActionType.any, interactionSerialized))
                                                    {
                                                        interactRect.y += EditorGUIUtility.singleLineHeight;
                                                        string[] subdialogsTexts = new string[currentDialog.subDialogs.Count];
                                                        int[] subdialogsIndexs = new int[currentDialog.subDialogs.Count];
                                                        for (int i = 0; i < currentDialog.subDialogs.Count; i++)
                                                        {
                                                            subdialogsTexts[i] = currentDialog.subDialogs[i].text;
                                                            subdialogsIndexs[i] = currentDialog.subDialogs[i].index;
                                                        }
                                                        if (interactionSerialized.FindPropertyRelative("dialogAction").enumValueIndex == (int)Interaction.DialogAction.changeEntry)
                                                            interactionSerialized.FindPropertyRelative("newDialogEntry").intValue = EditorGUI.IntPopup(interactRect, "new subdialog entry", interactionSerialized.FindPropertyRelative("newDialogEntry").intValue, subdialogsTexts, subdialogsIndexs);

                                                        if (interactionSerialized.FindPropertyRelative("dialogAction").enumValueIndex == (int)Interaction.DialogAction.changeOptionState
                                                            || interactionSerialized.FindPropertyRelative("dialogAction").enumValueIndex == (int)Interaction.DialogAction.changeOptionText
                                                            || CheckArePropertyInteraction(PropertyObjectType.dialogOption, PropertyActionType.any, interactionSerialized))
                                                        {
                                                            Dictionary<int, int> subDialogIndexAndArrayIndex = new Dictionary<int, int>();
                                                            for (int i = 0; i < currentDialog.subDialogs.Count; i++)
                                                            {
                                                                subDialogIndexAndArrayIndex.Add(currentDialog.subDialogs[i].index, i);
                                                            }
                                                            interactionSerialized.FindPropertyRelative("subDialogIndex").intValue = EditorGUI.IntPopup(interactRect, "subdialog", interactionSerialized.FindPropertyRelative("subDialogIndex").intValue, subdialogsTexts, subdialogsIndexs);
                                                            if (interactionSerialized.FindPropertyRelative("subDialogIndex").intValue > 0)
                                                            {
                                                                int currentSubDialogArrayIndex = subDialogIndexAndArrayIndex[interactionSerialized.FindPropertyRelative("subDialogIndex").intValue];
                                                                interactRect.y += EditorGUIUtility.singleLineHeight;
                                                                string[] optionsTexts = new string[currentDialog.subDialogs[currentSubDialogArrayIndex].options.Count];
                                                                int[] optionsIndexs = new int[currentDialog.subDialogs[currentSubDialogArrayIndex].options.Count];
                                                                for (int i = 0; i < currentDialog.subDialogs[currentSubDialogArrayIndex].options.Count; i++)
                                                                {
                                                                    optionsTexts[i] = currentDialog.subDialogs[currentSubDialogArrayIndex].options[i].initialText;
                                                                    optionsIndexs[i] = currentDialog.subDialogs[currentSubDialogArrayIndex].options[i].index;
                                                                }
                                                                interactionSerialized.FindPropertyRelative("optionIndex").intValue = EditorGUI.IntPopup(interactRect, "option", interactionSerialized.FindPropertyRelative("optionIndex").intValue, optionsTexts, optionsIndexs);
                                                                if (interactionSerialized.FindPropertyRelative("optionIndex").intValue > 0)
                                                                {
                                                                    interactRect.y += EditorGUIUtility.singleLineHeight;
                                                                    if (interactionSerialized.FindPropertyRelative("dialogAction").enumValueIndex == (int)Interaction.DialogAction.changeOptionState)
                                                                        EditorGUI.PropertyField(interactRect, interactionSerialized.FindPropertyRelative("newOptionState"));
                                                                    if (interactionSerialized.FindPropertyRelative("dialogAction").enumValueIndex == (int)Interaction.DialogAction.changeOptionText)
                                                                        EditorGUI.PropertyField(interactRect, interactionSerialized.FindPropertyRelative("newOptionText"));
                                                                }
                                                            }
                                                        }
                                                    }

                                                    
                                                }
                                            }
                                            else if (interactionSerialized.FindPropertyRelative("type").enumValueIndex == (int)Interaction.InteractionType.inventory)
                                            {
                                                if (CheckArePropertyInteraction(PropertyObjectType.inventory, PropertyActionType.any, interactionSerialized))
                                                    interactRect.y = propertyRectPosition;

                                                interactRect.y = interactRect.y + EditorGUIUtility.singleLineHeight;

                                                EditorGUI.PropertyField(interactRect, interactionSerialized.FindPropertyRelative("inventoryAction"));
                                                interactRect.y += EditorGUIUtility.singleLineHeight;
                                                InventoryList inventory = Resources.Load<InventoryList>("Inventory");
                                                EditorGUI.LabelField(interactRect, "Inventory");
                                                interactionSerialized.FindPropertyRelative("inventorySelected").intValue = GetInventoryWithPopUp(interactRect, inventory, interactionSerialized.FindPropertyRelative("inventorySelected").intValue, false);
                                            }
                                            else if (interactionSerialized.FindPropertyRelative("type").enumValueIndex == (int)Interaction.InteractionType.custom)
                                            {
                                                interactRect.y += EditorGUIUtility.singleLineHeight;

                                                EditorGUI.PropertyField(interactRect, interactionSerialized.FindPropertyRelative("customScriptAction"));

                                                interactRect.y += EditorGUIUtility.singleLineHeight;

                                                ShowCustomArguments(ref interactRect, interactionSerialized, customScriptArgumentsDict);


                                                if (interactionSerialized.FindPropertyRelative("customScriptAction").enumValueIndex == (int)Interaction.CustomScriptAction.customBoolean)
                                                {
                                                    ShowLineToGo(ref interactRect, interactionSerialized, attemps, indexA);
                                                }
                                                EditorGUI.PropertyField(interactRect, interactionSerialized.FindPropertyRelative("action"));
                                            }

                                        }

                                    }
                                };
                                interactionsListDict.Add(interactionKey, interactionList);
                            }
                            interactionsListDict[interactionKey].DoList(attempRect);
                        }
                    },
                    onAddCallback = (list) => 
                    {
                        ReorderableList.defaultBehaviours.DoAddButton(list);
                        attemps.serializedObject.ApplyModifiedProperties();
                    }
                };

                attempsListDict.Add(attempKey, attempsList);
            }
            attempsListDict[attempKey].DoList(verbRect);
        }
    }

    private static void ShowCustomArguments(ref Rect interactRect, SerializedProperty interactionSerialized, Dictionary<string, ReorderableList> customScriptArgumentsDict)
    {
        string customScriptArgumentsKey = interactionSerialized.propertyPath;

        if (!customScriptArgumentsDict.ContainsKey(customScriptArgumentsKey))
        {
            ReorderableList customScriptArgumentsList = new ReorderableList(interactionSerialized.FindPropertyRelative("customActionArguments").serializedObject, interactionSerialized.FindPropertyRelative("customActionArguments"), true, true, true, true)
            {
                drawHeaderCallback = (rectCS) =>
                {
                    EditorGUI.LabelField(rectCS, "arguments");
                },
                drawElementCallback = (rectCS, indexCS, activeCS, focusCS) =>
                {
                    rectCS.height = EditorGUIUtility.singleLineHeight;
                    SerializedProperty expandedProperty = interactionSerialized.FindPropertyRelative("customActionArguments").GetArrayElementAtIndex(indexCS).FindPropertyRelative("expandedInInspector");
                    expandedProperty.boolValue = EditorGUI.Foldout(new Rect(rectCS.x + 7, rectCS.y, rectCS.width, rectCS.height), expandedProperty.boolValue, GUIContent.none);
                    EditorGUI.PropertyField(new Rect(rectCS.x + 7, rectCS.y, rectCS.width, rectCS.height), interactionSerialized.FindPropertyRelative("customActionArguments").GetArrayElementAtIndex(indexCS).FindPropertyRelative("name"), GUIContent.none);
                    if (expandedProperty.boolValue)
                    {
                        rectCS.y += EditorGUIUtility.singleLineHeight;
                        EditorGUI.PropertyField(rectCS, interactionSerialized.FindPropertyRelative("customActionArguments").GetArrayElementAtIndex(indexCS).FindPropertyRelative("type"));
                        rectCS.y += EditorGUIUtility.singleLineHeight;
                        int type = interactionSerialized.FindPropertyRelative("type").GetArrayElementAtIndex(indexCS).FindPropertyRelative("type").enumValueFlag;
                        if (type == (int)VariableType.string_type)
                        {
                            EditorGUI.PropertyField(rectCS, interactionSerialized.FindPropertyRelative("customActionArguments").GetArrayElementAtIndex(indexCS).FindPropertyRelative("stringArgument"));
                        }
                        else if (type == (int)VariableType.boolean_type)
                        {
                            EditorGUI.PropertyField(rectCS, interactionSerialized.FindPropertyRelative("customActionArguments").GetArrayElementAtIndex(indexCS).FindPropertyRelative("boolArgument"));
                        }
                        else if (type == (int)VariableType.integer_type)
                        {
                            EditorGUI.PropertyField(rectCS, interactionSerialized.FindPropertyRelative("customActionArguments").GetArrayElementAtIndex(indexCS).FindPropertyRelative("intArgument"));
                        }
                        else if (type == (int)VariableType.object_type)
                        {
                            EditorGUI.PropertyField(rectCS, interactionSerialized.FindPropertyRelative("customActionArguments").GetArrayElementAtIndex(indexCS).FindPropertyRelative("objectArgument"));
                        }
                    }
                },
                elementHeightCallback = (indexCS) =>
                {
                    return GetCustomArgumentHeight(interactionSerialized, indexCS);
                }


            };

            customScriptArgumentsDict.Add(customScriptArgumentsKey, customScriptArgumentsList);
        }

        customScriptArgumentsDict[customScriptArgumentsKey].DoList(interactRect);

        interactRect.y += EditorGUIUtility.singleLineHeight * 4;

        for (int i = 0; i < interactionSerialized.FindPropertyRelative("customActionArguments").arraySize; i++)
        {
            interactRect.y += GetCustomArgumentHeight(interactionSerialized, i);
        }
    }

    [System.Serializable]
    public class InteractionData
    {
        public int indexV;
        public int indexA;
        public ReorderableList list;
        public SerializedProperty property;
        public List<InteractionsAttemp> attemps;
    }

    static Interaction copiedInteraction;
    
    private static void onMouse(InteractionData interactioncopy)
    {
        GenericMenu menu = new GenericMenu();

        if (interactioncopy.indexA < interactioncopy.attemps.Count)
        {
            menu.AddItem(new GUIContent("Copy interaction"), false, Copy, interactioncopy);
            if(copiedInteraction != null)
                menu.AddItem(new GUIContent("Paste interaction"), false, Paste, interactioncopy);
        }
        else
            menu.AddDisabledItem(new GUIContent("Cant copy right now"));
        menu.AddItem(new GUIContent("Cancel"), false, Cancel);

        menu.ShowAsContext();
    }

    private static void Cancel()
    {
    }


    private static void Copy(object interaction)
    {
        copiedInteraction = ((InteractionData)interaction).attemps[((InteractionData)interaction).indexA].interactions[((InteractionData)interaction).list.index];
    }

    private static void Paste(object interaction)
    {
        if (copiedInteraction == null) return;

        copiedInteraction.Copy(((InteractionData)interaction).attemps[((InteractionData)interaction).indexA].interactions[((InteractionData)interaction).list.index]);
        ((InteractionData)interaction).list.serializedProperty.serializedObject.Update();
    }

    public static void CheckVerbs(ref List<VerbInteractions> verbs)
    {
        Settings settings = Resources.Load<Settings>("Settings/Settings");

        List<VerbInteractions> interactionsTempList = new List<VerbInteractions>();
        List<int> interactionsAdded = new List<int>();
        for (int i = 0; i < verbs.Count; i++)
        {
            for (int j = 0; j < settings.verbs.Length; j++)
            {
                if (verbs[i].verb.index == settings.verbs[j].index)
                {
                    VerbInteractions tempVerb = new VerbInteractions();
                    tempVerb.verb = new Verb();
                    tempVerb.verb.name = settings.verbs[j].name;
                    tempVerb.verb.isLikeUse = settings.verbs[j].isLikeUse;
                    tempVerb.verb.isLikeGive = settings.verbs[j].isLikeGive;
                    tempVerb.verb.index = settings.verbs[j].index;
                    tempVerb.attempsContainer = verbs[i].attempsContainer;
                    if (!interactionsAdded.Contains(settings.verbs[j].index))
                    {
                        interactionsAdded.Add(settings.verbs[j].index);
                        interactionsTempList.Add(tempVerb);
                    }
                    break;
                }
            }
        }

        verbs = interactionsTempList;
    }


    public static void OnAddVerbDropdown(ReorderableList list, List<VerbInteractions> verbs, SerializedObject serializedObject) 
    {
        Settings settings = Resources.Load<Settings>("Settings/Settings");


        var menu = new GenericMenu();

        List<int> indexs = new List<int>();
        for (int i = 0; i < settings.verbs.Length; i++)
        {
            bool founded = false;
            for (int j = 0; j < verbs.Count; j++)
            {
                if (settings.verbs[i].index == verbs[j].verb.index)
                {
                    founded = true;
                    break;
                }
            }
            if (!founded)
                indexs.Add(i);
        }

        for (int i = 0; i < indexs.Count; i++)
        {
            menu.AddItem(new GUIContent(settings.verbs[indexs[i]].name), false, OnAddNewVerb, new NewVerbVariableParam() { index = indexs[i], list = list, serializedObject = serializedObject});
        }

        menu.ShowAsContext();
    }

    private static void OnAddNewVerb(object var)
    {
        Settings settings = Resources.Load<Settings>("Settings/Settings");

        NewVerbVariableParam variable = (NewVerbVariableParam)var;
        ReorderableList verbsList = variable.list;
        int elementIndex = verbsList.serializedProperty.arraySize;
        int settingsVerbIndex = variable.index;

        verbsList.serializedProperty.arraySize++;
        verbsList.index = elementIndex;
        var element = verbsList.serializedProperty.GetArrayElementAtIndex(elementIndex);
        element.FindPropertyRelative("verb").FindPropertyRelative("name").stringValue = settings.verbs[settingsVerbIndex].name;
        element.FindPropertyRelative("verb").FindPropertyRelative("isLikeUse").boolValue = settings.verbs[settingsVerbIndex].isLikeUse;
        element.FindPropertyRelative("verb").FindPropertyRelative("isLikeGive").boolValue = settings.verbs[settingsVerbIndex].isLikeGive;
        element.FindPropertyRelative("verb").FindPropertyRelative("index").intValue = settings.verbs[settingsVerbIndex].index;

        variable.serializedObject.ApplyModifiedProperties();
    }

    public static string[] GetInteractionsText(SerializedProperty interactions)
    {
        string[] texts = new string[interactions.arraySize];
        for (int i = 0; i < interactions.arraySize; i++)
        {
            texts[i] = (i + 1) + " " + interactions.GetArrayElementAtIndex(i).FindPropertyRelative("type").enumNames[interactions.GetArrayElementAtIndex(i).FindPropertyRelative("type").enumValueIndex].ToString();
            if (interactions.GetArrayElementAtIndex(i).FindPropertyRelative("type").enumValueIndex == (int)Interaction.InteractionType.character)
            {
                texts[i] += " " + interactions.GetArrayElementAtIndex(i).FindPropertyRelative("characterAction").enumNames[interactions.GetArrayElementAtIndex(i).FindPropertyRelative("characterAction").enumValueIndex].ToString();
            }
            else if (interactions.GetArrayElementAtIndex(i).FindPropertyRelative("type").enumValueIndex == (int)Interaction.InteractionType.propertiesContainer)
            {
                texts[i] += " " + interactions.GetArrayElementAtIndex(i).FindPropertyRelative("propertiesAction").enumNames[interactions.GetArrayElementAtIndex(i).FindPropertyRelative("propertiesAction").enumValueIndex].ToString();
            }
            else if (interactions.GetArrayElementAtIndex(i).FindPropertyRelative("type").enumValueIndex == (int)Interaction.InteractionType.inventory)
            {
                texts[i] += " " + interactions.GetArrayElementAtIndex(i).FindPropertyRelative("inventoryAction").enumNames[interactions.GetArrayElementAtIndex(i).FindPropertyRelative("inventoryAction").enumValueIndex].ToString();
            }
            else if (interactions.GetArrayElementAtIndex(i).FindPropertyRelative("type").enumValueIndex == (int)Interaction.InteractionType.dialog)
            {
                texts[i] += " " + interactions.GetArrayElementAtIndex(i).FindPropertyRelative("dialogAction").enumNames[interactions.GetArrayElementAtIndex(i).FindPropertyRelative("dialogAction").enumValueIndex].ToString();
            }
        }

        return texts;

    }

    private static bool CheckArePropertyInteraction(PropertyObjectType objectType, PropertyActionType proptype, SerializedProperty interactionSerialized)
    {
        bool areObj = false;
        bool areProp = false;

        
        if ((proptype & PropertyActionType.getGlobalProperty)!=0)
        {
            if ((interactionSerialized.FindPropertyRelative("type").enumValueIndex == (int)Interaction.InteractionType.propertiesContainer &&
                     interactionSerialized.FindPropertyRelative("propertiesAction").enumValueIndex == (int)Interaction.PropertiesContainerAction.getGlobalProperty)
               || (interactionSerialized.FindPropertyRelative("type").enumValueIndex == (int)Interaction.InteractionType.inventory &&
                     interactionSerialized.FindPropertyRelative("inventoryAction").enumValueIndex == (int)Interaction.InventoryAction.getGlobalProperty)
               || (interactionSerialized.FindPropertyRelative("type").enumValueIndex == (int)Interaction.InteractionType.character &&
                     interactionSerialized.FindPropertyRelative("characterAction").enumValueIndex == (int)Interaction.CharacterAction.getGlobalProperty)
                || (interactionSerialized.FindPropertyRelative("type").enumValueIndex == (int)Interaction.InteractionType.dialog &&
                     interactionSerialized.FindPropertyRelative("dialogAction").enumValueIndex == (int)Interaction.DialogAction.getGlobalProperty))
                areProp = true;
        }
        if ((proptype & PropertyActionType.getLocalProperty)!=0)
        {
            if ((interactionSerialized.FindPropertyRelative("type").enumValueIndex == (int)Interaction.InteractionType.propertiesContainer &&
                     interactionSerialized.FindPropertyRelative("propertiesAction").enumValueIndex == (int)Interaction.PropertiesContainerAction.getLocalProperty)
               || (interactionSerialized.FindPropertyRelative("type").enumValueIndex == (int)Interaction.InteractionType.inventory &&
                     interactionSerialized.FindPropertyRelative("inventoryAction").enumValueIndex == (int)Interaction.InventoryAction.getLocalProperty)
               || (interactionSerialized.FindPropertyRelative("type").enumValueIndex == (int)Interaction.InteractionType.character &&
                     interactionSerialized.FindPropertyRelative("characterAction").enumValueIndex == (int)Interaction.CharacterAction.getLocalProperty)
                || (interactionSerialized.FindPropertyRelative("type").enumValueIndex == (int)Interaction.InteractionType.dialog &&
                     interactionSerialized.FindPropertyRelative("dialogAction").enumValueIndex == (int)Interaction.DialogAction.getLocalProperty))
                areProp = true;
        }
        if ((proptype & PropertyActionType.setGlobalProperty)!=0)
        {
            if ((interactionSerialized.FindPropertyRelative("type").enumValueIndex == (int)Interaction.InteractionType.propertiesContainer &&
                     interactionSerialized.FindPropertyRelative("propertiesAction").enumValueIndex == (int)Interaction.PropertiesContainerAction.setGlobalProperty)
               || (interactionSerialized.FindPropertyRelative("type").enumValueIndex == (int)Interaction.InteractionType.inventory &&
                     interactionSerialized.FindPropertyRelative("inventoryAction").enumValueIndex == (int)Interaction.InventoryAction.setGlobalProperty)
               || (interactionSerialized.FindPropertyRelative("type").enumValueIndex == (int)Interaction.InteractionType.character &&
                     interactionSerialized.FindPropertyRelative("characterAction").enumValueIndex == (int)Interaction.CharacterAction.setGlobalProperty)
                || (interactionSerialized.FindPropertyRelative("type").enumValueIndex == (int)Interaction.InteractionType.dialog &&
                     interactionSerialized.FindPropertyRelative("dialogAction").enumValueIndex == (int)Interaction.DialogAction.setGlobalProperty))
                areProp = true;
        }
        if ((proptype & PropertyActionType.setLocalProperty)!=0)
        {
            if ((interactionSerialized.FindPropertyRelative("type").enumValueIndex == (int)Interaction.InteractionType.propertiesContainer &&
                     interactionSerialized.FindPropertyRelative("propertiesAction").enumValueIndex == (int)Interaction.PropertiesContainerAction.setLocalProperty)
               || (interactionSerialized.FindPropertyRelative("type").enumValueIndex == (int)Interaction.InteractionType.inventory &&
                     interactionSerialized.FindPropertyRelative("inventoryAction").enumValueIndex == (int)Interaction.InventoryAction.setLocalProperty)
               || (interactionSerialized.FindPropertyRelative("type").enumValueIndex == (int)Interaction.InteractionType.character &&
                     interactionSerialized.FindPropertyRelative("characterAction").enumValueIndex == (int)Interaction.CharacterAction.setLocalProperty)
                || (interactionSerialized.FindPropertyRelative("type").enumValueIndex == (int)Interaction.InteractionType.dialog &&
                     interactionSerialized.FindPropertyRelative("dialogAction").enumValueIndex == (int)Interaction.DialogAction.setLocalProperty))
                areProp = true;
        }

        if ((objectType & PropertyObjectType.propertiesContainer)!=0)
        {
            if (interactionSerialized.FindPropertyRelative("type").enumValueIndex == (int)Interaction.InteractionType.propertiesContainer)
                areObj = true;
        }
        if ((objectType & PropertyObjectType.inventory)!=0)
        {
            if (interactionSerialized.FindPropertyRelative("type").enumValueIndex == (int)Interaction.InteractionType.inventory)
                areObj = true;
        }
        if ((objectType & PropertyObjectType.character) != 0)
        {
            if (interactionSerialized.FindPropertyRelative("type").enumValueIndex == (int)Interaction.InteractionType.character)
                areObj = true;
        }
        if ((objectType & PropertyObjectType.dialogOption) != 0)
        {
            if (interactionSerialized.FindPropertyRelative("type").enumValueIndex == (int)Interaction.InteractionType.dialog)
                areObj = true;
        }

        return areObj && areProp;
    }

}
