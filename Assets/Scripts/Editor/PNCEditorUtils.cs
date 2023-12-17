using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
public static class PNCEditorUtils
{

    private enum PropertyObjectType {
        room_object,
        character,
        dialog_option,
        inventory,
        properties_container,
        any
    }

    private enum PropertyActionType {
        set_global_property,
        get_global_property,
        set_local_property,
        get_local_property,
        any_local,
        any_global,
        any_set,
        any_get,
        any
    }

    private enum PropertyVariableType {
        boolean_type,
        float_type,
        integer_type,
        string_type
    }

    private enum PropertyType {
        local,
        global
    }

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

                    if (settings.globalPropertiesConfig[i].object_type.HasFlag(type))
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
                if (settings.globalPropertiesConfig[i].object_type.HasFlag(type))
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
                              
                ShowProperty(ref rect, element, PropertyType.local,  PropertyVariableType.boolean_type, true);

                ShowProperty(ref rect, element, PropertyType.local, PropertyVariableType.integer_type, true);

                ShowProperty(ref rect, element, PropertyType.local, PropertyVariableType.string_type, true);

            },
            elementHeightCallback = (int index) =>
            {
                SerializedProperty element = property.GetArrayElementAtIndex(index);

                float height = 4f;
                if (element.FindPropertyRelative("hasBoolean").boolValue)
                    height += 2;
                if (element.FindPropertyRelative("hasInteger").boolValue)
                    height += 2;
                if (element.FindPropertyRelative("hasString").boolValue)
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


    private static void ShowProperty(ref Rect rect, SerializedProperty element, PropertyType type, PropertyVariableType variable, bool useRect = false)
    {
        Rect getRect(Rect rectParam)
        {
            if (useRect == false) return EditorGUILayout.GetControlRect();
            else return rectParam;
        }

        bool hasProperty;
        string typeString;
        if (type == PropertyType.global)
        {
            if(variable == PropertyVariableType.boolean_type)
                hasProperty = element.FindPropertyRelative("config").FindPropertyRelative("hasBoolean").boolValue;
            else if(variable == PropertyVariableType.integer_type)
                hasProperty = element.FindPropertyRelative("config").FindPropertyRelative("hasInteger").boolValue;
            else if (variable == PropertyVariableType.string_type)
                hasProperty = element.FindPropertyRelative("config").FindPropertyRelative("hasString").boolValue;
            else
                hasProperty = element.FindPropertyRelative("config").FindPropertyRelative("hasFloat").boolValue;
        }
        else
        {
            rect.y += EditorGUIUtility.singleLineHeight;
            if (variable == PropertyVariableType.boolean_type)
            {
                element.FindPropertyRelative("hasBoolean").boolValue = EditorGUI.Toggle(getRect(rect), "has boolean value:", element.FindPropertyRelative("hasBoolean").boolValue);
                hasProperty = element.FindPropertyRelative("hasBoolean").boolValue;
            }
            else if (variable == PropertyVariableType.integer_type)
            {
                element.FindPropertyRelative("hasInteger").boolValue = EditorGUI.Toggle(getRect(rect), "has integer value:", element.FindPropertyRelative("hasInteger").boolValue);
                hasProperty = element.FindPropertyRelative("hasInteger").boolValue;
            }
            else if (variable == PropertyVariableType.string_type)
            {
                element.FindPropertyRelative("hasString").boolValue = EditorGUI.Toggle(getRect(rect), "has string value:", element.FindPropertyRelative("hasString").boolValue);
                hasProperty = element.FindPropertyRelative("hasString").boolValue;
            }
            else
            {
                element.FindPropertyRelative("hasFloat").boolValue = EditorGUI.Toggle(getRect(rect), "has float value:", element.FindPropertyRelative("hasFloat").boolValue);
                hasProperty = element.FindPropertyRelative("hasFloat").boolValue;
            }
        }



        if (variable == PropertyVariableType.boolean_type)
            typeString = "boolean";
        else if (variable == PropertyVariableType.integer_type)
            typeString = "integer";
        else if (variable == PropertyVariableType.string_type)
            typeString = "string";
        else
            typeString = "float";




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
                if(variable == PropertyVariableType.boolean_type)
                    element.FindPropertyRelative("boolean").boolValue =
                        EditorGUI.Toggle(getRect(rect), "boolean value:", element.FindPropertyRelative("boolean").boolValue);
                else if (variable == PropertyVariableType.integer_type)
                    element.FindPropertyRelative("integer").intValue =
                        EditorGUI.IntField(getRect(rect), "integer value:", element.FindPropertyRelative("integer").intValue);
                else if (variable == PropertyVariableType.string_type)
                    element.FindPropertyRelative("String").stringValue =
                        EditorGUI.TextField(getRect(rect), "string value:", element.FindPropertyRelative("String").stringValue);
                else
                    element.FindPropertyRelative("boolean").boolValue =
                        EditorGUI.Toggle(getRect(rect), "float value:", element.FindPropertyRelative("boolean").boolValue);

                rect.y += EditorGUIUtility.singleLineHeight;
                if (GUI.Button(getRect(rect), "set " + typeString + " default value"))
                {
                    element.FindPropertyRelative(typeString + "Default").boolValue = true;
                }
            }
        }
    
    }

    private static void PropertyInteraction(ref Rect rect, SerializedProperty interactionSerialized, PNCPropertyInterface propertyObject, PropertyActionType propertyActionType, PropertyVariableType variableType)
    {
        string propertyTypeString = "";
        if (propertyActionType == PropertyActionType.any_global)
        {
            propertyTypeString = "global";
        }
        else
        {
            propertyTypeString = "local";
        }

        int index = interactionSerialized.FindPropertyRelative(propertyTypeString + "PropertySelected").intValue;
        bool hasProperty = false;
        string variableTypeString = "";
        if (propertyActionType == PropertyActionType.any_global)
        {
            if (variableType == PropertyVariableType.boolean_type)
            {
                hasProperty = propertyObject.GlobalProperties[index].config.hasBoolean;
            }
            else if (variableType == PropertyVariableType.integer_type)
            {
                hasProperty = propertyObject.GlobalProperties[index].config.hasInteger;
            }
            else if (variableType == PropertyVariableType.string_type)
            {
                hasProperty = propertyObject.GlobalProperties[index].config.hasString;
            }
            else if (variableType == PropertyVariableType.float_type)
            {
                //hasProperty = propertyObject.GlobalProperties[index].config.hasFloat;
            }
        }
        else if (propertyActionType == PropertyActionType.any_local)
        {
            if (variableType == PropertyVariableType.boolean_type)
            {
                hasProperty = propertyObject.LocalProperties[index].hasBoolean;
            }
            else if (variableType == PropertyVariableType.integer_type)
            {
                hasProperty = propertyObject.LocalProperties[index].hasInteger;
            }
            else if (variableType == PropertyVariableType.string_type)
            {
                hasProperty = propertyObject.LocalProperties[index].hasString;
            }
            else if (variableType == PropertyVariableType.float_type)
            {
                //hasProperty = propertyObject.LocalProperties[index].hasFloat;
            }
        }

        if (variableType == PropertyVariableType.boolean_type)
            variableTypeString = "Boolean";
        else if (variableType == PropertyVariableType.integer_type)
            variableTypeString = "Integer";
        else if (variableType == PropertyVariableType.string_type)
            variableTypeString = "String";
        else
            variableTypeString = "Float";


        if (hasProperty)
        {
            PropertyActionType propertyActionTypeSet;
            PropertyActionType propertyActionTypeGet;
            if (propertyActionType == PropertyActionType.any_global)
            {
                propertyActionTypeSet = PropertyActionType.set_global_property;
                propertyActionTypeGet = PropertyActionType.get_global_property;
            }
            else
            {
                propertyActionTypeSet = PropertyActionType.set_local_property;
                propertyActionTypeGet = PropertyActionType.get_local_property;
            }

            if (CheckArePropertyInteraction(PropertyObjectType.any, propertyActionTypeSet, interactionSerialized) || CheckArePropertyInteraction(PropertyObjectType.any, propertyActionTypeGet, interactionSerialized))
            {
                string actionString = "";
                string editorDescription = "";

                if (CheckArePropertyInteraction(PropertyObjectType.any, propertyActionTypeSet, interactionSerialized))
                {
                    actionString = "change";
                    if (variableType == PropertyVariableType.integer_type || variableType == PropertyVariableType.float_type)
                    {
                        if (interactionSerialized.FindPropertyRelative("changeIntegerOrFloatOperation").enumValueIndex == (int)Interaction.ChangeIntegerOrFloatOperation.add)
                            editorDescription = "value to add";
                        if (interactionSerialized.FindPropertyRelative("changeIntegerOrFloatOperation").enumValueIndex == (int)Interaction.ChangeIntegerOrFloatOperation.subtract)
                            editorDescription = "value to subtract";
                        if (interactionSerialized.FindPropertyRelative("changeIntegerOrFloatOperation").enumValueIndex == (int)Interaction.ChangeIntegerOrFloatOperation.set)
                            editorDescription = "value to set";
                    }
                    else
                        editorDescription = "value to set";
                }
                else if (CheckArePropertyInteraction(PropertyObjectType.any, propertyActionTypeGet, interactionSerialized))
                {
                    actionString = "compare";
                    if (variableType == PropertyVariableType.integer_type || variableType == PropertyVariableType.float_type)
                    {
                        if (interactionSerialized.FindPropertyRelative("compareIntegerOrFloatOperation").enumValueIndex == (int)Interaction.CompareIntegerOrFloatOperation.areEqual)
                            editorDescription = "value to compare";
                        if (interactionSerialized.FindPropertyRelative("compareIntegerOrFloatOperation").enumValueIndex == (int)Interaction.CompareIntegerOrFloatOperation.isGreaterThan)
                            editorDescription = "property is greater than value";
                        if (interactionSerialized.FindPropertyRelative("compareIntegerOrFloatOperation").enumValueIndex == (int)Interaction.CompareIntegerOrFloatOperation.isLessThan)
                            editorDescription = "property is less than value";
                    }
                    else
                        editorDescription = "value to compare";
                }

                rect.y += EditorGUIUtility.singleLineHeight;
                interactionSerialized.FindPropertyRelative(propertyTypeString + "_" + actionString + variableTypeString + "Value").boolValue = EditorGUI.Toggle(rect, actionString + " " + variableTypeString.ToLower() + " value", interactionSerialized.FindPropertyRelative(propertyTypeString + "_" + actionString + variableTypeString + "Value").boolValue);
                if (interactionSerialized.FindPropertyRelative(propertyTypeString + "_" + actionString + variableTypeString + "Value").boolValue)
                {
                    if (CheckArePropertyInteraction(PropertyObjectType.any, propertyActionTypeGet, interactionSerialized)
                        && (variableType == PropertyVariableType.integer_type || (variableType == PropertyVariableType.float_type)))
                    {
                        rect.y += EditorGUIUtility.singleLineHeight;
                        interactionSerialized.FindPropertyRelative("compareIntegerOrFloatOperation").enumValueIndex = EditorGUI.Popup(rect, interactionSerialized.FindPropertyRelative("compareIntegerOrFloatOperation").enumValueIndex, interactionSerialized.FindPropertyRelative("compareIntegerOrFloatOperation").enumDisplayNames);
                    }

                    if (CheckArePropertyInteraction(PropertyObjectType.any, propertyActionTypeSet, interactionSerialized)
                    && (variableType == PropertyVariableType.integer_type || (variableType == PropertyVariableType.float_type)))
                    {
                        rect.y += EditorGUIUtility.singleLineHeight;
                        interactionSerialized.FindPropertyRelative("changeIntegerOrFloatOperation").enumValueIndex = EditorGUI.Popup(rect, interactionSerialized.FindPropertyRelative("changeIntegerOrFloatOperation").enumValueIndex, interactionSerialized.FindPropertyRelative("changeIntegerOrFloatOperation").enumDisplayNames);
                    }

                    rect.y += EditorGUIUtility.singleLineHeight;
                    if (variableType == PropertyVariableType.boolean_type)
                        interactionSerialized.FindPropertyRelative(propertyTypeString + "_BooleanValue").boolValue = EditorGUI.Toggle(rect, editorDescription, interactionSerialized.FindPropertyRelative(propertyTypeString + "_BooleanValue").boolValue);
                    else if (variableType == PropertyVariableType.integer_type)
                        interactionSerialized.FindPropertyRelative(propertyTypeString + "_IntegerValue").intValue = EditorGUI.IntField(rect, editorDescription, interactionSerialized.FindPropertyRelative(propertyTypeString + "_IntegerValue").intValue);
                    else if (variableType == PropertyVariableType.string_type)
                        interactionSerialized.FindPropertyRelative(propertyTypeString + "_StringValue").stringValue = EditorGUI.TextField(rect, editorDescription, interactionSerialized.FindPropertyRelative(propertyTypeString + "_StringValue").stringValue);
                    else
                        interactionSerialized.FindPropertyRelative(propertyTypeString + "_FloatValue").floatValue = EditorGUI.FloatField(rect, editorDescription,interactionSerialized.FindPropertyRelative(propertyTypeString + "_FloatValue").floatValue);


                    if (CheckArePropertyInteraction(PropertyObjectType.any, propertyActionTypeGet, interactionSerialized))
                    {
                        rect.y += EditorGUIUtility.singleLineHeight;
                        if (variableType == PropertyVariableType.boolean_type)
                            interactionSerialized.FindPropertyRelative(propertyTypeString + "_defaultBooleanValue").boolValue = EditorGUI.Toggle(rect, "default value", interactionSerialized.FindPropertyRelative(propertyTypeString + "_defaultBooleanValue").boolValue);
                        else if (variableType == PropertyVariableType.integer_type)
                            interactionSerialized.FindPropertyRelative(propertyTypeString + "_defaultIntegerValue").intValue = EditorGUI.IntField(rect, "default value", interactionSerialized.FindPropertyRelative(propertyTypeString + "_defaultIntegerValue").intValue);
                        else if (variableType == PropertyVariableType.string_type)
                            interactionSerialized.FindPropertyRelative(propertyTypeString + "_defaultStringValue").stringValue = EditorGUI.TextField(rect, "default value", interactionSerialized.FindPropertyRelative(propertyTypeString + "_defaultStringValue").stringValue);
                        else
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
                    if (!settings.globalPropertiesConfig[j].object_type.HasFlag(type))
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

                ShowProperty(ref newRect, element, PropertyType.global, PropertyVariableType.boolean_type , rect != null);

                ShowProperty(ref newRect, element, PropertyType.global, PropertyVariableType.integer_type, rect != null);

                ShowProperty(ref newRect, element, PropertyType.global, PropertyVariableType.string_type, rect != null);

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
                height += GetInteractionHeight(attempSerialized.FindPropertyRelative("interactions").GetArrayElementAtIndex(i)) * 1.025f;
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
                
                if (serializedGlobalProperties.GetArrayElementAtIndex(i).FindPropertyRelative("config").FindPropertyRelative("hasBoolean").boolValue)
                    height += 2 * EditorGUIUtility.singleLineHeight;

                if (serializedGlobalProperties.GetArrayElementAtIndex(i).FindPropertyRelative("config").FindPropertyRelative("hasString").boolValue)
                    height += 2 * EditorGUIUtility.singleLineHeight;

                if (serializedGlobalProperties.GetArrayElementAtIndex(i).FindPropertyRelative("config").FindPropertyRelative("hasInteger").boolValue)
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

            if (serializedLocalProperties.GetArrayElementAtIndex(i).FindPropertyRelative("hasBoolean").boolValue)
                height += 2 * EditorGUIUtility.singleLineHeight;

            if (serializedLocalProperties.GetArrayElementAtIndex(i).FindPropertyRelative("hasString").boolValue)
                height += 2 * EditorGUIUtility.singleLineHeight;

            if (serializedLocalProperties.GetArrayElementAtIndex(i).FindPropertyRelative("hasInteger").boolValue)
                height += 2 * EditorGUIUtility.singleLineHeight;
        }


        return height;
    }

    public static float GetInteractionHeight(SerializedProperty interactionSerialized)
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
                else if (CheckArePropertyInteraction(PropertyObjectType.dialog_option, PropertyActionType.any, interactionSerialized))
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
                    if (CheckArePropertyInteraction(PropertyObjectType.any, PropertyActionType.any_global, interactionSerialized))
                    {
                        int index = interactionSerialized.FindPropertyRelative("globalPropertySelected").intValue;

                        if (propertiesObject.GlobalProperties.Length > index)
                        {
                            if (propertiesObject.GlobalProperties[index].config.hasBoolean)
                            {
                                height += 1;
                                if (interactionSerialized.FindPropertyRelative("global_changeBooleanValue").boolValue
                                    && CheckArePropertyInteraction(PropertyObjectType.any, PropertyActionType.set_global_property, interactionSerialized))
                                    height += 1;
                            }
                            if (propertiesObject.GlobalProperties[index].config.hasInteger)
                            {
                                height += 1;
                                if (interactionSerialized.FindPropertyRelative("global_changeIntegerValue").boolValue
                                    && CheckArePropertyInteraction(PropertyObjectType.any, PropertyActionType.set_global_property, interactionSerialized))
                                    height += 1;
                            }
                            if (propertiesObject.GlobalProperties[index].config.hasString)
                            {
                                height += 1;
                                if (interactionSerialized.FindPropertyRelative("global_changeStringValue").boolValue
                                    && CheckArePropertyInteraction(PropertyObjectType.any, PropertyActionType.set_global_property, interactionSerialized))
                                    height += 1;
                            }
                            if (CheckArePropertyInteraction(PropertyObjectType.any, PropertyActionType.get_global_property, interactionSerialized))
                            {
                                if (interactionSerialized.FindPropertyRelative("global_compareBooleanValue").boolValue && propertiesObject.GlobalProperties[index].config.hasBoolean)
                                    height += 2;
                                if (interactionSerialized.FindPropertyRelative("global_compareIntegerValue").boolValue && propertiesObject.GlobalProperties[index].config.hasInteger)
                                    height += 2;
                                if (interactionSerialized.FindPropertyRelative("global_compareStringValue").boolValue && propertiesObject.GlobalProperties[index].config.hasString)
                                    height += 2;
                                if ((interactionSerialized.FindPropertyRelative("global_compareBooleanValue").boolValue && propertiesObject.GlobalProperties[index].config.hasBoolean) ||
                                    (interactionSerialized.FindPropertyRelative("global_compareIntegerValue").boolValue && propertiesObject.GlobalProperties[index].config.hasInteger) ||
                                    (interactionSerialized.FindPropertyRelative("global_compareStringValue").boolValue && propertiesObject.GlobalProperties[index].config.hasString))
                                    height += GetGoToLineHeight(interactionSerialized);
                            }
                        }
                    }
                    if (CheckArePropertyInteraction(PropertyObjectType.any, PropertyActionType.any_local, interactionSerialized))
                    {
                        int index = interactionSerialized.FindPropertyRelative("localPropertySelected").intValue;
                        if (propertiesObject.LocalProperties.Length > index)
                        {
                            if (propertiesObject.LocalProperties[index].hasBoolean)
                            {
                                height += 1;
                                if (interactionSerialized.FindPropertyRelative("local_changeBooleanValue").boolValue
                                    && CheckArePropertyInteraction(PropertyObjectType.any, PropertyActionType.set_local_property, interactionSerialized))
                                    height += 1;
                            }
                            if (propertiesObject.LocalProperties[index].hasInteger)
                            {
                                height += 1;
                                if (interactionSerialized.FindPropertyRelative("local_changeIntegerValue").boolValue
                                    && CheckArePropertyInteraction(PropertyObjectType.any, PropertyActionType.set_local_property, interactionSerialized))
                                    height += 2;
                            }
                            if (propertiesObject.LocalProperties[index].hasString)
                            {
                                height += 1;
                                if (interactionSerialized.FindPropertyRelative("local_changeStringValue").boolValue
                                    && CheckArePropertyInteraction(PropertyObjectType.any, PropertyActionType.set_local_property, interactionSerialized))
                                    height += 1;
                            }
                            if (CheckArePropertyInteraction( PropertyObjectType.any, PropertyActionType.get_local_property, interactionSerialized))
                            {
                                if (interactionSerialized.FindPropertyRelative("local_compareBooleanValue").boolValue && propertiesObject.LocalProperties[index].hasBoolean)
                                    height += 2;
                                if (interactionSerialized.FindPropertyRelative("local_compareIntegerValue").boolValue && propertiesObject.LocalProperties[index].hasInteger)
                                    height += 3;
                                if (interactionSerialized.FindPropertyRelative("local_compareStringValue").boolValue && propertiesObject.LocalProperties[index].hasString)
                                    height += 2;
                                if ((interactionSerialized.FindPropertyRelative("local_compareBooleanValue").boolValue && propertiesObject.LocalProperties[index].hasBoolean) ||
                                    (interactionSerialized.FindPropertyRelative("local_compareIntegerValue").boolValue && propertiesObject.LocalProperties[index].hasInteger) ||
                                    (interactionSerialized.FindPropertyRelative("local_compareStringValue").boolValue && propertiesObject.LocalProperties[index].hasString))
                                    height += GetGoToLineHeight(interactionSerialized);
                            }
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

                                        return PNCEditorUtils.GetInteractionHeight(interactionSerialized);
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
                                                if (interactionSerialized.FindPropertyRelative("type").enumValueIndex == (int)Interaction.InteractionType.properties_container)
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
                                                if (CheckArePropertyInteraction(PropertyObjectType.any, PropertyActionType.any_global,interactionSerialized))
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
                                                            PropertyInteraction(ref interactRect, interactionSerialized, propertyObject, PropertyActionType.any_global, PropertyVariableType.boolean_type);

                                                            PropertyInteraction(ref interactRect, interactionSerialized, propertyObject, PropertyActionType.any_global, PropertyVariableType.integer_type);

                                                            PropertyInteraction(ref interactRect, interactionSerialized, propertyObject, PropertyActionType.any_global, PropertyVariableType.string_type);
                                                            
                                                            if (CheckArePropertyInteraction(PropertyObjectType.any, PropertyActionType.get_global_property, interactionSerialized) &&
                                                                    ((interactionSerialized.FindPropertyRelative("global_compareBooleanValue").boolValue && propertyObject.GlobalProperties[index].config.hasBoolean)||
                                                                    (interactionSerialized.FindPropertyRelative("global_compareIntegerValue").boolValue && propertyObject.GlobalProperties[index].config.hasInteger)||
                                                                    (interactionSerialized.FindPropertyRelative("global_compareStringValue").boolValue && propertyObject.GlobalProperties[index].config.hasString)))
                                                            {
                                                                interactRect.y += EditorGUIUtility.singleLineHeight;
                                                                ShowLineToGo(ref interactRect, interactionSerialized, attemps, indexA);
                                                            }
                                                        }

                                                    }
                                                }
                                                else if (CheckArePropertyInteraction(PropertyObjectType.any, PropertyActionType.any_local, interactionSerialized))
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
                                                            PropertyInteraction(ref interactRect, interactionSerialized, propertyObject, PropertyActionType.any_local, PropertyVariableType.boolean_type);

                                                            PropertyInteraction(ref interactRect, interactionSerialized, propertyObject, PropertyActionType.any_local, PropertyVariableType.integer_type);

                                                            PropertyInteraction(ref interactRect, interactionSerialized, propertyObject, PropertyActionType.any_local, PropertyVariableType.string_type);

                                                            if (CheckArePropertyInteraction(PropertyObjectType.any, PropertyActionType.get_local_property, interactionSerialized) &&
                                                                    ((interactionSerialized.FindPropertyRelative("local_compareBooleanValue").boolValue && propertyObject.LocalProperties[index].hasBoolean)||
                                                                    (interactionSerialized.FindPropertyRelative("local_compareIntegerValue").boolValue && propertyObject.LocalProperties[index].hasInteger) ||
                                                                    (interactionSerialized.FindPropertyRelative("local_compareStringValue").boolValue && propertyObject.LocalProperties[index].hasString)))
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
                                                if (CheckArePropertyInteraction(PropertyObjectType.dialog_option, PropertyActionType.any, interactionSerialized))
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
                                                    || CheckArePropertyInteraction(PropertyObjectType.dialog_option, PropertyActionType.any, interactionSerialized))
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
                                                            || CheckArePropertyInteraction(PropertyObjectType.dialog_option, PropertyActionType.any, interactionSerialized))
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
                        int type = interactionSerialized.FindPropertyRelative("customActionArguments").GetArrayElementAtIndex(indexCS).FindPropertyRelative("type").enumValueIndex;
                        if (type == (int)CustomArgument.ArgumentType.String)
                        {
                            EditorGUI.PropertyField(rectCS, interactionSerialized.FindPropertyRelative("customActionArguments").GetArrayElementAtIndex(indexCS).FindPropertyRelative("stringArgument"));
                        }
                        else if (type == (int)CustomArgument.ArgumentType.Boolean)
                        {
                            EditorGUI.PropertyField(rectCS, interactionSerialized.FindPropertyRelative("customActionArguments").GetArrayElementAtIndex(indexCS).FindPropertyRelative("boolArgument"));
                        }
                        else if (type == (int)CustomArgument.ArgumentType.Integer)
                        {
                            EditorGUI.PropertyField(rectCS, interactionSerialized.FindPropertyRelative("customActionArguments").GetArrayElementAtIndex(indexCS).FindPropertyRelative("intArgument"));
                        }
                        else if (type == (int)CustomArgument.ArgumentType.Object)
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
            if (interactions.GetArrayElementAtIndex(i).FindPropertyRelative("type").enumValueIndex == (int)Interaction.InteractionType.properties_container)
            {
                texts[i] += " " + interactions.GetArrayElementAtIndex(i).FindPropertyRelative("propertiesAction").enumNames[interactions.GetArrayElementAtIndex(i).FindPropertyRelative("propertiesAction").enumValueIndex].ToString();
            }
            if (interactions.GetArrayElementAtIndex(i).FindPropertyRelative("type").enumValueIndex == (int)Interaction.InteractionType.inventory)
            {
                texts[i] += " " + interactions.GetArrayElementAtIndex(i).FindPropertyRelative("inventoryAction").enumNames[interactions.GetArrayElementAtIndex(i).FindPropertyRelative("inventoryAction").enumValueIndex].ToString();
            }
        }

        return texts;

    }

    private static bool CheckArePropertyInteraction(PropertyObjectType type, PropertyActionType proptype, SerializedProperty interactionSerialized)
    {
        bool areType = false;
        bool areProp = false;

        if (proptype == PropertyActionType.any)
        {
            if ((interactionSerialized.FindPropertyRelative("type").enumValueIndex == (int)Interaction.InteractionType.properties_container &&
                     (interactionSerialized.FindPropertyRelative("propertiesAction").enumValueIndex == (int)Interaction.PropertiesContainerAction.getGlobalProperty
                   || interactionSerialized.FindPropertyRelative("propertiesAction").enumValueIndex == (int)Interaction.PropertiesContainerAction.getLocalProperty
                   || interactionSerialized.FindPropertyRelative("propertiesAction").enumValueIndex == (int)Interaction.PropertiesContainerAction.setGlobalProperty
                   || interactionSerialized.FindPropertyRelative("propertiesAction").enumValueIndex == (int)Interaction.PropertiesContainerAction.setLocalProperty))
            || (interactionSerialized.FindPropertyRelative("type").enumValueIndex == (int)Interaction.InteractionType.inventory &&
                     (interactionSerialized.FindPropertyRelative("inventoryAction").enumValueIndex == (int)Interaction.InventoryAction.getGlobalProperty
                   || interactionSerialized.FindPropertyRelative("inventoryAction").enumValueIndex == (int)Interaction.InventoryAction.getLocalProperty
                   || interactionSerialized.FindPropertyRelative("inventoryAction").enumValueIndex == (int)Interaction.InventoryAction.setGlobalProperty
                   || interactionSerialized.FindPropertyRelative("inventoryAction").enumValueIndex == (int)Interaction.InventoryAction.setLocalProperty))
            || (interactionSerialized.FindPropertyRelative("type").enumValueIndex == (int)Interaction.InteractionType.character &&
                     (interactionSerialized.FindPropertyRelative("characterAction").enumValueIndex == (int)Interaction.CharacterAction.getGlobalProperty
                   || interactionSerialized.FindPropertyRelative("characterAction").enumValueIndex == (int)Interaction.CharacterAction.getLocalProperty
                   || interactionSerialized.FindPropertyRelative("characterAction").enumValueIndex == (int)Interaction.CharacterAction.setGlobalProperty
                   || interactionSerialized.FindPropertyRelative("characterAction").enumValueIndex == (int)Interaction.CharacterAction.setLocalProperty))
            || (interactionSerialized.FindPropertyRelative("type").enumValueIndex == (int)Interaction.InteractionType.dialog &&
                     (interactionSerialized.FindPropertyRelative("dialogAction").enumValueIndex == (int)Interaction.DialogAction.getGlobalProperty
                   || interactionSerialized.FindPropertyRelative("dialogAction").enumValueIndex == (int)Interaction.DialogAction.getLocalProperty
                   || interactionSerialized.FindPropertyRelative("dialogAction").enumValueIndex == (int)Interaction.DialogAction.setGlobalProperty
                   || interactionSerialized.FindPropertyRelative("dialogAction").enumValueIndex == (int)Interaction.DialogAction.setLocalProperty)))
                areProp = true;
        }
        if (proptype == PropertyActionType.get_global_property || proptype == PropertyActionType.any_global || proptype == PropertyActionType.any_get)
        {
            if ((interactionSerialized.FindPropertyRelative("type").enumValueIndex == (int)Interaction.InteractionType.properties_container &&
                     interactionSerialized.FindPropertyRelative("propertiesAction").enumValueIndex == (int)Interaction.PropertiesContainerAction.getGlobalProperty)
               || (interactionSerialized.FindPropertyRelative("type").enumValueIndex == (int)Interaction.InteractionType.inventory &&
                     interactionSerialized.FindPropertyRelative("inventoryAction").enumValueIndex == (int)Interaction.InventoryAction.getGlobalProperty)
               || (interactionSerialized.FindPropertyRelative("type").enumValueIndex == (int)Interaction.InteractionType.character &&
                     interactionSerialized.FindPropertyRelative("characterAction").enumValueIndex == (int)Interaction.CharacterAction.getGlobalProperty)
                || (interactionSerialized.FindPropertyRelative("type").enumValueIndex == (int)Interaction.InteractionType.dialog &&
                     interactionSerialized.FindPropertyRelative("dialogAction").enumValueIndex == (int)Interaction.DialogAction.getGlobalProperty))
                areProp = true;
        }
        if (proptype == PropertyActionType.get_local_property || proptype == PropertyActionType.any_local || proptype == PropertyActionType.any_get)
        {
            if ((interactionSerialized.FindPropertyRelative("type").enumValueIndex == (int)Interaction.InteractionType.properties_container &&
                     interactionSerialized.FindPropertyRelative("propertiesAction").enumValueIndex == (int)Interaction.PropertiesContainerAction.getLocalProperty)
               || (interactionSerialized.FindPropertyRelative("type").enumValueIndex == (int)Interaction.InteractionType.inventory &&
                     interactionSerialized.FindPropertyRelative("inventoryAction").enumValueIndex == (int)Interaction.InventoryAction.getLocalProperty)
               || (interactionSerialized.FindPropertyRelative("type").enumValueIndex == (int)Interaction.InteractionType.character &&
                     interactionSerialized.FindPropertyRelative("characterAction").enumValueIndex == (int)Interaction.CharacterAction.getLocalProperty)
                || (interactionSerialized.FindPropertyRelative("type").enumValueIndex == (int)Interaction.InteractionType.dialog &&
                     interactionSerialized.FindPropertyRelative("dialogAction").enumValueIndex == (int)Interaction.DialogAction.getLocalProperty))
                areProp = true;
        }
        if (proptype == PropertyActionType.set_global_property || proptype == PropertyActionType.any_global || proptype == PropertyActionType.any_set)
        {
            if ((interactionSerialized.FindPropertyRelative("type").enumValueIndex == (int)Interaction.InteractionType.properties_container &&
                     interactionSerialized.FindPropertyRelative("propertiesAction").enumValueIndex == (int)Interaction.PropertiesContainerAction.setGlobalProperty)
               || (interactionSerialized.FindPropertyRelative("type").enumValueIndex == (int)Interaction.InteractionType.inventory &&
                     interactionSerialized.FindPropertyRelative("inventoryAction").enumValueIndex == (int)Interaction.InventoryAction.setGlobalProperty)
               || (interactionSerialized.FindPropertyRelative("type").enumValueIndex == (int)Interaction.InteractionType.character &&
                     interactionSerialized.FindPropertyRelative("characterAction").enumValueIndex == (int)Interaction.CharacterAction.setGlobalProperty)
                || (interactionSerialized.FindPropertyRelative("type").enumValueIndex == (int)Interaction.InteractionType.dialog &&
                     interactionSerialized.FindPropertyRelative("dialogAction").enumValueIndex == (int)Interaction.DialogAction.setGlobalProperty))
                areProp = true;
        }
        if (proptype == PropertyActionType.set_local_property || proptype == PropertyActionType.any_local || proptype == PropertyActionType.any_set)
        {
            if ((interactionSerialized.FindPropertyRelative("type").enumValueIndex == (int)Interaction.InteractionType.properties_container &&
                     interactionSerialized.FindPropertyRelative("propertiesAction").enumValueIndex == (int)Interaction.PropertiesContainerAction.setLocalProperty)
               || (interactionSerialized.FindPropertyRelative("type").enumValueIndex == (int)Interaction.InteractionType.inventory &&
                     interactionSerialized.FindPropertyRelative("inventoryAction").enumValueIndex == (int)Interaction.InventoryAction.setLocalProperty)
               || (interactionSerialized.FindPropertyRelative("type").enumValueIndex == (int)Interaction.InteractionType.character &&
                     interactionSerialized.FindPropertyRelative("characterAction").enumValueIndex == (int)Interaction.CharacterAction.setLocalProperty)
                || (interactionSerialized.FindPropertyRelative("type").enumValueIndex == (int)Interaction.InteractionType.dialog &&
                     interactionSerialized.FindPropertyRelative("dialogAction").enumValueIndex == (int)Interaction.DialogAction.setLocalProperty))
                areProp = true;
        }


        if (type == PropertyObjectType.any)
        {
            if (interactionSerialized.FindPropertyRelative("type").enumValueIndex == (int)Interaction.InteractionType.properties_container
             || interactionSerialized.FindPropertyRelative("type").enumValueIndex == (int)Interaction.InteractionType.inventory
             || interactionSerialized.FindPropertyRelative("type").enumValueIndex == (int)Interaction.InteractionType.character
             || interactionSerialized.FindPropertyRelative("type").enumValueIndex == (int)Interaction.InteractionType.dialog)
                areType = true;
        }
        else if (type == PropertyObjectType.properties_container)
        {
            if (interactionSerialized.FindPropertyRelative("type").enumValueIndex == (int)Interaction.InteractionType.properties_container)
                areType = true;
        }
        else if (type == PropertyObjectType.inventory)
        {
            if (interactionSerialized.FindPropertyRelative("type").enumValueIndex == (int)Interaction.InteractionType.inventory)
                areType = true;
        }
        else if (type == PropertyObjectType.character)
        {
            if (interactionSerialized.FindPropertyRelative("type").enumValueIndex == (int)Interaction.InteractionType.character)
                areType = true;
        }
        else if (type == PropertyObjectType.dialog_option)
        {
            if (interactionSerialized.FindPropertyRelative("type").enumValueIndex == (int)Interaction.InteractionType.dialog)
                areType = true;
        }
        else
            areType = false;

        return areType && areProp;
    }

}
