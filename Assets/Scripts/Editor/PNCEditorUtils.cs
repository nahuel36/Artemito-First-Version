using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
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



    public static void ShowLocalVariables(ref InteractuableLocalVariable[] variables, ref SerializedProperty variables_serialized)
    {
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Label("Local Variables", EditorStyles.boldLabel);
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

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

    public static void ShowGlobalVariables(System.Enum type, ref InteractuableGlobalVariable[] variables, ref SerializedProperty variables_serialized)
    {
        Settings settings = Resources.Load<Settings>("Settings/Settings");

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Label("Global Variables", EditorStyles.boldLabel);
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
                if (variables[i].properties.variable_type.HasFlag(GlobalVariableProperty.variable_types.boolean))
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
                if (variables[i].properties.variable_type.HasFlag(GlobalVariableProperty.variable_types.String))
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
            GUILayout.Label("There are more than one variable with the same name", EditorStyles.boldLabel);

    }
}
