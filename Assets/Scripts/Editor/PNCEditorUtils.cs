using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using UnityEditorInternal;
using UnityEditor.Rendering;
using UnityEngine.Events;
public static class PNCEditorUtils 
{
    public static void InitializeGlobalVariables(System.Enum type, ref InteractuableGlobalVariable[] globalVariables)
    {
        Settings settings = Resources.Load<Settings>("Settings/Settings");

        List<InteractuableGlobalVariable> tempGlobalVarList = new List<InteractuableGlobalVariable>();
        for (int i = 0; i < settings.global_variables.Length; i++)
        {
            bool founded = false;
            for (int j = 0; j < globalVariables.Length; j++)
            {
                if ((settings.global_variables[i].ID == -1 && globalVariables[j].name == settings.global_variables[i].name) ||
                    (settings.global_variables[i].ID != -1 && globalVariables[j].globalID == -1 && globalVariables[j].name == settings.global_variables[i].name) ||
                    (settings.global_variables[i].ID != -1 && globalVariables[j].globalID != -1 && globalVariables[j].globalID == settings.global_variables[i].ID))
                {
                    globalVariables[j].name = settings.global_variables[i].name;
                    
                    if (globalVariables[j].globalID == -1)
                    {
                        globalVariables[j].globalID = settings.global_variables[i].ID;
                    }

                    globalVariables[j].properties = settings.global_variables[i];

                    if (settings.global_variables[i].object_type.HasFlag(type))
                        tempGlobalVarList.Add(globalVariables[j]);
                    founded = true;
                }
            }
            if (founded == false)
            {
                InteractuableGlobalVariable tempVar = new InteractuableGlobalVariable();
                tempVar.name = settings.global_variables[i].name;
                tempVar.globalID = settings.global_variables[i].ID;
                tempVar.properties = settings.global_variables[i];
                if (settings.global_variables[i].object_type.HasFlag(type))
                    tempGlobalVarList.Add(tempVar);
            }
        }

        globalVariables = tempGlobalVarList.ToArray();
    }

