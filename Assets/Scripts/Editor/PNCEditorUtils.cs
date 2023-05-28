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
