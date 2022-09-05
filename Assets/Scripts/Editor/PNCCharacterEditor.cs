using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

[CustomEditor(typeof(PNCCharacter))]
public class PnCCharacterEditor : Editor
{
    Settings settings;
    SerializedProperty interactions;
    SerializedProperty local_variables_serialized;
    SerializedProperty global_variables_serialized;
    bool[] showLocalVariable;
    bool[] showGlobalVariable;

    

    public void OnEnable()
    {
        settings = AssetDatabase.LoadAssetAtPath<Settings>("Assets/PnC/Settings/Settings.asset");
        interactions = serializedObject.FindProperty("interactions");

        List<Mode> interactionsTempList = new List<Mode>();
        for (int i = 0; i < settings.modes.Length; i++)
        {
            bool founded = false;
            for (int j = 0; j < ((PNCCharacter)target).interactions.Length; j++)
            {
                if (((PNCCharacter)target).interactions[j].name == settings.modes[i])
                { 
                    interactionsTempList.Add(((PNCCharacter)target).interactions[j]);
                    founded = true;
                }
            }
            if (founded == false)
            {
                Mode tempMode = new Mode();
                tempMode.name = settings.modes[i];
                tempMode.interactions = new UnityEngine.Events.UnityEvent[0];
                interactionsTempList.Add(tempMode);
            }
        }
        
        for (int i = 0; i < ((PNCCharacter)target).interactions.Length; i++)
        {
            bool contains = false;            
            for (int j = 0; j < interactionsTempList.Count; j++)
            {
                if (((PNCCharacter)target).interactions[i].name == interactionsTempList[j].name)
                {
                    contains = true;
                }
            }
            if(contains == false)
            {
                interactionsTempList.Add(((PNCCharacter)target).interactions[i]);
            }
        }
        
        ((PNCCharacter)target).interactions = interactionsTempList.ToArray();

        List<InteractuableVariables> tempGlobalVarList = new List<InteractuableVariables>();
        for (int i = 0; i < settings.global_variables.Length; i++)
        {
            bool founded = false;
            for (int j = 0; j < ((PNCCharacter)target).global_variables.Length; j++)
            {
                if ((settings.global_variables[i].ID == -1 && ((PNCCharacter)target).global_variables[j].name == settings.global_variables[i].name)|| 
                    (settings.global_variables[i].ID != -1 && ((PNCCharacter)target).global_variables[j].globalHashCode == -1 && ((PNCCharacter)target).global_variables[j].name == settings.global_variables[i].name) ||
                    (settings.global_variables[i].ID != -1 && ((PNCCharacter)target).global_variables[j].globalHashCode != -1 && ((PNCCharacter)target).global_variables[j].globalHashCode == settings.global_variables[i].ID))
                {
                    ((PNCCharacter)target).global_variables[j].name = settings.global_variables[i].name;
                    if (settings.global_variables[i].ID == -1)
                    { 
                        settings.global_variables[i].ID = ((PNCCharacter)target).global_variables[j].GetHashCode();
                        ((PNCCharacter)target).global_variables[j].globalHashCode = ((PNCCharacter)target).global_variables[j].GetHashCode(); 
                    }
                    else if(((PNCCharacter)target).global_variables[j].globalHashCode == -1)
                    {
                        ((PNCCharacter)target).global_variables[j].globalHashCode = settings.global_variables[i].ID;
                    }
                    if(settings.global_variables[i].type.HasFlag((GlobalVariableProperty.types.characters)))
                        tempGlobalVarList.Add(((PNCCharacter)target).global_variables[j]);
                    founded = true;
                }
            }
            if (founded == false)
            {
                InteractuableVariables tempMode = new InteractuableVariables();
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
                if (settings.global_variables[i].type.HasFlag((GlobalVariableProperty.types.characters)))
                    tempGlobalVarList.Add(tempMode);
            }
        }

        ((PNCCharacter)target).global_variables = tempGlobalVarList.ToArray();

        local_variables_serialized = serializedObject.FindProperty("local_variables");
        global_variables_serialized = serializedObject.FindProperty("global_variables");

        showLocalVariable = new bool[local_variables_serialized.arraySize];
        showGlobalVariable = new bool[global_variables_serialized.arraySize];
    }

    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI();
        //EditorGUILayout.PropertyField(serializedObject.FindProperty("target"));
        //EditorGUILayout.ObjectField(serializedObject.FindProperty("target"));

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Label("Interactions", EditorStyles.boldLabel);
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        //EditorGUILayout.LabelField("Interactions", new GUILayoutOption[] { new GUILayoutOption(GUILayoutOption.});