    public static void InitializeLocalVariables(out ReorderableList list, SerializedObject serializedObject, SerializedProperty property)
    {
        list = new ReorderableList(serializedObject, property, true, true, true, true)
        {
            drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                SerializedProperty element = property.GetArrayElementAtIndex(index);

                Rect variablesRect = rect;
                variablesRect.height = EditorGUIUtility.singleLineHeight;
                EditorGUI.PropertyField(variablesRect, property.GetArrayElementAtIndex(index).FindPropertyRelative("name"));
                variablesRect.y += EditorGUIUtility.singleLineHeight;
                element.FindPropertyRelative("hasBoolean").boolValue = EditorGUI.Toggle(variablesRect, "have boolean value:", element.FindPropertyRelative("hasBoolean").boolValue);
                if (element.FindPropertyRelative("hasBoolean").boolValue)
                { 
                    variablesRect.y += EditorGUIUtility.singleLineHeight;
                    if (element.FindPropertyRelative("booleanDefault").boolValue)
                    {
                        EditorGUI.LabelField(variablesRect, "boolean value: default");
                        variablesRect.y += EditorGUIUtility.singleLineHeight;
                        if (GUI.Button(variablesRect, "set boolean value"))
                        {
                            element.FindPropertyRelative("booleanDefault").boolValue = false;
                        }
                    }
                    else
                    {
                        element.FindPropertyRelative("boolean").boolValue =
                            EditorGUI.Toggle(variablesRect, "boolean value:", element.FindPropertyRelative("boolean").boolValue);
                        variablesRect.y += EditorGUIUtility.singleLineHeight;
                        if (GUI.Button(variablesRect, "set boolean default value"))
                        {
                            element.FindPropertyRelative("booleanDefault").boolValue = true;
                        }
                    }
                }
                variablesRect.y += EditorGUIUtility.singleLineHeight;
                element.FindPropertyRelative("hasInteger").boolValue = EditorGUI.Toggle(variablesRect, "have integer value:", element.FindPropertyRelative("hasInteger").boolValue);
                if (element.FindPropertyRelative("hasInteger").boolValue)
                {
                    variablesRect.y += EditorGUIUtility.singleLineHeight;
                    if (element.FindPropertyRelative("integerDefault").boolValue)
                    {
                        EditorGUI.LabelField(variablesRect, "integer value: default");
                        variablesRect.y += EditorGUIUtility.singleLineHeight;
                        if (GUI.Button(variablesRect, "set integer value"))
                        {
                            element.FindPropertyRelative("integerDefault").boolValue = false;
                        }
                    }
                    else
                    {
                        element.FindPropertyRelative("integer").intValue =
                            EditorGUI.IntField(variablesRect, "integer value:", element.FindPropertyRelative("integer").intValue);
                        variablesRect.y += EditorGUIUtility.singleLineHeight;
                        if (GUI.Button(variablesRect, "set integer default value"))
                        {
                            element.FindPropertyRelative("integerDefault").boolValue = true;
                        }
                    }
                }
                variablesRect.y += EditorGUIUtility.singleLineHeight;
                element.FindPropertyRelative("hasString").boolValue = EditorGUI.Toggle(variablesRect, "have string value:", element.FindPropertyRelative("hasString").boolValue);
                if (element.FindPropertyRelative("hasString").boolValue)
                {
                    variablesRect.y += EditorGUIUtility.singleLineHeight;
                    if (element.FindPropertyRelative("stringDefault").boolValue)
                    {
                        EditorGUI.LabelField(variablesRect, "string value: default");
                        variablesRect.y += EditorGUIUtility.singleLineHeight;
                        if (GUI.Button(variablesRect, "set string value"))
                        {
                            property.GetArrayElementAtIndex(index).FindPropertyRelative("stringDefault").boolValue = false;
                        }
                    }
                    else
                    {
                        property.GetArrayElementAtIndex(index).FindPropertyRelative("String").stringValue=
                            EditorGUI.TextField(variablesRect, "string value:", property.GetArrayElementAtIndex(index).FindPropertyRelative("String").stringValue);
                        variablesRect.y += EditorGUIUtility.singleLineHeight;
                        if (GUI.Button(variablesRect, "set string default value"))
                        {
                            element.FindPropertyRelative("stringDefault").boolValue = true;
                        }
                    }
                }

            },
            elementHeightCallback = (int index) => 
            {
                SerializedProperty element = property.GetArrayElementAtIndex(index);

                float height = 5f;
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
                EditorGUI.LabelField(rect, "Local Variables");
            }

        };
        
    }

    public static void VerificateLocalVariables(ref InteractuableLocalVariable[] variables, ref SerializedProperty variables_serialized)
    {

        var group = variables.GroupBy(vari => vari.name, (vari) => new { Count = vari.name.Count() });
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

            GUILayout.Label("<b>There are more than one local variable with the same name</b>", style);
        }


    }

    public static void ShowLocalVariables(ReorderableList list, ref InteractuableLocalVariable[] local_variables, ref SerializedProperty local_variables_serialized)
    {
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUIStyle tittleStyle = new GUIStyle();
        tittleStyle.normal.textColor = Color.white;
        tittleStyle.fontSize = 14;
        GUILayout.Label("<b>Local Variables</b>", tittleStyle);
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        list.DoLayoutList();

        PNCEditorUtils.VerificateLocalVariables(ref local_variables, ref local_variables_serialized);
    }

    public static void ShowGlobalVariables(System.Enum type, ref InteractuableGlobalVariable[] variables, ref SerializedProperty variables_serialized)
    {
        Settings settings = Resources.Load<Settings>("Settings/Settings");

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUIStyle style = new GUIStyle();
        style.normal.textColor = Color.white;
        style.fontSize = 14;
        GUILayout.Label("<b>Global Variables</b>", style);
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        for (int i = 0; i < variables.Length; i++)
        {
            bool areType = true;

            for (int j = 0; j < settings.global_variables.Length; j++)
            {
                if (variables[i].globalID != -1 && settings.global_variables[j].ID == variables[i].globalID)
                {
                    variables[i].name = settings.global_variables[j].name;
                    if (!settings.global_variables[j].object_type.HasFlag(type))
                        areType = false;
                }
            }
            if (!areType)
                continue;

            variables[i].expandedInInspector = EditorGUILayout.Foldout(variables[i].expandedInInspector, variables[i].name);

            if (variables[i].expandedInInspector)
            {
                EditorGUILayout.BeginVertical("GroupBox");

                if (variables[i].properties.hasInteger)
                {
                    if (!variables[i].integerDefault)
                    {
                        variables_serialized.GetArrayElementAtIndex(i).FindPropertyRelative("integer").intValue = EditorGUILayout.IntField("integer value:", variables_serialized.GetArrayElementAtIndex(i).FindPropertyRelative("integer").intValue);
                        if (GUILayout.Button("Set integer default value"))
                        {
                            variables_serialized.GetArrayElementAtIndex(i).FindPropertyRelative("integerDefault").boolValue = true;
                        }
                    }
                    else
                    {
                        GUILayout.Label("integer value : default", EditorStyles.boldLabel);
                        if (GUILayout.Button("Set integer value"))
                        {
                            variables_serialized.GetArrayElementAtIndex(i).FindPropertyRelative("integerDefault").boolValue = false;
                        }
                    }
                }
                if (variables[i].properties.hasBoolean)
                {
                    if (!variables[i].booleanDefault)
                    {
                        variables_serialized.GetArrayElementAtIndex(i).FindPropertyRelative("boolean").boolValue = EditorGUILayout.Toggle("boolean value:", variables_serialized.GetArrayElementAtIndex(i).FindPropertyRelative("boolean").boolValue);
                        if (GUILayout.Button("Set boolean default value"))
                            variables_serialized.GetArrayElementAtIndex(i).FindPropertyRelative("booleanDefault").boolValue = true;
                    }
                    else
                    {
                        GUILayout.Label("boolean value : default", EditorStyles.boldLabel);
                        if (GUILayout.Button("Set boolean value"))
                        {
                            variables_serialized.GetArrayElementAtIndex(i).FindPropertyRelative("booleanDefault").boolValue = false;
                        }
                    }
                }
                if (variables[i].properties.hasString)
                {
                    if (!variables[i].stringDefault)
                    {
                        variables_serialized.GetArrayElementAtIndex(i).FindPropertyRelative("String").stringValue = EditorGUILayout.TextField("string value:", variables_serialized.GetArrayElementAtIndex(i).FindPropertyRelative("String").stringValue);
                        if (GUILayout.Button("Set string default value"))
                            variables_serialized.GetArrayElementAtIndex(i).FindPropertyRelative("stringDefault").boolValue = true;
                    }
                    else
                    {
                        GUILayout.Label("string value : default", EditorStyles.boldLabel);
                        if (GUILayout.Button("Set string value"))
                        {
                            variables_serialized.GetArrayElementAtIndex(i).FindPropertyRelative("stringDefault").boolValue = false;
                        }
                    }
                }

                GUILayout.EndVertical();
            }
        }

        if (GUILayout.Button("Edit global variables"))
        {
            Selection.objects = new UnityEngine.Object[] { settings };
            EditorGUIUtility.PingObject(settings);
        }


        var group = variables.GroupBy(vari => vari.name, (vari) => new { Count = vari.name.Count() });
        bool repeated = false;

        foreach (var vari in group)
        {
            if (vari.Count() > 1)
                repeated = true;

        }
        if (repeated)
        {
            GUIStyle styleRepeated = new GUIStyle();
            styleRepeated.normal.textColor = Color.red;
            styleRepeated.fontSize = 5;
            
            GUILayout.Label("<b>There are more than one global variable with the same name</b>", styleRepeated);
        }

    }

    public static float GetAttempsContainerHeight(SerializedProperty serializedVerb, int indexC)
    {

        if (serializedVerb.GetArrayElementAtIndex(indexC).FindPropertyRelative("attempsContainer").FindPropertyRelative("expandedInInspector").boolValue)
        {
            float heightM = 6f * EditorGUIUtility.singleLineHeight;
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



    public static float GetInteractionHeight(SerializedProperty interactionSerialized)
    {

        if (interactionSerialized.FindPropertyRelative("expandedInInspector").boolValue)
        {
            if (interactionSerialized.FindPropertyRelative("type").enumValueIndex == (int)Interaction.InteractionType.character)
            {
                float height = 5.25f;
                if (interactionSerialized.FindPropertyRelative("characterAction").enumValueIndex == (int)Interaction.CharacterAction.say ||
                    interactionSerialized.FindPropertyRelative("characterAction").enumValueIndex == (int)Interaction.CharacterAction.sayWithScript)
                    height++;
                return EditorGUIUtility.singleLineHeight * height;
            }
            if (interactionSerialized.FindPropertyRelative("type").enumValueIndex == (int)Interaction.InteractionType.inventory)
            {
                float height = 3.25f;
                return EditorGUIUtility.singleLineHeight * height;
            }
            if (interactionSerialized.FindPropertyRelative("type").enumValueIndex == (int)Interaction.InteractionType.dialog)
            {
                float height = 4.25f;
                return EditorGUIUtility.singleLineHeight * height;
            }
            if (interactionSerialized.FindPropertyRelative("type").enumValueIndex == (int)Interaction.InteractionType.variables)
            {
                float height = 4.25f;
                if ((PNCVariablesContainer)interactionSerialized.FindPropertyRelative("variableObject").objectReferenceValue != null)
                {
                    height += 1;
                    if (interactionSerialized.FindPropertyRelative("variablesAction").enumValueIndex == (int)Interaction.VariablesAction.getGlobalVariable
                        || interactionSerialized.FindPropertyRelative("variablesAction").enumValueIndex == (int)Interaction.VariablesAction.setGlobalVariable)
                    {
                        int index = interactionSerialized.FindPropertyRelative("globalVariableSelected").intValue;

                        if (((PNCVariablesContainer)interactionSerialized.FindPropertyRelative("variableObject").objectReferenceValue).global_variables.Length > index)
                        {
                            if (((PNCVariablesContainer)interactionSerialized.FindPropertyRelative("variableObject").objectReferenceValue).global_variables[index].properties.hasBoolean)
                            {
                                height += 1;
                                if (interactionSerialized.FindPropertyRelative("global_changeBooleanValue").boolValue)
                                    height += 1;


                            }
                            if (((PNCVariablesContainer)interactionSerialized.FindPropertyRelative("variableObject").objectReferenceValue).global_variables[index].properties.hasInteger)
                            {
                                height += 1;
                                if (interactionSerialized.FindPropertyRelative("global_changeIntegerValue").boolValue)
                                    height += 1;
                            }
                            if (((PNCVariablesContainer)interactionSerialized.FindPropertyRelative("variableObject").objectReferenceValue).global_variables[index].properties.hasString)
                            {
                                height += 1;
                                if (interactionSerialized.FindPropertyRelative("global_changeStringValue").boolValue)
                                    height += 1;
                            }
                            if (interactionSerialized.FindPropertyRelative("variablesAction").enumValueIndex == (int)Interaction.VariablesAction.getGlobalVariable)
                            {
                                if (interactionSerialized.FindPropertyRelative("global_compareBooleanValue").boolValue)
                                    height += 2;
                                if (interactionSerialized.FindPropertyRelative("global_compareIntegerValue").boolValue)
                                    height += 2;
                                if (interactionSerialized.FindPropertyRelative("global_compareStringValue").boolValue)
                                    height += 2;
                                if (interactionSerialized.FindPropertyRelative("OnCompareResultFalseAction").enumValueIndex == (int)Conditional.GetVariableAction.GoToSpecificLine)
                                    height += 1;
                                if (interactionSerialized.FindPropertyRelative("OnCompareResultTrueAction").enumValueIndex == (int)Conditional.GetVariableAction.GoToSpecificLine)
                                    height += 1;
                                if (interactionSerialized.FindPropertyRelative("global_compareBooleanValue").boolValue ||
                                    interactionSerialized.FindPropertyRelative("global_compareIntegerValue").boolValue ||
                                    interactionSerialized.FindPropertyRelative("global_compareStringValue").boolValue)
                                    height += 2;
                            }
                        }
                    }
                    if (interactionSerialized.FindPropertyRelative("variablesAction").enumValueIndex == (int)Interaction.VariablesAction.getLocalVariable
                        || interactionSerialized.FindPropertyRelative("variablesAction").enumValueIndex == (int)Interaction.VariablesAction.setLocalVariable)
                    {
                        int index = interactionSerialized.FindPropertyRelative("localVariableSelected").intValue;
                        if (((PNCVariablesContainer)interactionSerialized.FindPropertyRelative("variableObject").objectReferenceValue).local_variables.Length > index)
                        {
                            if (((PNCVariablesContainer)interactionSerialized.FindPropertyRelative("variableObject").objectReferenceValue).local_variables[index].hasBoolean)
                            {
                                height += 1;
                                if (interactionSerialized.FindPropertyRelative("local_changeBooleanValue").boolValue)
                                    height += 1;
                            }
                            if (((PNCVariablesContainer)interactionSerialized.FindPropertyRelative("variableObject").objectReferenceValue).local_variables[index].hasInteger)
                            {
                                height += 1;
                                if (interactionSerialized.FindPropertyRelative("local_changeIntegerValue").boolValue)
                                    height += 1;
                            }
                            if (((PNCVariablesContainer)interactionSerialized.FindPropertyRelative("variableObject").objectReferenceValue).local_variables[index].hasString)
                            {
                                height += 1;
                                if (interactionSerialized.FindPropertyRelative("local_changeStringValue").boolValue)
                                    height += 1;
                            }
                            if (interactionSerialized.FindPropertyRelative("variablesAction").enumValueIndex == (int)Interaction.VariablesAction.getLocalVariable)
                            {
                                if (interactionSerialized.FindPropertyRelative("local_compareBooleanValue").boolValue)
                                    height += 2;
                                if (interactionSerialized.FindPropertyRelative("local_compareIntegerValue").boolValue)
                                    height += 2;
                                if (interactionSerialized.FindPropertyRelative("local_compareStringValue").boolValue)
                                    height += 2;
                                if (interactionSerialized.FindPropertyRelative("OnCompareResultFalseAction").enumValueIndex == (int)Conditional.GetVariableAction.GoToSpecificLine)
                                    height += 1;
                                if (interactionSerialized.FindPropertyRelative("OnCompareResultTrueAction").enumValueIndex == (int)Conditional.GetVariableAction.GoToSpecificLine)
                                    height += 1;
                                if (interactionSerialized.FindPropertyRelative("local_compareBooleanValue").boolValue ||
                                    interactionSerialized.FindPropertyRelative("local_compareIntegerValue").boolValue ||
                                    interactionSerialized.FindPropertyRelative("local_compareStringValue").boolValue)
                                    height += 2;
                            }
                        }

                    }
                }
                return EditorGUIUtility.singleLineHeight * height;
            }
            if (interactionSerialized.FindPropertyRelative("type").enumValueIndex == (int)Interaction.InteractionType.custom)
                                return EditorGUIUtility.singleLineHeight * (8 + (interactionSerialized.FindPropertyRelative("action.m_PersistentCalls.m_Calls").arraySize * 3));
        }
        return EditorGUIUtility.singleLineHeight;

    }

    public static void DrawElementAttempContainer(SerializedProperty containerProperty, int indexC, Rect rect, Dictionary<string, ReorderableList> attempsListDict, Dictionary<string, ReorderableList> interactionsListDict, List<InteractionsAttemp> noSerializedAttemps, bool isInventoryItem)
    {
        var attempContainer = containerProperty.GetArrayElementAtIndex(indexC).FindPropertyRelative("attempsContainer");
        var attemps = attempContainer.FindPropertyRelative("attemps");
        var verbRect = new Rect(rect);
        var verbExpanded = attempContainer.FindPropertyRelative("expandedInInspector");
        verbRect.x += 8;



        //verbExpanded.boolValue = EditorGUI.Foldout(new Rect(verbRect.x, verbRect.y, verbRect.width, EditorGUIUtility.singleLineHeight), verbExpanded.boolValue, containerProperty.GetArrayElementAtIndex(indexC).FindPropertyRelative("name").stringValue);
        if(isInventoryItem)
            verbExpanded.boolValue = EditorGUI.Foldout(new Rect(verbRect.x, verbRect.y, verbRect.width, EditorGUIUtility.singleLineHeight), verbExpanded.boolValue, GUIContent.none);
        else
            verbExpanded.boolValue = EditorGUI.Foldout(new Rect(verbRect.x, verbRect.y, verbRect.width, EditorGUIUtility.singleLineHeight), verbExpanded.boolValue, containerProperty.GetArrayElementAtIndex(indexC).FindPropertyRelative("verb").FindPropertyRelative("name").stringValue);
                
        verbRect.y += EditorGUIUtility.singleLineHeight;

        if (verbExpanded.boolValue)
        {

            EditorGUI.PropertyField(new Rect(verbRect.x, verbRect.y, EditorGUIUtility.currentViewWidth, EditorGUIUtility.singleLineHeight), attempContainer.FindPropertyRelative("isCyclical"));

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

                                        if (interactExpanded.boolValue)
                                        {
                                            EditorGUI.PropertyField(interactRect, interactionSerialized.FindPropertyRelative("type"));
                                            interactRect.y += EditorGUIUtility.singleLineHeight;
                                            if (interactionSerialized.FindPropertyRelative("type").enumValueIndex == (int)Interaction.InteractionType.character)
                                            {
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
                                                }
                                                else if (interactionSerialized.FindPropertyRelative("characterAction").enumValueIndex == (int)Interaction.CharacterAction.walk)
                                                    EditorGUI.PropertyField(interactRect, interactionSerialized.FindPropertyRelative("WhereToWalk"));
                                            }
                                            else if (interactionSerialized.FindPropertyRelative("type").enumValueIndex == (int)Interaction.InteractionType.dialog)
                                            {
                                                EditorGUI.PropertyField(interactRect, interactionSerialized.FindPropertyRelative("dialogAction"));
                                                interactRect.y += EditorGUIUtility.singleLineHeight;
                                                EditorGUI.PropertyField(interactRect, interactionSerialized.FindPropertyRelative("dialogSelected"));
                                            }
                                            else if (interactionSerialized.FindPropertyRelative("type").enumValueIndex == (int)Interaction.InteractionType.inventory)
                                            {
                                                EditorGUI.PropertyField(interactRect, interactionSerialized.FindPropertyRelative("inventoryAction"));                                                
                                            }
                                            else if (interactionSerialized.FindPropertyRelative("type").enumValueIndex == (int)Interaction.InteractionType.variables)
                                            {
                                                EditorGUI.PropertyField(interactRect, interactionSerialized.FindPropertyRelative("variablesAction"));
                                                interactRect.y += EditorGUIUtility.singleLineHeight;
                                                EditorGUI.PropertyField(interactRect, interactionSerialized.FindPropertyRelative("variableObject"));
                                                interactRect.y += EditorGUIUtility.singleLineHeight;
                                                if (interactionSerialized.FindPropertyRelative("variablesAction").enumValueIndex == (int)Interaction.VariablesAction.getGlobalVariable ||
                                                    interactionSerialized.FindPropertyRelative("variablesAction").enumValueIndex == (int)Interaction.VariablesAction.setGlobalVariable)
                                                {
                                                    if (interactionSerialized.FindPropertyRelative("variableObject").objectReferenceValue != null)
                                                    {
                                                        PNCVariablesContainer variableObject = ((PNCVariablesContainer)interactionSerialized.FindPropertyRelative("variableObject").objectReferenceValue);
                                                        InteractuableGlobalVariable[] variables = variableObject.global_variables;
                                                        string[] content = new string[variables.Length];

                                                        for (int z = 0; z < variables.Length; z++)
                                                        {
                                                            content[z] = variableObject.global_variables[z].name;
                                                        }
                                                        interactionSerialized.FindPropertyRelative("globalVariableSelected").intValue = EditorGUI.Popup(interactRect, "Variable", interactionSerialized.FindPropertyRelative("globalVariableSelected").intValue, content);

                                                        int index = interactionSerialized.FindPropertyRelative("globalVariableSelected").intValue;
                                                        if (variableObject.global_variables.Length > index)
                                                        {
                                                            interactRect.y += EditorGUIUtility.singleLineHeight;
                                                            if (variableObject.global_variables[index].properties.hasBoolean)
                                                            {
                                                                if (interactionSerialized.FindPropertyRelative("variablesAction").enumValueIndex == (int) Interaction.VariablesAction.setGlobalVariable)
                                                                {
                                                                    interactionSerialized.FindPropertyRelative("global_changeBooleanValue").boolValue = EditorGUI.Toggle(interactRect, "change boolean value", interactionSerialized.FindPropertyRelative("global_changeBooleanValue").boolValue);
                                                                    if (interactionSerialized.FindPropertyRelative("global_changeBooleanValue").boolValue)
                                                                    {
                                                                        interactRect.y += EditorGUIUtility.singleLineHeight;
                                                                        interactionSerialized.FindPropertyRelative("global_BooleanValue").boolValue = EditorGUI.Toggle(interactRect, "value to set", interactionSerialized.FindPropertyRelative("global_BooleanValue").boolValue);
                                                                    }
                                                                }
                                                                else if (interactionSerialized.FindPropertyRelative("variablesAction").enumValueIndex == (int)Interaction.VariablesAction.getGlobalVariable)
                                                                {
                                                                    interactionSerialized.FindPropertyRelative("global_compareBooleanValue").boolValue = EditorGUI.Toggle(interactRect, "compare boolean value", interactionSerialized.FindPropertyRelative("global_compareBooleanValue").boolValue);
                                                                    if (interactionSerialized.FindPropertyRelative("global_compareBooleanValue").boolValue)
                                                                    {
                                                                        interactRect.y += EditorGUIUtility.singleLineHeight;
                                                                        interactionSerialized.FindPropertyRelative("global_BooleanValue").boolValue = EditorGUI.Toggle(interactRect, "value to compare", interactionSerialized.FindPropertyRelative("global_BooleanValue").boolValue);
                                                                        interactRect.y += EditorGUIUtility.singleLineHeight;
                                                                        interactionSerialized.FindPropertyRelative("global_defaultBooleanValue").boolValue = EditorGUI.Toggle(interactRect, "default value", interactionSerialized.FindPropertyRelative("global_defaultBooleanValue").boolValue);
                                                                    }
                                                                }
                                                            }
                                                            if (variableObject.global_variables[index].properties.hasInteger)
                                                            {
                                                                if (interactionSerialized.FindPropertyRelative("variablesAction").enumValueIndex == (int)Interaction.VariablesAction.setGlobalVariable)
                                                                {
                                                                    interactRect.y += EditorGUIUtility.singleLineHeight;
                                                                    interactionSerialized.FindPropertyRelative("global_changeIntegerValue").boolValue = EditorGUI.Toggle(interactRect, "change integer value", interactionSerialized.FindPropertyRelative("global_changeIntegerValue").boolValue);
                                                                    if (interactionSerialized.FindPropertyRelative("global_changeIntegerValue").boolValue)
                                                                    {
                                                                        interactRect.y += EditorGUIUtility.singleLineHeight;
                                                                        interactionSerialized.FindPropertyRelative("global_IntegerValue").intValue = EditorGUI.IntField(interactRect, "value", interactionSerialized.FindPropertyRelative("global_IntegerValue").intValue);
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    interactRect.y += EditorGUIUtility.singleLineHeight;
                                                                    interactionSerialized.FindPropertyRelative("global_compareIntegerValue").boolValue = EditorGUI.Toggle(interactRect, "compare integer value", interactionSerialized.FindPropertyRelative("global_compareIntegerValue").boolValue);
                                                                    if (interactionSerialized.FindPropertyRelative("global_compareIntegerValue").boolValue)
                                                                    {
                                                                        interactRect.y += EditorGUIUtility.singleLineHeight;
                                                                        interactionSerialized.FindPropertyRelative("global_IntegerValue").intValue = EditorGUI.IntField(interactRect, "value to compare", interactionSerialized.FindPropertyRelative("global_IntegerValue").intValue);
                                                                        interactRect.y += EditorGUIUtility.singleLineHeight;
                                                                        interactionSerialized.FindPropertyRelative("global_defaultIntegerValue").intValue = EditorGUI.IntField(interactRect, "default value", interactionSerialized.FindPropertyRelative("global_defaultIntegerValue").intValue);
                                                                    }
                                                                }
                                                            }
                                                            if (variableObject.global_variables[index].properties.hasString)
                                                            {
                                                                if (interactionSerialized.FindPropertyRelative("variablesAction").enumValueIndex == (int)Interaction.VariablesAction.setGlobalVariable)
                                                                {
                                                                    interactRect.y += EditorGUIUtility.singleLineHeight;
                                                                    interactionSerialized.FindPropertyRelative("global_changeStringValue").boolValue = EditorGUI.Toggle(interactRect, "change string value", interactionSerialized.FindPropertyRelative("global_changeStringValue").boolValue);
                                                                    if (interactionSerialized.FindPropertyRelative("global_changeStringValue").boolValue)
                                                                    {
                                                                        interactRect.y += EditorGUIUtility.singleLineHeight;
                                                                        interactionSerialized.FindPropertyRelative("global_StringValue").stringValue = EditorGUI.TextField(interactRect, "value", interactionSerialized.FindPropertyRelative("global_StringValue").stringValue);
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    interactRect.y += EditorGUIUtility.singleLineHeight;
                                                                    interactionSerialized.FindPropertyRelative("global_compareStringValue").boolValue = EditorGUI.Toggle(interactRect, "compare string value", interactionSerialized.FindPropertyRelative("global_compareStringValue").boolValue);
                                                                    if (interactionSerialized.FindPropertyRelative("global_compareStringValue").boolValue)
                                                                    {
                                                                        interactRect.y += EditorGUIUtility.singleLineHeight;
                                                                        interactionSerialized.FindPropertyRelative("global_StringValue").stringValue = EditorGUI.TextField(interactRect, "value to compare", interactionSerialized.FindPropertyRelative("global_StringValue").stringValue);
                                                                        interactRect.y += EditorGUIUtility.singleLineHeight;
                                                                        interactionSerialized.FindPropertyRelative("global_defaultStringValue").stringValue = EditorGUI.TextField(interactRect, "default value", interactionSerialized.FindPropertyRelative("global_defaultStringValue").stringValue);
                                                                    }
                                                                }
                                                            }
                                                            if (interactionSerialized.FindPropertyRelative("variablesAction").enumValueIndex == (int)Interaction.VariablesAction.getGlobalVariable &&
                                                                    (interactionSerialized.FindPropertyRelative("global_compareBooleanValue").boolValue ||
                                                                    interactionSerialized.FindPropertyRelative("global_compareIntegerValue").boolValue ||
                                                                    interactionSerialized.FindPropertyRelative("global_compareStringValue").boolValue))
                                                            {
                                                                interactRect.y += EditorGUIUtility.singleLineHeight;
                                                                interactionSerialized.FindPropertyRelative("OnCompareResultTrueAction").enumValueIndex = EditorGUI.Popup(interactRect, interactionSerialized.FindPropertyRelative("OnCompareResultTrueAction").enumValueIndex,interactionSerialized.FindPropertyRelative("OnCompareResultTrueAction").enumDisplayNames);
                                                                interactRect.y += EditorGUIUtility.singleLineHeight;
                                                                if (interactionSerialized.FindPropertyRelative("OnCompareResultTrueAction").enumValueIndex == (int)Conditional.GetVariableAction.GoToSpecificLine)
                                                                {
                                                                    interactionSerialized.FindPropertyRelative("LineToGoOnTrueResult").intValue = EditorGUI.Popup(interactRect, "line to go", interactionSerialized.FindPropertyRelative("LineToGoOnTrueResult").intValue, PNCEditorUtils.GetInteractionsText(attemps.GetArrayElementAtIndex(indexA).FindPropertyRelative("interactions")));
                                                                    interactRect.y += EditorGUIUtility.singleLineHeight;
                                                                }
                                                                interactionSerialized.FindPropertyRelative("OnCompareResultFalseAction").enumValueIndex = EditorGUI.Popup(interactRect, interactionSerialized.FindPropertyRelative("OnCompareResultFalseAction").enumValueIndex, interactionSerialized.FindPropertyRelative("OnCompareResultFalseAction").enumDisplayNames);
                                                                interactRect.y += EditorGUIUtility.singleLineHeight;
                                                                if (interactionSerialized.FindPropertyRelative("OnCompareResultFalseAction").enumValueIndex == (int)Conditional.GetVariableAction.GoToSpecificLine)
                                                                {
                                                                    interactionSerialized.FindPropertyRelative("LineToGoOnFalseResult").intValue = EditorGUI.Popup(interactRect, "line to go", interactionSerialized.FindPropertyRelative("LineToGoOnFalseResult").intValue, PNCEditorUtils.GetInteractionsText(attemps.GetArrayElementAtIndex(indexA).FindPropertyRelative("interactions")));
                                                                }
                                                            }
                                                        }

                                                    }
                                                }
                                                else if (interactionSerialized.FindPropertyRelative("variablesAction").enumValueIndex == (int)Interaction.VariablesAction.getLocalVariable ||
                                                    interactionSerialized.FindPropertyRelative("variablesAction").enumValueIndex == (int)Interaction.VariablesAction.setLocalVariable)
                                                {
                                                    PNCVariablesContainer variableObject = ((PNCVariablesContainer)interactionSerialized.FindPropertyRelative("variableObject").objectReferenceValue);
                                                    if (variableObject != null)
                                                    {
                                                        InteractuableLocalVariable[] variables = variableObject.local_variables;
                                                        string[] content = new string[variables.Length];

                                                        for (int z = 0; z < variables.Length; z++)
                                                        {
                                                            content[z] = variableObject.local_variables[z].name;
                                                        }
                                                        interactionSerialized.FindPropertyRelative("localVariableSelected").intValue = EditorGUI.Popup(interactRect, "Variable", interactionSerialized.FindPropertyRelative("localVariableSelected").intValue, content);
                                                        int index = interactionSerialized.FindPropertyRelative("localVariableSelected").intValue;
                                                        if (variableObject.local_variables.Length > index)
                                                        {
                                                            if (variableObject.local_variables[index].hasBoolean)
                                                            {
                                                                if (interactionSerialized.FindPropertyRelative("variablesAction").enumValueIndex == (int)Interaction.VariablesAction.setLocalVariable)
                                                                {
                                                                    interactRect.y += EditorGUIUtility.singleLineHeight;
                                                                    interactionSerialized.FindPropertyRelative("local_changeBooleanValue").boolValue = EditorGUI.Toggle(interactRect, "change boolean value", interactionSerialized.FindPropertyRelative("local_changeBooleanValue").boolValue);
                                                                    if (interactionSerialized.FindPropertyRelative("local_changeBooleanValue").boolValue)
                                                                    {
                                                                        interactRect.y += EditorGUIUtility.singleLineHeight;
                                                                        interactionSerialized.FindPropertyRelative("local_BooleanValue").boolValue = EditorGUI.Toggle(interactRect, "value", interactionSerialized.FindPropertyRelative("local_BooleanValue").boolValue);
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    interactRect.y += EditorGUIUtility.singleLineHeight;
                                                                    interactionSerialized.FindPropertyRelative("local_compareBooleanValue").boolValue = EditorGUI.Toggle(interactRect, "compare integer value", interactionSerialized.FindPropertyRelative("local_compareBooleanValue").boolValue);
                                                                    if (interactionSerialized.FindPropertyRelative("local_compareBooleanValue").boolValue)
                                                                    {
                                                                        interactRect.y += EditorGUIUtility.singleLineHeight;
                                                                        interactionSerialized.FindPropertyRelative("local_BooleanValue").boolValue = EditorGUI.Toggle(interactRect, "value to compare", interactionSerialized.FindPropertyRelative("local_BooleanValue").boolValue);
                                                                        interactRect.y += EditorGUIUtility.singleLineHeight;
                                                                        interactionSerialized.FindPropertyRelative("local_defaultBooleanValue").boolValue = EditorGUI.Toggle(interactRect, "default value", interactionSerialized.FindPropertyRelative("local_defaultBooleanValue").boolValue);
                                                                    }
                                                                }
                                                            }
                                                            if (variableObject.local_variables[index].hasInteger)
                                                            {
                                                                if (interactionSerialized.FindPropertyRelative("variablesAction").enumValueIndex == (int)Interaction.VariablesAction.setLocalVariable)
                                                                {
                                                                    interactRect.y += EditorGUIUtility.singleLineHeight;
                                                                    interactionSerialized.FindPropertyRelative("local_changeIntegerValue").boolValue = EditorGUI.Toggle(interactRect, "change integer value", interactionSerialized.FindPropertyRelative("local_changeIntegerValue").boolValue);
                                                                    if (interactionSerialized.FindPropertyRelative("local_changeIntegerValue").boolValue)
                                                                    {
                                                                        interactRect.y += EditorGUIUtility.singleLineHeight;
                                                                        interactionSerialized.FindPropertyRelative("local_IntegerValue").intValue = EditorGUI.IntField(interactRect, "value", interactionSerialized.FindPropertyRelative("local_IntegerValue").intValue);
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    interactRect.y += EditorGUIUtility.singleLineHeight;
                                                                    interactionSerialized.FindPropertyRelative("local_compareIntegerValue").boolValue = EditorGUI.Toggle(interactRect, "compare integer value", interactionSerialized.FindPropertyRelative("local_compareIntegerValue").boolValue);
                                                                    if (interactionSerialized.FindPropertyRelative("local_compareIntegerValue").boolValue)
                                                                    {
                                                                        interactRect.y += EditorGUIUtility.singleLineHeight;
                                                                        interactionSerialized.FindPropertyRelative("local_IntegerValue").intValue = EditorGUI.IntField(interactRect, "value to compare", interactionSerialized.FindPropertyRelative("local_IntegerValue").intValue);
                                                                        interactRect.y += EditorGUIUtility.singleLineHeight;
                                                                        interactionSerialized.FindPropertyRelative("local_defaultIntegerValue").intValue  = EditorGUI.IntField(interactRect, "default value", interactionSerialized.FindPropertyRelative("local_defaultIntegerValue").intValue);
                                                                    }
                                                                }
                                                            }
                                                            if (variableObject.local_variables[index].hasString)
                                                            {
                                                                if (interactionSerialized.FindPropertyRelative("variablesAction").enumValueIndex == (int)Interaction.VariablesAction.setLocalVariable)
                                                                {
                                                                    interactRect.y += EditorGUIUtility.singleLineHeight;
                                                                    interactionSerialized.FindPropertyRelative("local_changeStringValue").boolValue = EditorGUI.Toggle(interactRect, "change string value", interactionSerialized.FindPropertyRelative("local_changeStringValue").boolValue);
                                                                    if (interactionSerialized.FindPropertyRelative("local_changeStringValue").boolValue)
                                                                    {
                                                                        interactRect.y += EditorGUIUtility.singleLineHeight;
                                                                        interactionSerialized.FindPropertyRelative("local_StringValue").stringValue = EditorGUI.TextField(interactRect, "value", interactionSerialized.FindPropertyRelative("local_StringValue").stringValue);
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    interactRect.y += EditorGUIUtility.singleLineHeight;
                                                                    interactionSerialized.FindPropertyRelative("local_compareStringValue").boolValue = EditorGUI.Toggle(interactRect, "compare string value", interactionSerialized.FindPropertyRelative("local_compareStringValue").boolValue);
                                                                    if (interactionSerialized.FindPropertyRelative("local_compareStringValue").boolValue)
                                                                    {
                                                                        interactRect.y += EditorGUIUtility.singleLineHeight;
                                                                        interactionSerialized.FindPropertyRelative("local_StringValue").stringValue = EditorGUI.TextField(interactRect, "value to compare", interactionSerialized.FindPropertyRelative("local_StringValue").stringValue);
                                                                        interactRect.y += EditorGUIUtility.singleLineHeight;
                                                                        interactionSerialized.FindPropertyRelative("local_defaultStringValue").stringValue = EditorGUI.TextField(interactRect, "default value", interactionSerialized.FindPropertyRelative("local_defaultStringValue").stringValue);
                                                                    }
                                                                }
                                                            }
                                                            if (interactionSerialized.FindPropertyRelative("variablesAction").enumValueIndex == (int)Interaction.VariablesAction.getLocalVariable &&
                                                                    (interactionSerialized.FindPropertyRelative("local_compareBooleanValue").boolValue ||
                                                                    interactionSerialized.FindPropertyRelative("local_compareIntegerValue").boolValue ||
                                                                    interactionSerialized.FindPropertyRelative("local_compareStringValue").boolValue))
                                                            {
                                                                interactRect.y += EditorGUIUtility.singleLineHeight;
                                                                interactionSerialized.FindPropertyRelative("OnCompareResultTrueAction").enumValueIndex = EditorGUI.Popup(interactRect, interactionSerialized.FindPropertyRelative("OnCompareResultTrueAction").enumValueIndex, interactionSerialized.FindPropertyRelative("OnCompareResultTrueAction").enumDisplayNames);
                                                                interactRect.y += EditorGUIUtility.singleLineHeight;
                                                                if (interactionSerialized.FindPropertyRelative("OnCompareResultTrueAction").enumValueIndex == (int)Conditional.GetVariableAction.GoToSpecificLine)
                                                                {
                                                                    interactionSerialized.FindPropertyRelative("LineToGoOnTrueResult").intValue = EditorGUI.Popup(interactRect, "line to go", interactionSerialized.FindPropertyRelative("LineToGoOnTrueResult").intValue, PNCEditorUtils.GetInteractionsText(attemps.GetArrayElementAtIndex(indexA).FindPropertyRelative("interactions")));
                                                                    interactRect.y += EditorGUIUtility.singleLineHeight;
                                                                }
                                                                interactionSerialized.FindPropertyRelative("OnCompareResultFalseAction").enumValueIndex = EditorGUI.Popup(interactRect, interactionSerialized.FindPropertyRelative("OnCompareResultFalseAction").enumValueIndex, interactionSerialized.FindPropertyRelative("OnCompareResultFalseAction").enumDisplayNames);
                                                                interactRect.y += EditorGUIUtility.singleLineHeight;
                                                                if (interactionSerialized.FindPropertyRelative("OnCompareResultFalseAction").enumValueIndex == (int)Conditional.GetVariableAction.GoToSpecificLine)
                                                                {
                                                                    interactionSerialized.FindPropertyRelative("LineToGoOnFalseResult").intValue = EditorGUI.Popup(interactRect, "line to go", interactionSerialized.FindPropertyRelative("LineToGoOnFalseResult").intValue, PNCEditorUtils.GetInteractionsText(attemps.GetArrayElementAtIndex(indexA).FindPropertyRelative("interactions")));
                                                                }
                                                            }
                                                        }

                                                    }
                                                }
                                            }
                                            else if (interactionSerialized.FindPropertyRelative("type").enumValueIndex == (int)Interaction.InteractionType.custom)
                                                EditorGUI.PropertyField(interactRect, interactionSerialized.FindPropertyRelative("action"));

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
            if (interactions.GetArrayElementAtIndex(i).FindPropertyRelative("type").enumValueIndex == (int)Interaction.InteractionType.variables)
            {
                texts[i] += " " + interactions.GetArrayElementAtIndex(i).FindPropertyRelative("variablesAction").enumNames[interactions.GetArrayElementAtIndex(i).FindPropertyRelative("variablesAction").enumValueIndex].ToString();
            }
            if (interactions.GetArrayElementAtIndex(i).FindPropertyRelative("type").enumValueIndex == (int)Interaction.InteractionType.inventory)
            {
                texts[i] += " " + interactions.GetArrayElementAtIndex(i).FindPropertyRelative("inventoryAction").enumNames[interactions.GetArrayElementAtIndex(i).FindPropertyRelative("inventoryAction").enumValueIndex].ToString();
            }
        }

        return texts;

    }

}
