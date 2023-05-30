using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using UnityEditorInternal;
using UnityEditor.Rendering;
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
                    (settings.global_variables[i].ID != -1 && globalVariables[j].globalHashCode == -1 && globalVariables[j].name == settings.global_variables[i].name) ||
                    (settings.global_variables[i].ID != -1 && globalVariables[j].globalHashCode != -1 && globalVariables[j].globalHashCode == settings.global_variables[i].ID))
                {
                    globalVariables[j].name = settings.global_variables[i].name;
                    if (settings.global_variables[i].ID == -1)
                    {
                        settings.global_variables[i].ID = globalVariables[j].GetHashCode();
                        globalVariables[j].globalHashCode = globalVariables[j].GetHashCode();
                    }
                    else if (globalVariables[j].globalHashCode == -1)
                    {
                        globalVariables[j].globalHashCode = settings.global_variables[i].ID;
                    }

                    globalVariables[j].properties = settings.global_variables[i];

                    if (settings.global_variables[i].object_type.HasFlag(type))
                        tempGlobalVarList.Add(globalVariables[j]);
                    founded = true;
                }
            }
            if (founded == false)
            {
                InteractuableGlobalVariable tempVerb = new InteractuableGlobalVariable();
                tempVerb.name = settings.global_variables[i].name;
                if (settings.global_variables[i].ID == -1)
                {
                    settings.global_variables[i].ID = tempVerb.GetHashCode();
                    tempVerb.globalHashCode = tempVerb.GetHashCode();
                }
                else
                {
                    tempVerb.globalHashCode = settings.global_variables[i].ID;
                }
                tempVerb.properties = settings.global_variables[i];
                if (settings.global_variables[i].object_type.HasFlag(type))
                    tempGlobalVarList.Add(tempVerb);
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
                if (variables[i].globalHashCode != -1 && settings.global_variables[j].ID == variables[i].globalHashCode)
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

                if (variables[i].properties.variable_type.HasFlag(GlobalVariableProperty.variable_types.integer))
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
                if (variables[i].properties.variable_type.HasFlag(GlobalVariableProperty.variable_types.boolean))
                {
                    if (!variables[i].booleanDefault)
                    {
                        variables_serialized.GetArrayElementAtIndex(i).FindPropertyRelative("boolean").boolValue = EditorGUILayout.Toggle("boolean value:", variables_serialized.GetArrayElementAtIndex(i).FindPropertyRelative("boolean").boolValue);
                        if (GUILayout.Button("Set boolean default value"))
                            variables_serialized.GetArrayElementAtIndex(i).FindPropertyRelative("booleanDefault").boolValue = false;
                    }
                    else
                    {
                        GUILayout.Label("boolean value : default", EditorStyles.boldLabel);
                        if (GUILayout.Button("Set boolean value"))
                        {
                            variables_serialized.GetArrayElementAtIndex(i).FindPropertyRelative("booleanDefault").boolValue = true;
                        }
                    }
                }
                if (variables[i].properties.variable_type.HasFlag(GlobalVariableProperty.variable_types.String))
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

    public static float GetAttempsContainerHeight(SerializedProperty serializedContainer, List<InteractionsAttemp> noSerializedAttemps, int indexC)
    {

        if (serializedContainer.GetArrayElementAtIndex(indexC).FindPropertyRelative("expandedInInspector").boolValue)
        {
            float heightM = 5 * EditorGUIUtility.singleLineHeight;
            var attemps = serializedContainer.GetArrayElementAtIndex(indexC).FindPropertyRelative("attemps");
            for (int i = 0; i < attemps.arraySize; i++)
            {
                heightM += GetAttempHeight(attemps.GetArrayElementAtIndex(i), noSerializedAttemps[i]);

            }
            return heightM;
        }
        return EditorGUIUtility.singleLineHeight;
    }

    public static float GetAttempHeight(SerializedProperty attempSerialized, InteractionsAttemp attempNoSerialized)
    {
        if (attempSerialized.FindPropertyRelative("expandedInInspector").boolValue)
        {
            float height = 5 * EditorGUIUtility.singleLineHeight;
            for (int i = 0; i < attempSerialized.FindPropertyRelative("interactions").arraySize; i++)
            {
                height += GetInteractionHeight(attempSerialized.FindPropertyRelative("interactions").GetArrayElementAtIndex(i), attempNoSerialized.interactions[i]);
            }
            return height;
        }

        return EditorGUIUtility.singleLineHeight;

    }



    public static float GetInteractionHeight(SerializedProperty interactionSerialized, Interaction interactionNoSerialized)
    {

        if (interactionSerialized.FindPropertyRelative("expandedInInspector").boolValue)
        {
            if (interactionNoSerialized.type == Interaction.InteractionType.character)
            {
                float height = 5.25f;
                if (interactionNoSerialized.characterAction == Interaction.CharacterAction.say ||
                    interactionNoSerialized.characterAction == Interaction.CharacterAction.sayWithScript)
                    height++;
                return EditorGUIUtility.singleLineHeight * height;
            }
            if (interactionNoSerialized.type == Interaction.InteractionType.variables)
            {
                float height = 4.25f;
                if (interactionNoSerialized.variableObject)
                {
                    height += 1;
                    if (interactionNoSerialized.variablesAction == Interaction.VariablesAction.getGlobalVariable
                        || interactionNoSerialized.variablesAction == Interaction.VariablesAction.setGlobalVariable)
                    {
                        int index = interactionNoSerialized.globalVariableSelected;

                        if (interactionNoSerialized.variableObject.global_variables.Length > index)
                        {
                            if (interactionNoSerialized.variableObject.global_variables[index].properties.variable_type.HasFlag(GlobalVariableProperty.variable_types.boolean))
                            {
                                height += 1;
                                if (interactionNoSerialized.global_changeBooleanValue)
                                    height += 1;


                            }
                            if (interactionNoSerialized.variableObject.global_variables[index].properties.variable_type.HasFlag(GlobalVariableProperty.variable_types.integer))
                            {
                                height += 1;
                                if (interactionNoSerialized.global_changeIntegerValue)
                                    height += 1;
                            }
                            if (interactionNoSerialized.variableObject.global_variables[index].properties.variable_type.HasFlag(GlobalVariableProperty.variable_types.String))
                            {
                                height += 1;
                                if (interactionNoSerialized.global_changeStringValue)
                                    height += 1;
                            }
                            if (interactionNoSerialized.variablesAction == Interaction.VariablesAction.getGlobalVariable)
                            {
                                if (interactionNoSerialized.global_compareBooleanValue)
                                    height += 2;
                                if (interactionNoSerialized.global_compareIntegerValue)
                                    height += 2;
                                if (interactionNoSerialized.global_compareStringValue)
                                    height += 2;
                                if (interactionNoSerialized.OnCompareResultFalseAction == Conditional.GetVariableAction.GoToSpecificLine)
                                    height += 1;
                                if (interactionNoSerialized.OnCompareResultTrueAction == Conditional.GetVariableAction.GoToSpecificLine)
                                    height += 1;
                                if (interactionNoSerialized.global_compareBooleanValue ||
                                    interactionNoSerialized.global_compareIntegerValue ||
                                    interactionNoSerialized.global_compareStringValue)
                                    height += 2;
                            }
                        }
                    }
                    if (interactionNoSerialized.variablesAction == Interaction.VariablesAction.getLocalVariable
                        || interactionNoSerialized.variablesAction == Interaction.VariablesAction.setLocalVariable)
                    {
                        int index = interactionNoSerialized.localVariableSelected;
                        if (interactionNoSerialized.variableObject.local_variables.Length > index)
                        {
                            if (interactionNoSerialized.variableObject.local_variables[index].hasBoolean)
                            {
                                height += 1;
                                if (interactionNoSerialized.local_changeBooleanValue)
                                    height += 1;
                            }
                            if (interactionNoSerialized.variableObject.local_variables[index].hasInteger)
                            {
                                height += 1;
                                if (interactionNoSerialized.local_changeIntegerValue)
                                    height += 1;
                            }
                            if (interactionNoSerialized.variableObject.local_variables[index].hasString)
                            {
                                height += 1;
                                if (interactionNoSerialized.local_changeStringValue)
                                    height += 1;
                            }
                            if (interactionNoSerialized.variablesAction == Interaction.VariablesAction.getLocalVariable)
                            {
                                if (interactionNoSerialized.local_compareBooleanValue)
                                    height += 2;
                                if (interactionNoSerialized.local_compareIntegerValue)
                                    height += 2;
                                if (interactionNoSerialized.local_compareStringValue)
                                    height += 2;
                                if (interactionNoSerialized.OnCompareResultFalseAction == Conditional.GetVariableAction.GoToSpecificLine)
                                    height += 1;
                                if (interactionNoSerialized.OnCompareResultTrueAction == Conditional.GetVariableAction.GoToSpecificLine)
                                    height += 1;
                                if (interactionNoSerialized.local_compareBooleanValue ||
                                    interactionNoSerialized.local_compareIntegerValue ||
                                    interactionNoSerialized.local_compareStringValue)
                                    height += 2;
                            }
                        }

                    }
                }
                return EditorGUIUtility.singleLineHeight * height;
            }
            if (interactionNoSerialized.type == Interaction.InteractionType.custom)
                return EditorGUIUtility.singleLineHeight * (8 + interactionNoSerialized.action.GetPersistentEventCount() * 3);
        }
        return EditorGUIUtility.singleLineHeight;

    }

    public static void DrawElementAttempContainer(SerializedProperty containerProperty, int indexC, Rect rect, Dictionary<string, ReorderableList> attempsListDict, Dictionary<string, ReorderableList> interactionsListDict, List<InteractionsAttemp> noSerializedAttemps)
    {
        var attempContainer = containerProperty.GetArrayElementAtIndex(indexC);
        var attemps = attempContainer.FindPropertyRelative("attemps");
        var verbRect = new Rect(rect);
        var verbExpanded = attempContainer.FindPropertyRelative("expandedInInspector");
        verbRect.x += 8;

        verbExpanded.boolValue = EditorGUI.Foldout(new Rect(verbRect.x, verbRect.y, verbRect.width, EditorGUIUtility.singleLineHeight), verbExpanded.boolValue, attempContainer.FindPropertyRelative("name").stringValue);

        verbRect.y += EditorGUIUtility.singleLineHeight;

        if (verbExpanded.boolValue)
        {
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
                        return PNCEditorUtils.GetAttempHeight(attemps.GetArrayElementAtIndex(indexA), noSerializedAttemps[indexA]);
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
                                        var interactionNoSerialized = noSerializedAttemps[indexA].interactions[indexI];

                                        return PNCEditorUtils.GetInteractionHeight(interactionSerialized, interactionNoSerialized);
                                    }
                                    ,
                                    drawElementCallback = (rectI, indexI, activeI, focusI) =>
                                    {
                                        var interactionSerialized = interactionsListDict[interactionKey].serializedProperty.GetArrayElementAtIndex(indexI);
                                        var interactionNoSerialized = noSerializedAttemps[indexA].interactions[indexI];
                                        var interactRect = new Rect(rectI);
                                        var interactExpanded = interactionSerialized.FindPropertyRelative("expandedInInspector");
                                        interactRect.height = EditorGUIUtility.singleLineHeight;

                                        interactExpanded.boolValue = EditorGUI.Foldout(interactRect, interactExpanded.boolValue, (indexI + 1).ToString() + "° interaction");
                                        interactRect.y += EditorGUIUtility.singleLineHeight;

                                        if (interactExpanded.boolValue)
                                        {
                                            EditorGUI.PropertyField(interactRect, interactionSerialized.FindPropertyRelative("type"));
                                            interactRect.y += EditorGUIUtility.singleLineHeight;
                                            if (interactionNoSerialized.type == Interaction.InteractionType.character)
                                            {
                                                EditorGUI.PropertyField(interactRect, interactionSerialized.FindPropertyRelative("character"));
                                                interactRect.y += EditorGUIUtility.singleLineHeight;
                                                EditorGUI.PropertyField(interactRect, interactionSerialized.FindPropertyRelative("characterAction"));
                                                interactRect.y += EditorGUIUtility.singleLineHeight;
                                                if (interactionNoSerialized.characterAction == Interaction.CharacterAction.say)
                                                {
                                                    EditorGUI.PropertyField(interactRect, interactionSerialized.FindPropertyRelative("WhatToSay"));
                                                    interactRect.y += EditorGUIUtility.singleLineHeight;
                                                    EditorGUI.PropertyField(interactRect, interactionSerialized.FindPropertyRelative("CanSkip"));
                                                }
                                                if (interactionNoSerialized.characterAction == Interaction.CharacterAction.sayWithScript)
                                                {
                                                    EditorGUI.PropertyField(interactRect, interactionSerialized.FindPropertyRelative("SayScript"));
                                                    if (!(interactionSerialized.FindPropertyRelative("SayScript").objectReferenceValue is SayScript))
                                                    {
                                                        interactionSerialized.FindPropertyRelative("SayScript").objectReferenceValue = null;
                                                    }
                                                    interactRect.y += EditorGUIUtility.singleLineHeight;
                                                    EditorGUI.PropertyField(interactRect, interactionSerialized.FindPropertyRelative("CanSkip"));
                                                }
                                                else if (interactionNoSerialized.characterAction == Interaction.CharacterAction.walk)
                                                    EditorGUI.PropertyField(interactRect, interactionSerialized.FindPropertyRelative("WhereToWalk"));
                                            }
                                            else if (interactionNoSerialized.type == Interaction.InteractionType.variables)
                                            {
                                                EditorGUI.PropertyField(interactRect, interactionSerialized.FindPropertyRelative("variablesAction"));
                                                interactRect.y += EditorGUIUtility.singleLineHeight;
                                                EditorGUI.PropertyField(interactRect, interactionSerialized.FindPropertyRelative("variableObject"));
                                                interactRect.y += EditorGUIUtility.singleLineHeight;
                                                if (interactionNoSerialized.variablesAction == Interaction.VariablesAction.getGlobalVariable ||
                                                    interactionNoSerialized.variablesAction == Interaction.VariablesAction.setGlobalVariable)
                                                {
                                                    if (interactionNoSerialized.variableObject)
                                                    {
                                                        InteractuableGlobalVariable[] variables = interactionNoSerialized.variableObject.global_variables;
                                                        string[] content = new string[variables.Length];

                                                        for (int z = 0; z < variables.Length; z++)
                                                        {
                                                            content[z] = interactionNoSerialized.variableObject.global_variables[z].name;
                                                        }
                                                        interactionNoSerialized.globalVariableSelected = EditorGUI.Popup(interactRect, "Variable", interactionNoSerialized.globalVariableSelected, content);

                                                        int index = interactionNoSerialized.globalVariableSelected;
                                                        if (interactionNoSerialized.variableObject.global_variables.Length > index)
                                                        {
                                                            interactRect.y += EditorGUIUtility.singleLineHeight;
                                                            if (interactionNoSerialized.variableObject.global_variables[index].properties.variable_type.HasFlag(GlobalVariableProperty.variable_types.boolean))
                                                            {
                                                                if (interactionNoSerialized.variablesAction == Interaction.VariablesAction.setGlobalVariable)
                                                                {
                                                                    interactionNoSerialized.global_changeBooleanValue = EditorGUI.Toggle(interactRect, "change boolean value", interactionNoSerialized.global_changeBooleanValue);
                                                                    if (interactionNoSerialized.global_changeBooleanValue)
                                                                    {
                                                                        interactRect.y += EditorGUIUtility.singleLineHeight;
                                                                        interactionNoSerialized.global_BooleanValue = EditorGUI.Toggle(interactRect, "value to set", interactionNoSerialized.global_BooleanValue);
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    interactionNoSerialized.global_compareBooleanValue = EditorGUI.Toggle(interactRect, "compare boolean value", interactionNoSerialized.global_compareBooleanValue);
                                                                    if (interactionNoSerialized.global_compareBooleanValue)
                                                                    {
                                                                        interactRect.y += EditorGUIUtility.singleLineHeight;
                                                                        interactionNoSerialized.global_BooleanValue = EditorGUI.Toggle(interactRect, "value to compare", interactionNoSerialized.global_BooleanValue);
                                                                        interactRect.y += EditorGUIUtility.singleLineHeight;
                                                                        interactionNoSerialized.global_defaultBooleanValue = EditorGUI.Toggle(interactRect, "default value", interactionNoSerialized.global_defaultBooleanValue);
                                                                    }
                                                                }
                                                            }
                                                            if (interactionNoSerialized.variableObject.global_variables[index].properties.variable_type.HasFlag(GlobalVariableProperty.variable_types.integer))
                                                            {
                                                                if (interactionNoSerialized.variablesAction == Interaction.VariablesAction.setGlobalVariable)
                                                                {
                                                                    interactRect.y += EditorGUIUtility.singleLineHeight;
                                                                    interactionNoSerialized.global_changeIntegerValue = EditorGUI.Toggle(interactRect, "change integer value", interactionNoSerialized.global_changeIntegerValue);
                                                                    if (interactionNoSerialized.global_changeIntegerValue)
                                                                    {
                                                                        interactRect.y += EditorGUIUtility.singleLineHeight;
                                                                        interactionNoSerialized.global_IntegerValue = EditorGUI.IntField(interactRect, "value", interactionNoSerialized.global_IntegerValue);
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    interactRect.y += EditorGUIUtility.singleLineHeight;
                                                                    interactionNoSerialized.global_compareIntegerValue = EditorGUI.Toggle(interactRect, "compare integer value", interactionNoSerialized.global_compareIntegerValue);
                                                                    if (interactionNoSerialized.global_compareIntegerValue)
                                                                    {
                                                                        interactRect.y += EditorGUIUtility.singleLineHeight;
                                                                        interactionNoSerialized.global_IntegerValue = EditorGUI.IntField(interactRect, "value to compare", interactionNoSerialized.global_IntegerValue);
                                                                        interactRect.y += EditorGUIUtility.singleLineHeight;
                                                                        interactionNoSerialized.global_defaultIntegerValue = EditorGUI.IntField(interactRect, "default value", interactionNoSerialized.global_defaultIntegerValue);
                                                                    }
                                                                }
                                                            }
                                                            if (interactionNoSerialized.variableObject.global_variables[index].properties.variable_type.HasFlag(GlobalVariableProperty.variable_types.String))
                                                            {
                                                                if (interactionNoSerialized.variablesAction == Interaction.VariablesAction.setGlobalVariable)
                                                                {
                                                                    interactRect.y += EditorGUIUtility.singleLineHeight;
                                                                    interactionNoSerialized.global_changeStringValue = EditorGUI.Toggle(interactRect, "change string value", interactionNoSerialized.global_changeStringValue);
                                                                    if (interactionNoSerialized.global_changeStringValue)
                                                                    {
                                                                        interactRect.y += EditorGUIUtility.singleLineHeight;
                                                                        interactionNoSerialized.global_StringValue = EditorGUI.TextField(interactRect, "value", interactionNoSerialized.global_StringValue);
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    interactRect.y += EditorGUIUtility.singleLineHeight;
                                                                    interactionNoSerialized.global_compareStringValue = EditorGUI.Toggle(interactRect, "compare string value", interactionNoSerialized.global_compareStringValue);
                                                                    if (interactionNoSerialized.global_compareStringValue)
                                                                    {
                                                                        interactRect.y += EditorGUIUtility.singleLineHeight;
                                                                        interactionNoSerialized.global_StringValue = EditorGUI.TextField(interactRect, "value to compare", interactionNoSerialized.global_StringValue);
                                                                        interactRect.y += EditorGUIUtility.singleLineHeight;
                                                                        interactionNoSerialized.global_defaultStringValue = EditorGUI.TextField(interactRect, "default value", interactionNoSerialized.global_defaultStringValue);
                                                                    }
                                                                }
                                                            }
                                                            if (interactionNoSerialized.variablesAction == Interaction.VariablesAction.getGlobalVariable &&
                                                                    (interactionNoSerialized.global_compareBooleanValue ||
                                                                    interactionNoSerialized.global_compareIntegerValue ||
                                                                    interactionNoSerialized.global_compareStringValue))
                                                            {
                                                                interactRect.y += EditorGUIUtility.singleLineHeight;
                                                                interactionNoSerialized.OnCompareResultTrueAction = (Conditional.GetVariableAction)EditorGUI.EnumPopup(interactRect, "action if value/s match", (System.Enum)interactionNoSerialized.OnCompareResultTrueAction);
                                                                interactRect.y += EditorGUIUtility.singleLineHeight;
                                                                if (interactionNoSerialized.OnCompareResultTrueAction == Conditional.GetVariableAction.GoToSpecificLine)
                                                                {
                                                                    interactionNoSerialized.LineToGoOnTrueResult = EditorGUI.Popup(interactRect, "line to go", interactionNoSerialized.LineToGoOnTrueResult, PNCEditorUtils.GetInteractionsText(noSerializedAttemps[indexA].interactions));
                                                                    interactRect.y += EditorGUIUtility.singleLineHeight;
                                                                }
                                                                interactionNoSerialized.OnCompareResultFalseAction = (Conditional.GetVariableAction)EditorGUI.EnumPopup(interactRect, "action if value/s doesn't match", (System.Enum)interactionNoSerialized.OnCompareResultFalseAction);
                                                                interactRect.y += EditorGUIUtility.singleLineHeight;
                                                                if (interactionNoSerialized.OnCompareResultFalseAction == Conditional.GetVariableAction.GoToSpecificLine)
                                                                {
                                                                    interactionNoSerialized.LineToGoOnFalseResult = EditorGUI.Popup(interactRect, "line to go", interactionNoSerialized.LineToGoOnFalseResult, PNCEditorUtils.GetInteractionsText(noSerializedAttemps[indexA].interactions));
                                                                }
                                                            }
                                                        }

                                                    }
                                                }
                                                else if (interactionNoSerialized.variablesAction == Interaction.VariablesAction.getLocalVariable ||
                                                    interactionNoSerialized.variablesAction == Interaction.VariablesAction.setLocalVariable)
                                                {
                                                    if (interactionNoSerialized.variableObject)
                                                    {
                                                        InteractuableLocalVariable[] variables = interactionNoSerialized.variableObject.local_variables;
                                                        string[] content = new string[variables.Length];

                                                        for (int z = 0; z < variables.Length; z++)
                                                        {
                                                            content[z] = interactionNoSerialized.variableObject.local_variables[z].name;
                                                        }
                                                        interactionNoSerialized.localVariableSelected = EditorGUI.Popup(interactRect, "Variable", interactionNoSerialized.localVariableSelected, content);
                                                        int index = interactionNoSerialized.localVariableSelected;
                                                        if (interactionNoSerialized.variableObject.local_variables.Length > index)
                                                        {
                                                            if (interactionNoSerialized.variableObject.local_variables[index].hasBoolean)
                                                            {
                                                                if (interactionNoSerialized.variablesAction == Interaction.VariablesAction.setLocalVariable)
                                                                {
                                                                    interactRect.y += EditorGUIUtility.singleLineHeight;
                                                                    interactionNoSerialized.local_changeBooleanValue = EditorGUI.Toggle(interactRect, "change boolean value", interactionNoSerialized.local_changeBooleanValue);
                                                                    if (interactionNoSerialized.local_changeBooleanValue)
                                                                    {
                                                                        interactRect.y += EditorGUIUtility.singleLineHeight;
                                                                        interactionNoSerialized.local_BooleanValue = EditorGUI.Toggle(interactRect, "value", interactionNoSerialized.local_BooleanValue);
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    interactRect.y += EditorGUIUtility.singleLineHeight;
                                                                    interactionNoSerialized.local_compareBooleanValue = EditorGUI.Toggle(interactRect, "compare integer value", interactionNoSerialized.local_compareBooleanValue);
                                                                    if (interactionNoSerialized.local_compareBooleanValue)
                                                                    {
                                                                        interactRect.y += EditorGUIUtility.singleLineHeight;
                                                                        interactionNoSerialized.local_BooleanValue = EditorGUI.Toggle(interactRect, "value to compare", interactionNoSerialized.local_BooleanValue);
                                                                        interactRect.y += EditorGUIUtility.singleLineHeight;
                                                                        interactionNoSerialized.local_defaultBooleanValue = EditorGUI.Toggle(interactRect, "default value", interactionNoSerialized.local_defaultBooleanValue);
                                                                    }
                                                                }
                                                            }
                                                            if (interactionNoSerialized.variableObject.local_variables[index].hasInteger)
                                                            {
                                                                if (interactionNoSerialized.variablesAction == Interaction.VariablesAction.setLocalVariable)
                                                                {
                                                                    interactRect.y += EditorGUIUtility.singleLineHeight;
                                                                    interactionNoSerialized.local_changeIntegerValue = EditorGUI.Toggle(interactRect, "change integer value", interactionNoSerialized.local_changeIntegerValue);
                                                                    if (interactionNoSerialized.local_changeIntegerValue)
                                                                    {
                                                                        interactRect.y += EditorGUIUtility.singleLineHeight;
                                                                        interactionNoSerialized.local_IntegerValue = EditorGUI.IntField(interactRect, "value", interactionNoSerialized.local_IntegerValue);
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    interactRect.y += EditorGUIUtility.singleLineHeight;
                                                                    interactionNoSerialized.local_compareIntegerValue = EditorGUI.Toggle(interactRect, "compare integer value", interactionNoSerialized.local_compareIntegerValue);
                                                                    if (interactionNoSerialized.local_compareIntegerValue)
                                                                    {
                                                                        interactRect.y += EditorGUIUtility.singleLineHeight;
                                                                        interactionNoSerialized.local_IntegerValue = EditorGUI.IntField(interactRect, "value to compare", interactionNoSerialized.local_IntegerValue);
                                                                        interactRect.y += EditorGUIUtility.singleLineHeight;
                                                                        interactionNoSerialized.local_defaultIntegerValue = EditorGUI.IntField(interactRect, "default value", interactionNoSerialized.local_defaultIntegerValue);
                                                                    }
                                                                }
                                                            }
                                                            if (interactionNoSerialized.variableObject.local_variables[index].hasString)
                                                            {
                                                                if (interactionNoSerialized.variablesAction == Interaction.VariablesAction.setLocalVariable)
                                                                {
                                                                    interactRect.y += EditorGUIUtility.singleLineHeight;
                                                                    interactionNoSerialized.local_changeStringValue = EditorGUI.Toggle(interactRect, "change string value", interactionNoSerialized.local_changeStringValue);
                                                                    if (interactionNoSerialized.local_changeStringValue)
                                                                    {
                                                                        interactRect.y += EditorGUIUtility.singleLineHeight;
                                                                        interactionNoSerialized.local_StringValue = EditorGUI.TextField(interactRect, "value", interactionNoSerialized.local_StringValue);
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    interactRect.y += EditorGUIUtility.singleLineHeight;
                                                                    interactionNoSerialized.local_compareStringValue = EditorGUI.Toggle(interactRect, "compare string value", interactionNoSerialized.local_compareStringValue);
                                                                    if (interactionNoSerialized.local_compareStringValue)
                                                                    {
                                                                        interactRect.y += EditorGUIUtility.singleLineHeight;
                                                                        interactionNoSerialized.local_StringValue = EditorGUI.TextField(interactRect, "value to compare", interactionNoSerialized.local_StringValue);
                                                                        interactRect.y += EditorGUIUtility.singleLineHeight;
                                                                        interactionNoSerialized.local_defaultStringValue = EditorGUI.TextField(interactRect, "default value", interactionNoSerialized.local_defaultStringValue);
                                                                    }
                                                                }
                                                            }
                                                            if (interactionNoSerialized.variablesAction == Interaction.VariablesAction.getLocalVariable &&
                                                                    (interactionNoSerialized.local_compareBooleanValue ||
                                                                    interactionNoSerialized.local_compareIntegerValue ||
                                                                    interactionNoSerialized.local_compareStringValue))
                                                            {
                                                                interactRect.y += EditorGUIUtility.singleLineHeight;
                                                                interactionNoSerialized.OnCompareResultTrueAction = (Conditional.GetVariableAction)EditorGUI.EnumPopup(interactRect, "action if value/s match", (System.Enum)interactionNoSerialized.OnCompareResultTrueAction);
                                                                interactRect.y += EditorGUIUtility.singleLineHeight;
                                                                if (interactionNoSerialized.OnCompareResultTrueAction == Conditional.GetVariableAction.GoToSpecificLine)
                                                                {
                                                                    interactionNoSerialized.LineToGoOnTrueResult = EditorGUI.Popup(interactRect, "line to go", interactionNoSerialized.LineToGoOnTrueResult, PNCEditorUtils.GetInteractionsText(noSerializedAttemps[indexA].interactions));
                                                                    interactRect.y += EditorGUIUtility.singleLineHeight;
                                                                }
                                                                interactionNoSerialized.OnCompareResultFalseAction = (Conditional.GetVariableAction)EditorGUI.EnumPopup(interactRect, "action if value/s doesn't match", (System.Enum)interactionNoSerialized.OnCompareResultFalseAction);
                                                                interactRect.y += EditorGUIUtility.singleLineHeight;
                                                                if (interactionNoSerialized.OnCompareResultFalseAction == Conditional.GetVariableAction.GoToSpecificLine)
                                                                {
                                                                    interactionNoSerialized.LineToGoOnFalseResult = EditorGUI.Popup(interactRect, "line to go", interactionNoSerialized.LineToGoOnFalseResult, PNCEditorUtils.GetInteractionsText(noSerializedAttemps[indexA].interactions));
                                                                }
                                                            }
                                                        }

                                                    }
                                                }
                                            }
                                            else if (interactionNoSerialized.type == Interaction.InteractionType.custom)
                                                EditorGUI.PropertyField(interactRect, interactionSerialized.FindPropertyRelative("action"));

                                        }

                                    }
                                };
                                interactionsListDict.Add(interactionKey, interactionList);
                            }
                            interactionsListDict[interactionKey].DoList(attempRect);
                        }
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
        public List<InteractionsAttemp> attemps;
    }

    static Interaction copiedInteraction;

    private static void onMouse(InteractionData interactioncopy)
    {
        GenericMenu menu = new GenericMenu();

        menu.AddItem(new GUIContent("Copy interaction"), false, Copy, interactioncopy);
        menu.AddItem(new GUIContent("Paste interaction"), false, Paste, interactioncopy);
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
    }


    public static string[] GetInteractionsText(List<Interaction> interactions)
    {
        string[] texts = new string[interactions.Count];
        for (int i = 0; i < interactions.Count; i++)
        {
            texts[i] = (i + 1) + " " + interactions[i].type.ToString();
            if (interactions[i].type == Interaction.InteractionType.character)
            {
                texts[i] += " " + interactions[i].characterAction;
            }
            if (interactions[i].type == Interaction.InteractionType.variables)
            {
                texts[i] += " " + interactions[i].variablesAction;
            }
        }

        return texts;

    }

}
