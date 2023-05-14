using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using UnityEngine.UIElements;

[CustomEditor(typeof(PNCCharacter))]
public class PnCCharacterEditor : Editor
{
    Settings settings;
    SerializedProperty modes_serialized;
    SerializedProperty local_variables_serialized;
    SerializedProperty global_variables_serialized;

        

    public void OnEnable()
    {
        PNCCharacter myTarget = (PNCCharacter)target;

        modes_serialized = serializedObject.FindProperty("modes");
        settings = Resources.Load<Settings>("Settings/Settings");

        List<Mode> interactionsTempList = new List<Mode>();
        for (int i = 0; i < settings.modes.Length; i++)
        {
            bool founded = false;
            for (int j = 0; j < myTarget.modes.Length; j++)
            {
                if (myTarget.modes[j].name == settings.modes[i])
                { 
                    interactionsTempList.Add((myTarget).modes[j]);
                    founded = true;
                }
            }
            if (founded == false)
            {
                Mode tempMode = new Mode();
                tempMode.name = settings.modes[i];
                tempMode.attemps = new InteractionsAttemp[0];
                interactionsTempList.Add(tempMode);
            }
        }
        
        for (int i = 0; i < myTarget.modes.Length; i++)
        {
            bool contains = false;            
            for (int j = 0; j < interactionsTempList.Count; j++)
            {
                if (myTarget.modes[i].name == interactionsTempList[j].name)
                {
                    contains = true;
                }
            }
            if(contains == false)
            {
                interactionsTempList.Add(myTarget.modes[i]);
            }
        }

        myTarget.modes = interactionsTempList.ToArray();

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
                InteractuableGlobalVariable tempMode = new InteractuableGlobalVariable();
                tempMode.name = settings.global_variables[i].name;
                if(settings.global_variables[i].ID == -1)
                { 
                    settings.global_variables[i].ID = tempMode.GetHashCode();
                    tempMode.globalHashCode = tempMode.GetHashCode();
                }
                else
                {
                    tempMode.globalHashCode = settings.global_variables[i].ID;
                }
                tempMode.properties = settings.global_variables[i];
                if (settings.global_variables[i].object_type.HasFlag((GlobalVariableProperty.object_types.characters)))
                    tempGlobalVarList.Add(tempMode);
            }
        }

        myTarget.global_variables = tempGlobalVarList.ToArray();

        local_variables_serialized = serializedObject.FindProperty("local_variables");
        global_variables_serialized = serializedObject.FindProperty("global_variables");

    }



    public override void OnInspectorGUI()
    {
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Label("Interactions", EditorStyles.boldLabel);
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        PNCCharacter myTarget = (PNCCharacter)target;


        for (int i = 0; i < settings.modes.Length; i++)
        {
            myTarget.modes[i].expandedInInspector = EditorGUILayout.Foldout(myTarget.modes[i].expandedInInspector, settings.modes[i].ToString());
            if (myTarget.modes[i].expandedInInspector)
            {
                EditorGUILayout.BeginVertical("HelpBox");
                SerializedProperty mode_attemps = modes_serialized.GetArrayElementAtIndex(i).FindPropertyRelative("attemps");

                for (int j = 0; j < mode_attemps.arraySize; j++)
                {
                    SerializedProperty attemp_interactions = mode_attemps.GetArrayElementAtIndex(j).FindPropertyRelative("interactions");

                    GUIContent attemp_content = new GUIContent();
                    attemp_content.text = (j + 1) + "° attemp";

                    myTarget.modes[i].attemps[j].expandedInInspector = EditorGUILayout.Foldout(myTarget.modes[i].attemps[j].expandedInInspector, attemp_content);

                    if (myTarget.modes[i].attemps[j].expandedInInspector)
                    {
                        EditorGUILayout.BeginVertical("GroupBox");

                        for (int k = 0; k < attemp_interactions.arraySize; k++)
                        {
                            GUIContent interaction_content = new GUIContent();
                            interaction_content.text = (k + 1) + "° action";

                            myTarget.modes[i].attemps[j].interactions[k].expandedInInspector = EditorGUILayout.Foldout(myTarget.modes[i].attemps[j].interactions[k].expandedInInspector, interaction_content);
                            if (myTarget.modes[i].attemps[j].interactions[k].expandedInInspector)
                            {
                                EditorGUILayout.BeginVertical("GroupBox");

                                EditorGUILayout.PropertyField(attemp_interactions.GetArrayElementAtIndex(k).FindPropertyRelative("type"));


                                if (myTarget.modes[i].attemps[j].interactions[k].type == Interaction.InteractionType.character)
                                { 
                                    EditorGUILayout.PropertyField(attemp_interactions.GetArrayElementAtIndex(k).FindPropertyRelative("character"));
                                    EditorGUILayout.PropertyField(attemp_interactions.GetArrayElementAtIndex(k).FindPropertyRelative("characterAction"));
                                    if (myTarget.modes[i].attemps[j].interactions[k].characterAction == Interaction.CharacterAction.say)
                                        EditorGUILayout.PropertyField(attemp_interactions.GetArrayElementAtIndex(k).FindPropertyRelative("WhatToSay"));
                                    if (myTarget.modes[i].attemps[j].interactions[k].characterAction == Interaction.CharacterAction.walk)
                                        EditorGUILayout.PropertyField(attemp_interactions.GetArrayElementAtIndex(k).FindPropertyRelative("WhereToWalk"));
                                }
                                else if(myTarget.modes[i].attemps[j].interactions[k].type == Interaction.InteractionType.custom)
                                    EditorGUILayout.PropertyField(attemp_interactions.GetArrayElementAtIndex(k).FindPropertyRelative("action"));

                                EditorGUILayout.EndVertical();
                            }
                        }

                        if (attemp_interactions.arraySize > 0 && GUILayout.Button("Delete last interaction"))
                        {
                            List<Interaction> list_interactions = myTarget.modes[i].attemps[j].interactions.ToList();
                            list_interactions.RemoveAt(list_interactions.Count - 1);
                            myTarget.modes[i].attemps[j].interactions = list_interactions.ToArray();
                        }

                        if (GUILayout.Button("Create new interaction"))
                        {
                            if (myTarget.modes[i].attemps[j].interactions == null)
                            {
                                myTarget.modes[i].attemps[j].interactions = new Interaction[0];
                            }
                            myTarget.modes[i].attemps[j].interactions = myTarget.modes[i].attemps[j].interactions.Append(new Interaction()).ToArray();
                        }

                        EditorGUILayout.EndVertical();
                    }

                }


                if(mode_attemps.arraySize > 0 && GUILayout.Button("Delete last attemp"))
                {
                    List<InteractionsAttemp> list_interactions = myTarget.modes[i].attemps.ToList();
                    list_interactions.RemoveAt(list_interactions.Count-1);
                    myTarget.modes[i].attemps = list_interactions.ToArray();
                }

                if (GUILayout.Button("Create new attemp"))
                {
                    myTarget.modes[i].attemps = myTarget.modes[i].attemps.Append(new InteractionsAttemp()).ToArray();
                }
                EditorGUILayout.EndVertical();
            }
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
