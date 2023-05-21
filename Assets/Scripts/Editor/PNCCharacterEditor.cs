using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using UnityEngine.UIElements;
using UnityEditorInternal;
[CustomEditor(typeof(PNCCharacter))]
public class PnCCharacterEditor : Editor
{
    Settings settings;
    SerializedProperty verbs_serialized;
    SerializedProperty local_variables_serialized;
    SerializedProperty global_variables_serialized;

    ReorderableList verbsList;
    Dictionary<string,ReorderableList> attempsListDict = new Dictionary<string, ReorderableList>();
    Dictionary<string, ReorderableList> interactionsListDict = new Dictionary<string, ReorderableList>();


    [System.Serializable]
    public class InteractionData
    {
        public int indexV;
        public int indexA;
        public ReorderableList list;
    }
    static Interaction copiedInteraction;

    public float GetInteractionHeight(SerializedProperty interactionSerialized, Interaction interactionNoSerialized)
    {
    
        if (interactionSerialized.FindPropertyRelative("expandedInInspector").boolValue)
        {
            if (interactionNoSerialized.type == Interaction.InteractionType.character)
                return EditorGUIUtility.singleLineHeight * 5.25f;
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
                                if(interactionNoSerialized.global_changeIntegerValue)
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
                                    height += 1;
                                if (interactionNoSerialized.global_compareIntegerValue)
                                    height += 1;
                                if (interactionNoSerialized.global_compareStringValue)
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
                            if (interactionNoSerialized.variableObject.local_variables[index].type.HasFlag(InteractuableLocalVariable.types.boolean))
                            { 
                                height += 1;
                                if (interactionNoSerialized.local_changeBooleanValue)
                                    height += 1;
                            }
                            if (interactionNoSerialized.variableObject.local_variables[index].type.HasFlag(InteractuableLocalVariable.types.integer))
                            { 
                                height += 1;
                                if (interactionNoSerialized.local_changeIntegerValue)
                                    height += 1;
                            }
                            if (interactionNoSerialized.variableObject.local_variables[index].type.HasFlag(InteractuableLocalVariable.types.String))
                            { 
                                height += 1;
                                if (interactionNoSerialized.local_changeStringValue)
                                    height += 1;
                            }
                            if (interactionNoSerialized.variablesAction == Interaction.VariablesAction.getLocalVariable)
                            {
                                if (interactionNoSerialized.local_compareBooleanValue)
                                    height += 1;
                                if (interactionNoSerialized.local_compareIntegerValue)
                                    height += 1;
                                if (interactionNoSerialized.local_compareStringValue)
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

    public float GetAttempHeight(SerializedProperty attempSerialized, InteractionsAttemp attempNoSerialized)
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

    public void OnEnable()
    {
        PNCCharacter myTarget = (PNCCharacter)target;

        verbs_serialized = serializedObject.FindProperty("verbs");

        settings = Resources.Load<Settings>("Settings/Settings");

        verbsList = new ReorderableList(serializedObject, serializedObject.FindProperty("verbs"), true, true, false, false)
        {
            drawHeaderCallback = (rect) =>
            {
                EditorGUI.LabelField(rect, "verbs");
            },
            elementHeightCallback = (int indexV) =>
            {
                if (serializedObject.FindProperty("verbs").GetArrayElementAtIndex(indexV).FindPropertyRelative("expandedInInspector").boolValue)
                {
                    float heightM = 5 * EditorGUIUtility.singleLineHeight;
                    var attemps = serializedObject.FindProperty("verbs").GetArrayElementAtIndex(indexV).FindPropertyRelative("attemps");
                    for (int i = 0; i < attemps.arraySize; i++)
                    {
                        heightM += GetAttempHeight(attemps.GetArrayElementAtIndex(i), myTarget.verbs[indexV].attemps[i]);
                        
                    }
                    return heightM;
                }
                return EditorGUIUtility.singleLineHeight;
            },
            drawElementCallback = (rect, indexV, active, focus) =>
            {
                var verb = verbs_serialized.GetArrayElementAtIndex(indexV);
                var attemps = verb.FindPropertyRelative("attemps");
                var verbName = settings.verbs[indexV];
                var verbRect = new Rect(rect);
                var verbExpanded = verb.FindPropertyRelative("expandedInInspector");

                verbExpanded.boolValue = EditorGUI.Foldout(new Rect(verbRect.x, verbRect.y, verbRect.width, EditorGUIUtility.singleLineHeight), verbExpanded.boolValue, verbName);

                verbRect.y += EditorGUIUtility.singleLineHeight;

                if (verbExpanded.boolValue)
                { 
                    var attempKey = verb.propertyPath;

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
                                return GetAttempHeight(serializedObject.FindProperty("verbs").GetArrayElementAtIndex(indexV).FindPropertyRelative("attemps").GetArrayElementAtIndex(indexA), myTarget.verbs[indexV].attemps[indexA]);
                            },
                            drawElementCallback = (rectA, indexA, activeA, focusA) =>
                            {
                                var attemp = attempsListDict[attempKey].serializedProperty.GetArrayElementAtIndex(indexA);

                                var interactionKey = attemp.propertyPath;
                                var interactions = attemp.FindPropertyRelative("interactions");
                                var attempRect = new Rect(rectA);
                                var attempExpanded = attemp.FindPropertyRelative("expandedInInspector");

                                attempExpanded.boolValue = EditorGUI.Foldout(new Rect(attempRect.x, attempRect.y, attempRect.width, EditorGUIUtility.singleLineHeight),attempExpanded.boolValue, (indexA+1).ToString() + "° attemp");
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
                                                data.indexV = indexV;
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
                                                var interactionNoSerialized = myTarget.verbs[indexV].attemps[indexA].interactions[indexI];

                                                return GetInteractionHeight(interactionSerialized, interactionNoSerialized);
                                            }
                                            ,
                                            drawElementCallback = (rectI, indexI, activeI, focusI) =>
                                            {
                                                var interactionSerialized = interactionsListDict[interactionKey].serializedProperty.GetArrayElementAtIndex(indexI);
                                                var interactionNoSerialized = myTarget.verbs[indexV].attemps[indexA].interactions[indexI];
                                                var interactRect = new Rect(rectI);
                                                var interactExpanded = interactionSerialized.FindPropertyRelative("expandedInInspector");
                                                interactRect.height = EditorGUIUtility.singleLineHeight;

                                                interactExpanded.boolValue = EditorGUI.Foldout(interactRect,interactExpanded.boolValue, (indexI + 1).ToString() + "° interaction");
                                                interactRect.y += EditorGUIUtility.singleLineHeight;

                                                if (interactExpanded.boolValue)
                                                {        
                                                    EditorGUI.PropertyField(interactRect, interactionSerialized.FindPropertyRelative("type"));
                                                    interactRect.y += EditorGUIUtility.singleLineHeight;
                                                    if (interactionNoSerialized.type == Interaction.InteractionType.character)
                                                        {
                                                        EditorGUI.PropertyField(interactRect,interactionSerialized.FindPropertyRelative("character"));
                                                        interactRect.y += EditorGUIUtility.singleLineHeight;
                                                        EditorGUI.PropertyField(interactRect,interactionSerialized.FindPropertyRelative("characterAction"));
                                                        interactRect.y += EditorGUIUtility.singleLineHeight;
                                                        if (interactionNoSerialized.characterAction == Interaction.CharacterAction.say)
                                                            EditorGUI.PropertyField(interactRect, interactionSerialized.FindPropertyRelative("WhatToSay"));
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
                                                                            }
                                                                        }
                                                                    }
                                                                    if (interactionNoSerialized.variablesAction == Interaction.VariablesAction.getGlobalVariable &&
                                                                            (interactionNoSerialized.global_compareBooleanValue ||
                                                                            interactionNoSerialized.global_compareIntegerValue ||
                                                                            interactionNoSerialized.global_compareStringValue))
                                                                    {
                                                                        interactRect.y += EditorGUIUtility.singleLineHeight;
                                                                        interactionNoSerialized.OnCompareResultTrueAction = (Interaction.GetVariableAction)EditorGUI.EnumPopup(interactRect, "action if value/s match", (System.Enum)interactionNoSerialized.OnCompareResultTrueAction);
                                                                        interactRect.y += EditorGUIUtility.singleLineHeight;
                                                                        if (interactionNoSerialized.OnCompareResultTrueAction == Interaction.GetVariableAction.GoToSpecificLine)
                                                                        {
                                                                            interactionNoSerialized.LineToGoOnTrueResult = EditorGUI.IntField(interactRect, "line to go", interactionNoSerialized.LineToGoOnTrueResult);
                                                                            interactRect.y += EditorGUIUtility.singleLineHeight;
                                                                        }
                                                                        interactionNoSerialized.OnCompareResultFalseAction = (Interaction.GetVariableAction)EditorGUI.EnumPopup(interactRect, "action if value/s doesn't match", (System.Enum)interactionNoSerialized.OnCompareResultFalseAction);
                                                                        interactRect.y += EditorGUIUtility.singleLineHeight;
                                                                        if (interactionNoSerialized.OnCompareResultFalseAction == Interaction.GetVariableAction.GoToSpecificLine)
                                                                        {
                                                                            interactionNoSerialized.LineToGoOnFalseResult = EditorGUI.IntField(interactRect, "line to go", interactionNoSerialized.LineToGoOnFalseResult);
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
                                                                interactionNoSerialized.localVariableSelected = EditorGUI.Popup(interactRect,"Variable", interactionNoSerialized.localVariableSelected, content);
                                                                int index = interactionNoSerialized.localVariableSelected;
                                                                if (interactionNoSerialized.variableObject.local_variables.Length > index)
                                                                {
                                                                    if (interactionNoSerialized.variableObject.local_variables[index].type.HasFlag(InteractuableLocalVariable.types.boolean))
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
                                                                            }
                                                                        }
                                                                    }
                                                                    if (interactionNoSerialized.variableObject.local_variables[index].type.HasFlag(InteractuableLocalVariable.types.integer))
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
                                                                            }
                                                                        }
                                                                    }
                                                                    if (interactionNoSerialized.variableObject.local_variables[index].type.HasFlag(InteractuableLocalVariable.types.String))
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
                                                                                interactionNoSerialized.local_StringValue= EditorGUI.TextField(interactRect, "value to compare", interactionNoSerialized.local_StringValue);
                                                                            }
                                                                        }
                                                                    }
                                                                    if (interactionNoSerialized.variablesAction == Interaction.VariablesAction.getLocalVariable &&
                                                                            (interactionNoSerialized.local_compareBooleanValue ||
                                                                            interactionNoSerialized.local_compareIntegerValue ||
                                                                            interactionNoSerialized.local_compareStringValue))
                                                                    {
                                                                        interactRect.y += EditorGUIUtility.singleLineHeight;
                                                                        interactionNoSerialized.OnCompareResultTrueAction = (Interaction.GetVariableAction)EditorGUI.EnumPopup(interactRect, "action if value/s match", (System.Enum)interactionNoSerialized.OnCompareResultTrueAction);
                                                                        interactRect.y += EditorGUIUtility.singleLineHeight;
                                                                        if (interactionNoSerialized.OnCompareResultTrueAction == Interaction.GetVariableAction.GoToSpecificLine)
                                                                        {
                                                                            interactionNoSerialized.LineToGoOnTrueResult = EditorGUI.IntField(interactRect, "line to go", interactionNoSerialized.LineToGoOnTrueResult);
                                                                            interactRect.y += EditorGUIUtility.singleLineHeight;
                                                                        }
                                                                        interactionNoSerialized.OnCompareResultFalseAction = (Interaction.GetVariableAction)EditorGUI.EnumPopup(interactRect, "action if value/s doesn't match", (System.Enum)interactionNoSerialized.OnCompareResultFalseAction);
                                                                        interactRect.y += EditorGUIUtility.singleLineHeight;
                                                                        if (interactionNoSerialized.OnCompareResultFalseAction == Interaction.GetVariableAction.GoToSpecificLine)
                                                                        {
                                                                            interactionNoSerialized.LineToGoOnFalseResult = EditorGUI.IntField(interactRect, "line to go", interactionNoSerialized.LineToGoOnFalseResult);
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
        };

           


        List<Verb> interactionsTempList = new List<Verb>();
        for (int i = 0; i < settings.verbs.Length; i++)
        {
            bool founded = false;
            for (int j = 0; j < myTarget.verbs.Count; j++)
            {
                if (myTarget.verbs[j].name == settings.verbs[i])
                { 
                    interactionsTempList.Add((myTarget).verbs[j]);
                    founded = true;
                }
            }
            if (founded == false)
            {
                Verb tempVerb = new Verb();
                tempVerb.name = settings.verbs[i];
                tempVerb.attemps = new List<InteractionsAttemp>();
                interactionsTempList.Add(tempVerb);
            }
        }
        
        for (int i = 0; i < myTarget.verbs.Count; i++)
        {
            bool contains = false;            
            for (int j = 0; j < interactionsTempList.Count; j++)
            {
                if (myTarget.verbs[i].name == interactionsTempList[j].name)
                {
                    contains = true;
                }
            }
            if(contains == false)
            {
                interactionsTempList.Add(myTarget.verbs[i]);
            }
        }

        myTarget.verbs = interactionsTempList;

        List<InteractuableGlobalVariable> tempGlobalVarList = new List<InteractuableGlobalVariable>();
        for (int i = 0; i < settings.global_variables.Length; i++)
        {
            bool founded = false;
            for (int j = 0; j < myTarget.global_variables.Length; j++)
            {
                if ((settings.global_variables[i].ID == -1 && myTarget.global_variables[j].name == settings.global_variables[i].name)|| 
                    (settings.global_variables[i].ID != -1 && myTarget.global_variables[j].globalHashCode == -1 && myTarget.global_variables[j].name == settings.global_variables[i].name) ||
                    (settings.global_variables[i].ID != -1 && myTarget.global_variables[j].globalHashCode != -1 && myTarget.global_variables[j].globalHashCode == settings.global_variables[i].ID))
                {
                    myTarget.global_variables[j].name = settings.global_variables[i].name;
                    if (settings.global_variables[i].ID == -1)
                    { 
                        settings.global_variables[i].ID = myTarget.global_variables[j].GetHashCode();
                        myTarget.global_variables[j].globalHashCode = myTarget.global_variables[j].GetHashCode();
                    }
                    else if(myTarget.global_variables[j].globalHashCode == -1)
                    {
                        myTarget.global_variables[j].globalHashCode = settings.global_variables[i].ID;
                    }
                    
                    myTarget.global_variables[j].properties = settings.global_variables[i];

                    if (settings.global_variables[i].object_type.HasFlag((GlobalVariableProperty.object_types.characters)))
                        tempGlobalVarList.Add(myTarget.global_variables[j]);
                    founded = true;
                }
            }
            if (founded == false)
            {
                InteractuableGlobalVariable tempVerb = new InteractuableGlobalVariable();
                tempVerb.name = settings.global_variables[i].name;
                if(settings.global_variables[i].ID == -1)
                { 
                    settings.global_variables[i].ID = tempVerb.GetHashCode();
                    tempVerb.globalHashCode = tempVerb.GetHashCode();
                }
                else
                {
                    tempVerb.globalHashCode = settings.global_variables[i].ID;
                }
                tempVerb.properties = settings.global_variables[i];
                if (settings.global_variables[i].object_type.HasFlag((GlobalVariableProperty.object_types.characters)))
                    tempGlobalVarList.Add(tempVerb);
            }
        }

        myTarget.global_variables = tempGlobalVarList.ToArray();

        local_variables_serialized = serializedObject.FindProperty("local_variables");
        global_variables_serialized = serializedObject.FindProperty("global_variables");

    }