        for (int i = 0; i < settings.modes.Length; i++)
        {
            //EditorGUILayout.LabelField(settings.modes[i]);
            EditorGUILayout.PropertyField(interactions.GetArrayElementAtIndex(i),true);
            if (((PNCCharacter)target).interactions.Length > 0 && ((PNCCharacter)target).interactions[i].interactions != null &&  ((PNCCharacter)target).interactions[i].interactions.Length > 0)
                if(interactions.GetArrayElementAtIndex(i).isExpanded)
                    if(interactions.GetArrayElementAtIndex(i).FindPropertyRelative("interactions").isExpanded)
                        if(GUILayout.Button("Delete last interaction"))
            {
                List<UnityEngine.Events.UnityEvent> list_interactions = ((PNCCharacter)target).interactions[i].interactions.ToList();
                list_interactions.RemoveAt(list_interactions.Count-1);
                ((PNCCharacter)target).interactions[i].interactions = list_interactions.ToArray();
            }

            if (interactions.GetArrayElementAtIndex(i).isExpanded && interactions.GetArrayElementAtIndex(i).FindPropertyRelative("interactions").isExpanded && GUILayout.Button("Create interaction"))
            {
                ((PNCCharacter)target).interactions[i].interactions = ((PNCCharacter)target).interactions[i].interactions.Append(new UnityEngine.Events.UnityEvent()).ToArray();
            }
        }
        /*
        foreach(string mode in set.modes)
        { 
            EditorGUILayout.LabelField(mode);
            (target as PNCCharacter).interactions[]
            EditorGUILayout.PropertyField(serializedObject.FindProperty("interactions"));
        }
        */

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Label("Local Variables", EditorStyles.boldLabel);
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        ShowLocalVariables(ref ((PNCCharacter)target).local_variables, ref local_variables_serialized, ref showLocalVariable);

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Label("Global Variables", EditorStyles.boldLabel);
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        ShowGlobalVariables(ref ((PNCCharacter)target).global_variables, ref global_variables_serialized, ref showGlobalVariable);

        serializedObject.ApplyModifiedProperties();

        if(GUI.changed)
        {
            EditorUtility.SetDirty(target);
            EditorUtility.SetDirty(settings);
        }

    }

    public void ShowLocalVariables(ref InteractuableVariables[] variables, ref SerializedProperty variables_serialized, ref bool[] show_variables)
    {
        if (show_variables.Length < variables.Length)
            show_variables = new bool[variables.Length];

        for (int i = 0; i < variables.Length; i++)
        {
            show_variables[i] = EditorGUILayout.Foldout(show_variables[i], variables[i].name);

            if (show_variables[i])
            {
                variables[i].name = EditorGUILayout.TextField("name:", variables[i].name);

                variables[i].type = (InteractuableVariables.types)EditorGUILayout.EnumFlagsField("types:", variables[i].type);

                if (variables[i].type.HasFlag(InteractuableVariables.types.integer))
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
                if (variables[i].type.HasFlag(InteractuableVariables.types.boolean))
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
                if (variables[i].type.HasFlag(InteractuableVariables.types.String))
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
            }
        }

        if (GUILayout.Button("Create local variable"))
        {
            InteractuableVariables newvar = new InteractuableVariables();
            //serializedObject.ApplyModifiedProperties();
            
            //variables.arraySize++;

            variables = variables.Append<InteractuableVariables>(newvar).ToArray();
            show_variables = show_variables.Append(false).ToArray();
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

    public void ShowGlobalVariables(ref InteractuableVariables[] variables, ref SerializedProperty variables_serialized, ref bool[] show_variables)
    {
        if (show_variables.Length < variables.Length)
            show_variables = new bool[variables.Length];

        for (int i = 0; i < variables.Length; i++)
        {
            bool areType = true;

            for (int j = 0; j < settings.global_variables.Length; j++)
            {
                if (variables[i].globalHashCode != -1 && settings.global_variables[j].ID == variables[i].globalHashCode)
                {
                    variables[i].name = settings.global_variables[j].name;
                    if (!settings.global_variables[j].type.HasFlag(GlobalVariableProperty.types.characters))
                        areType = false;
                }
            }
            if (!areType)
                continue;

            show_variables[i] = EditorGUILayout.Foldout(show_variables[i], variables[i].name);

            if (show_variables[i])
            {
                variables[i].type = (InteractuableVariables.types)EditorGUILayout.EnumFlagsField("types:", variables[i].type);

                if (variables[i].type.HasFlag(InteractuableVariables.types.integer))
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
                if (variables[i].type.HasFlag(InteractuableVariables.types.boolean))
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
                if (variables[i].type.HasFlag(InteractuableVariables.types.String))
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
