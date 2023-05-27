using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using UnityEngine.UIElements;
using UnityEditorInternal;
[CustomEditor(typeof(PNCCharacter))]
public class PnCCharacterEditor : PNCVariablesContainerEditor
{
    SerializedProperty verbs_serialized;
    
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

        verbsList = new ReorderableList(serializedObject, verbs_serialized, true, true, false, false)
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
                var verbName = verb.FindPropertyRelative("name").stringValue;
                var verbRect = new Rect(rect);
                var verbExpanded = verb.FindPropertyRelative("expandedInInspector");
                verbRect.x += 8;

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
                                                                            interactionNoSerialized.LineToGoOnTrueResult = EditorGUI.IntField(interactRect, "line to go", interactionNoSerialized.LineToGoOnTrueResult);
                                                                            interactRect.y += EditorGUIUtility.singleLineHeight;
                                                                        }
                                                                        interactionNoSerialized.OnCompareResultFalseAction = (Conditional.GetVariableAction)EditorGUI.EnumPopup(interactRect, "action if value/s doesn't match", (System.Enum)interactionNoSerialized.OnCompareResultFalseAction);
                                                                        interactRect.y += EditorGUIUtility.singleLineHeight;
                                                                        if (interactionNoSerialized.OnCompareResultFalseAction == Conditional.GetVariableAction.GoToSpecificLine)
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
                                                                                interactRect.y += EditorGUIUtility.singleLineHeight;
                                                                                interactionNoSerialized.local_defaultBooleanValue = EditorGUI.Toggle(interactRect, "default value", interactionNoSerialized.local_defaultBooleanValue);
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
                                                                                interactRect.y += EditorGUIUtility.singleLineHeight;
                                                                                interactionNoSerialized.local_defaultIntegerValue = EditorGUI.IntField(interactRect, "default value", interactionNoSerialized.local_defaultIntegerValue);
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
                                                                            interactionNoSerialized.LineToGoOnTrueResult = EditorGUI.IntField(interactRect, "line to go", interactionNoSerialized.LineToGoOnTrueResult);
                                                                            interactRect.y += EditorGUIUtility.singleLineHeight;
                                                                        }
                                                                        interactionNoSerialized.OnCompareResultFalseAction = (Conditional.GetVariableAction)EditorGUI.EnumPopup(interactRect, "action if value/s doesn't match", (System.Enum)interactionNoSerialized.OnCompareResultFalseAction);
                                                                        interactRect.y += EditorGUIUtility.singleLineHeight;
                                                                        if (interactionNoSerialized.OnCompareResultFalseAction == Conditional.GetVariableAction.GoToSpecificLine)
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



        bool verbAdded = false;
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
                verbAdded = true;
                Verb tempVerb = new Verb();
                tempVerb.name = settings.verbs[i];
                tempVerb.attemps = new List<InteractionsAttemp>();
                interactionsTempList.Add(tempVerb);
            }
        }

        bool verbAdded2 = false;
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
                verbAdded2 = true;
                interactionsTempList.Add(myTarget.verbs[i]);
            }
        }

        if(verbAdded || verbAdded2)
            myTarget.verbs = interactionsTempList;

        InitializeGlobalVariables(GlobalVariableProperty.object_types.characters);

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
        EditorGUILayout.PropertyField(serializedObject.FindProperty("name"));
        
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

        if (settings.speechStyle == Settings.SpeechStyle.Sierra)
        { 
            GUILayout.Box(myTarget.SierraTextFace.texture,GUILayout.MaxHeight(100),GUILayout.MaxWidth(100),GUILayout.MinHeight(100),GUILayout.MinWidth(100));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("SierraTextFace"));
        }
                

        ShowLocalVariables(ref myTarget.local_variables, ref local_variables_serialized);
        
        ShowGlobalVariables(GlobalVariableProperty.object_types.characters, ref myTarget.global_variables, ref global_variables_serialized);

        serializedObject.ApplyModifiedProperties();

        if(GUI.changed)
        {
            EditorUtility.SetDirty(target);
            EditorUtility.SetDirty(settings);
        }

    }

}