    private void onMouse(InteractionData interactioncopy)
    {
        GenericMenu menu = new GenericMenu();
        
        menu.AddItem(new GUIContent("Copy interaction"), false, Copy, interactioncopy);
        menu.AddItem(new GUIContent("Paste interaction"), false, Paste, interactioncopy);
        menu.AddItem(new GUIContent("Cancel"), false, Cancel);

        menu.ShowAsContext();
    }

    private void Cancel()
    {
    }

    
    private void Copy(object interaction)
    {
        copiedInteraction = ((PNCCharacter)target).verbs[((InteractionData)interaction).indexV].attemps[((InteractionData)interaction).indexA].interactions[((InteractionData)interaction).list.index];
    }

    private void Paste(object interaction)
    {
        if (copiedInteraction == null) return;

        copiedInteraction.Copy(((PNCCharacter)target).verbs[((InteractionData)interaction).indexV].attemps[((InteractionData)interaction).indexA].interactions[((InteractionData)interaction).list.index]);
    }

    public override void OnInspectorGUI()
    {
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Label("Interactions", EditorStyles.boldLabel);
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        PNCCharacter myTarget = (PNCCharacter)target;

        verbsList.DoLayoutList();
        if (GUILayout.Button("Edit verbs"))
        {
            Selection.objects = new UnityEngine.Object[] { settings };
            EditorGUIUtility.PingObject(settings);
        }

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Label("Local Variables", EditorStyles.boldLabel);
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        ShowLocalVariables(ref myTarget.local_variables, ref local_variables_serialized);

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Label("Global Variables", EditorStyles.boldLabel);
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        ShowGlobalVariables(ref myTarget.global_variables, ref global_variables_serialized);

        serializedObject.ApplyModifiedProperties();

        if(GUI.changed)
        {
            EditorUtility.SetDirty(target);
            EditorUtility.SetDirty(settings);
        }

    }

