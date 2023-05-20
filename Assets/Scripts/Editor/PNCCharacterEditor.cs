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
    Interaction copiedInteraction;

    public float GetInteractionHeight(SerializedProperty interactionSerialized, Interaction interactionNoSerialized)
    {
    
        if (interactionSerialized.FindPropertyRelative("expandedInInspector").boolValue)
        {
            if (interactionNoSerialized.type == Interaction.InteractionType.character)
                return EditorGUIUtility.singleLineHeight * 5;
            if (interactionNoSerialized.type == Interaction.InteractionType.variables)
                return EditorGUIUtility.singleLineHeight * 8;
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
                                                var interactionSerializated = interactionsListDict[interactionKey].serializedProperty.GetArrayElementAtIndex(indexI);
                                                var interactionNoSerializated = myTarget.verbs[indexV].attemps[indexA].interactions[indexI];

                                                return GetInteractionHeight(interactionSerializated, interactionNoSerializated);
                                            }
                                            ,
                                            drawElementCallback = (rectI, indexI, activeI, focusI) =>
                                            {
                                                var interactionSerializated = interactionsListDict[interactionKey].serializedProperty.GetArrayElementAtIndex(indexI);
                                                var interactionNoSerializated = myTarget.verbs[indexV].attemps[indexA].interactions[indexI];
                                                var interactRect = new Rect(rectI);
                                                var interactExpanded = interactionSerializated.FindPropertyRelative("expandedInInspector");
                                                interactRect.height = EditorGUIUtility.singleLineHeight;

                                                interactExpanded.boolValue = EditorGUI.Foldout(interactRect,interactExpanded.boolValue, (indexI + 1).ToString() + "° interaction");
                                                interactRect.y += EditorGUIUtility.singleLineHeight;

                                                if (interactExpanded.boolValue)
                                                {        
                                                    EditorGUI.PropertyField(interactRect, interactionSerializated.FindPropertyRelative("type"));
                                                    interactRect.y += EditorGUIUtility.singleLineHeight;
                                                    if (interactionNoSerializated.type == Interaction.InteractionType.character)
                                                        {
                                                        EditorGUI.PropertyField(interactRect,interactionSerializated.FindPropertyRelative("character"));
                                                        interactRect.y += EditorGUIUtility.singleLineHeight;
                                                        EditorGUI.PropertyField(interactRect,interactionSerializated.FindPropertyRelative("characterAction"));
                                                        interactRect.y += EditorGUIUtility.singleLineHeight;
                                                        if (interactionNoSerializated.characterAction == Interaction.CharacterAction.say)
                                                            EditorGUI.PropertyField(interactRect, interactionSerializated.FindPropertyRelative("WhatToSay"));
                                                        else if (interactionNoSerializated.characterAction == Interaction.CharacterAction.walk)
                                                            EditorGUI.PropertyField(interactRect, interactionSerializated.FindPropertyRelative("WhereToWalk"));
                                                        }
                                                    else if (interactionNoSerializated.type == Interaction.InteractionType.variables)
                                                        {
                                                        EditorGUI.PropertyField(interactRect, interactionSerializated.FindPropertyRelative("variablesAction"));
                                                        interactRect.y += EditorGUIUtility.singleLineHeight;
                                                        EditorGUI.PropertyField(interactRect, interactionSerializated.FindPropertyRelative("variableObject"));
                                                        interactRect.y += EditorGUIUtility.singleLineHeight;
                                                        if (interactionNoSerializated.variablesAction == Interaction.VariablesAction.getGlobalVariable ||
                                                            interactionNoSerializated.variablesAction == Interaction.VariablesAction.setGlobalVariable)
                                                        {
                                                            if (interactionNoSerializated.variableObject)
                                                            {
                                                                InteractuableGlobalVariable[] variables = interactionNoSerializated.variableObject.global_variables;
                                                                string[] content = new string[variables.Length];

                                                                for (int z = 0; z < variables.Length; z++)
                                                                {
                                                                    content[z] = interactionNoSerializated.variableObject.global_variables[z].name;
                                                                }
                                                                interactionNoSerializated.globalVariableSelected = EditorGUI.Popup(interactRect, "Variable", interactionNoSerializated.globalVariableSelected, content);
                                                            }
                                                        }
                                                            else if (interactionNoSerializated.variablesAction == Interaction.VariablesAction.getLocalVariable ||
                                                                interactionNoSerializated.variablesAction == Interaction.VariablesAction.setLocalVariable)
                                                            {
                                                                if (interactionNoSerializated.variableObject)
                                                                {
                                                                    InteractuableLocalVariable[] variables = interactionNoSerializated.variableObject.local_variables;
                                                                    string[] content = new string[variables.Length];

                                                                    for (int z = 0; z < variables.Length; z++)
                                                                    {
                                                                        content[z] = interactionNoSerializated.variableObject.local_variables[z].name;
                                                                    }
                                                                    interactionNoSerializated.localVariableSelected = EditorGUI.Popup(interactRect,"Variable", interactionNoSerializated.localVariableSelected, content);
                                                                }
                                                            }
                                                        }
                                                        else if (interactionNoSerializated.type == Interaction.InteractionType.custom)
                                                            EditorGUI.PropertyField(interactRect, interactionSerializated.FindPropertyRelative("action"));

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
        
        menu.AddItem(new GUIContent("Copy"), false, Copy, interactioncopy);
        menu.AddItem(new GUIContent("Paste"), false, Paste, interactioncopy);
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
