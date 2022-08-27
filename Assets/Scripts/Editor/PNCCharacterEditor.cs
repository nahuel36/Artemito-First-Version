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
        int plusSize = interactions.arraySize;
        while (plusSize < settings.modes.Length)
            plusSize++;
        interactions.arraySize = plusSize;
        serializedObject.ApplyModifiedProperties();

        for (int i = 0; i < settings.modes.Length; i++)
        {
            ((PNCCharacter)target).interactions[i].name = settings.modes[i];

        }

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
            if (interactions.GetArrayElementAtIndex(i).isExpanded && interactions.GetArrayElementAtIndex(i).FindPropertyRelative("interactions").isExpanded && ((PNCCharacter)target).interactions[i].interactions.Length > 0 && GUILayout.Button("Delete last interaction"))
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

        ShowVariables(ref ((PNCCharacter)target).local_variables, ref local_variables_serialized, ref showLocalVariable);

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Label("Global Variables", EditorStyles.boldLabel);
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        ShowVariables(ref ((PNCCharacter)target).global_variables, ref global_variables_serialized, ref showGlobalVariable);

        serializedObject.ApplyModifiedProperties();

        if(GUI.changed)
        {
            EditorUtility.SetDirty(target);
        }

    }

    public void ShowVariables(ref InteractuableVariables[] variables, ref SerializedProperty variables_serialized, ref bool[] show_variables )
    {
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

        if (GUILayout.Button("Create variable"))
        {
            //serializedObject.ApplyModifiedProperties();
            variables = variables.Append<InteractuableVariables>(new InteractuableVariables()).ToArray();
            //variables.arraySize++;
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
}