    public void ShowLocalVariables(ref InteractuableLocalVariable[] variables, ref SerializedProperty variables_serialized)
    {
        for (int i = 0; i < variables.Length; i++)
        {
            variables[i].expandedInInspector = EditorGUILayout.Foldout(variables[i].expandedInInspector, variables[i].name);

            if (variables[i].expandedInInspector)
            {
                EditorGUILayout.BeginVertical("GroupBox");

                variables[i].name = EditorGUILayout.TextField("name:", variables[i].name);

                variables[i].type = (InteractuableLocalVariable.types)EditorGUILayout.EnumFlagsField("types:", variables[i].type);

                if (variables[i].type.HasFlag(InteractuableLocalVariable.types.integer))
                {
                    if (!variables[i].integerDefault)
                    {
                        variables[i].integer = EditorGUILayout.IntField("integer value:", variables[i].integer);
                        if (GUILayout.Button("Set integer default value"))
                            variables[i].integerDefault = true;
                    }
                    else
                    {
                        GUILayout.Label("integer value : default", EditorStyles.boldLabel);
                        if (GUILayout.Button("Set integer value"))
                        {
                            variables[i].integer = 0;
                            variables[i].integerDefault = false;
                        }
                    }
                }
                if (variables[i].type.HasFlag(InteractuableLocalVariable.types.boolean))
                {
                    if (!variables[i].booleanDefault)
                    {
                        variables[i].boolean = EditorGUILayout.Toggle("boolean value:", variables[i].boolean);
                        if (GUILayout.Button("Set boolean default value"))
                            variables[i].booleanDefault = true;
                    }
                    else
                    {
                        GUILayout.Label("boolean value : default", EditorStyles.boldLabel);
                        if (GUILayout.Button("Set boolean value"))
                        {
                            variables[i].boolean = false;
                            variables[i].booleanDefault = false;
                        }
                    }
                }
                if (variables[i].type.HasFlag(InteractuableLocalVariable.types.String))
                {
                    if (!variables[i].stringDefault)
                    {
                        variables[i].String = EditorGUILayout.TextField("string value:", variables[i].String);
                        if (GUILayout.Button("Set string default value"))
                            variables[i].stringDefault = true;
                    }
                    else
                    {
                        GUILayout.Label("string value : default", EditorStyles.boldLabel);
                        if (GUILayout.Button("Set string value"))
                        {
                            variables[i].String = "";
                            variables[i].stringDefault = false;
                        }
                    }
                }

                if (GUILayout.Button("Delete " + variables[i].name))
                {
                    variables_serialized.DeleteArrayElementAtIndex(i);
                }

                EditorGUILayout.EndVertical();
            }
        }

        if (GUILayout.Button("Create local variable"))
        {
            InteractuableLocalVariable newvar = new InteractuableLocalVariable();
            //serializedObject.ApplyModifiedProperties();
            
            //variables.arraySize++;

            variables = variables.Append<InteractuableLocalVariable>(newvar).ToArray();
        }

        var group = variables.GroupBy(vari => vari.name, (vari) => new { Count = vari.name.Count() });
        bool repeated = false;

        foreach (var vari in group)
        {
            if (vari.Count() > 1)
                repeated = true;

        }
        if (repeated)
            GUILayout.Label("There are more than one variable with the same name", EditorStyles.boldLabel);

    }

