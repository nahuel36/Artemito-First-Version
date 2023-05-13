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
    SerializedProperty interactions;
    SerializedProperty local_variables_serialized;
    SerializedProperty global_variables_serialized;
    static bool[] showLocalVariable;
    static bool[] showGlobalVariable;
    static bool[] showmode;
    static Dictionary<string, bool> showAttemp = new Dictionary<string, bool>();

    public void OnEnable()
    {
        settings = Resources.Load<Settings>("Settings/Settings");

        if(showmode == null || showmode.Length != settings.modes.Length)
            showmode = new bool[settings.modes.Length];

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
                tempMode.interactionsLists = new InteractionList[0];
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

        List<InteractuableGlobalVariable> tempGlobalVarList = new List<InteractuableGlobalVariable>();
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
                    
                    ((PNCCharacter)target).global_variables[j].properties = settings.global_variables[i];

                    if (settings.global_variables[i].object_type.HasFlag((GlobalVariableProperty.object_types.characters)))
                        tempGlobalVarList.Add(((PNCCharacter)target).global_variables[j]);
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

        ((PNCCharacter)target).global_variables = tempGlobalVarList.ToArray();

        local_variables_serialized = serializedObject.FindProperty("local_variables");
        global_variables_serialized = serializedObject.FindProperty("global_variables");

        if(showLocalVariable == null || showLocalVariable.Length != local_variables_serialized.arraySize)
            showLocalVariable = new bool[local_variables_serialized.arraySize];
        if (showGlobalVariable == null || showGlobalVariable.Length != global_variables_serialized.arraySize)
            showGlobalVariable = new bool[global_variables_serialized.arraySize];
    }

    public override void OnInspectorGUI()
    {
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Label("Interactions", EditorStyles.boldLabel);
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        for (int i = 0; i < settings.modes.Length; i++)
        {
            EditorGUILayout.BeginVertical("HelpBox");
            showmode[i] = EditorGUILayout.Foldout(showmode[i],settings.modes[i].ToString());
            if (showmode[i])
            {

                for (int j = 0; j < interactions.GetArrayElementAtIndex(i).FindPropertyRelative("interactionsLists").arraySize; j++)
                {
                    EditorGUILayout.BeginVertical("GroupBox");

                    GUIContent content = new GUIContent();
                    content.text = (j + 1) + "° attemp";
                   // if(EditorGUILayout.Foldout()

                    EditorGUILayout.PropertyField(interactions.GetArrayElementAtIndex(i).FindPropertyRelative("interactionsLists").GetArrayElementAtIndex(j).FindPropertyRelative("interactions"), content);

                    EditorGUILayout.EndVertical();
                }


                if (((PNCCharacter)target).interactions.Length > 0 && ((PNCCharacter)target).interactions[i].interactionsLists != null &&  ((PNCCharacter)target).interactions[i].interactionsLists.Length > 0)
                    if(showmode[i])
                       if(GUILayout.Button("Delete last attemp"))
                        {
                            List<InteractionList> list_interactions = ((PNCCharacter)target).interactions[i].interactionsLists.ToList();
                            list_interactions.RemoveAt(list_interactions.Count-1);
                            ((PNCCharacter)target).interactions[i].interactionsLists = list_interactions.ToArray();
                        }

                if (showmode[i] && GUILayout.Button("Create new attemp"))
                {
                    ((PNCCharacter)target).interactions[i].interactionsLists = ((PNCCharacter)target).interactions[i].interactionsLists.Append(new InteractionList()).ToArray();
                }
            }
            EditorGUILayout.EndVertical();
        }

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

    public void ShowLocalVariables(ref InteractuableLocalVariable[] variables, ref SerializedProperty variables_serialized, ref bool[] show_variables)
    {
        if (show_variables.Length < variables.Length)
            show_variables = new bool[variables.Length];

        for (int i = 0; i < variables.Length; i++)
        {
            show_variables[i] = EditorGUILayout.Foldout(show_variables[i], variables[i].name);

            if (show_variables[i])
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

    public void ShowGlobalVariables(ref InteractuableGlobalVariable[] variables, ref SerializedProperty variables_serialized, ref bool[] show_variables)
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
                    if (!settings.global_variables[j].object_type.HasFlag(GlobalVariableProperty.object_types.characters))
                        areType = false;
                }
            }
            if (!areType)
                continue;

            show_variables[i] = EditorGUILayout.Foldout(show_variables[i], variables[i].name);

            if (show_variables[i])
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