    public void ShowGlobalVariables(ref InteractuableGlobalVariable[] variables, ref SerializedProperty variables_serialized)
    {
        for (int i = 0; i < variables.Length; i++)
        {
            bool areType = true;

            for (int j = 0; j < settings.global_variables.Length; j++)
            {
                if (variables[i].globalHashCode != -1 && settings.global_variables[j].ID == variables[i].globalHashCode)
                {
                    variables[i].name = settings.global_variables[j].name;
                    if (!settings.global_variables[j].object_type.HasFlag(GlobalVariableProperty.object_types.characters))
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
                    if (!variables[i].properties.integerDefault)
                    {
                        variables[i].integer = EditorGUILayout.IntField("integer value:", variables[i].integer);
                        if (GUILayout.Button("Set integer default value"))
                            variables[i].properties.integerDefault = true;
                    }
                    else
                    {
                        GUILayout.Label("integer value : default", EditorStyles.boldLabel);
                        if (GUILayout.Button("Set integer value"))
                        {
                            variables[i].integer = 0;
                            variables[i].properties.integerDefault = false;
                        }
                    }
                }
                if (variables[i].properties.variable_type.HasFlag(GlobalVariableProperty.variable_types.boolean))
                {
                    if (!variables[i].properties.booleanDefault)
                    {
                        variables[i].boolean = EditorGUILayout.Toggle("boolean value:", variables[i].boolean);
                        if (GUILayout.Button("Set boolean default value"))
                            variables[i].properties.booleanDefault = true;
                    }
                    else
                    {
                        GUILayout.Label("boolean value : default", EditorStyles.boldLabel);
                        if (GUILayout.Button("Set boolean value"))
                        {
                            variables[i].boolean = false;
                            variables[i].properties.booleanDefault = false;
                        }
                    }
                }
                if (variables[i].properties.variable_type.HasFlag(GlobalVariableProperty.variable_types.String))
                {
                    if (!variables[i].properties.stringDefault)
                    {
                        variables[i].String = EditorGUILayout.TextField("string value:", variables[i].String);
                        if (GUILayout.Button("Set string default value"))
                            variables[i].properties.stringDefault = true;
                    }
                    else
                    {
                        GUILayout.Label("string value : default", EditorStyles.boldLabel);
                        if (GUILayout.Button("Set string value"))
                        {
                            variables[i].String = "";
                            variables[i].properties.stringDefault = false;
                        }
                    }
                }

                GUILayout.EndVertical();
            }
        }

        if(GUILayout.Button("Edit global variables"))
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
            GUILayout.Label("There are more than one variable with the same name", EditorStyles.boldLabel);

    }
}
