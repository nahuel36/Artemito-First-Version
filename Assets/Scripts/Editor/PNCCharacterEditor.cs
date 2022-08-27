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
    SerializedProperty variables;
    bool[] showVariable; 

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

        variables = serializedObject.FindProperty("variables");

        showVariable = new bool[variables.arraySize];
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


        for (int i = 0; i < variables.arraySize; i++)
        {
            showVariable[i] = EditorGUILayout.Foldout(showVariable[i], ((PNCCharacter)target).variables[i].name);

            if(showVariable[i])
            {
                ((PNCCharacter)target).variables[i].name = EditorGUILayout.TextField("name:", ((PNCCharacter)target).variables[i].name);

                ((PNCCharacter)target).variables[i].type = (PnCInteractuableVariables.types)EditorGUILayout.EnumFlagsField("types:",((PNCCharacter)target).variables[i].type);

                if(((PNCCharacter)target).variables[i].type.HasFlag(PnCInteractuableVariables.types.integer))
                {
                    if(!((PNCCharacter)target).variables[i].integerDefault)
                    { 
                        ((PNCCharacter)target).variables[i].integer = EditorGUILayout.IntField("integer value:",((PNCCharacter)target).variables[i].integer);
                        if (GUILayout.Button("Set integer default value"))
                            ((PNCCharacter)target).variables[i].integerDefault = true;
                    }
                    else
                    { 
                        GUILayout.Label("integer value : default", EditorStyles.boldLabel);
                        if (GUILayout.Button("Set integer value"))
                        { 
                            ((PNCCharacter)target).variables[i].integer = 0;
                            ((PNCCharacter)target).variables[i].integerDefault = false;
                        }
                    }
                }
                if (((PNCCharacter)target).variables[i].type.HasFlag(PnCInteractuableVariables.types.boolean))
                {
                    if (!((PNCCharacter)target).variables[i].booleanDefault)
                    {
                        ((PNCCharacter)target).variables[i].boolean = EditorGUILayout.Toggle("boolean value:", ((PNCCharacter)target).variables[i].boolean);
                        if (GUILayout.Button("Set boolean default value"))
                            ((PNCCharacter)target).variables[i].booleanDefault = true;
                    }
                    else
                    {
                        GUILayout.Label("boolean value : default", EditorStyles.boldLabel);
                        if (GUILayout.Button("Set boolean value"))
                        { 
                           ((PNCCharacter)target).variables[i].boolean = false;
                            ((PNCCharacter)target).variables[i].booleanDefault = false;
                        }
                    }
                }
                if (((PNCCharacter)target).variables[i].type.HasFlag(PnCInteractuableVariables.types.String))
                {
                    if(!((PNCCharacter)target).variables[i].stringDefault)
                    { 
                        ((PNCCharacter)target).variables[i].String = EditorGUILayout.TextField("string value:", ((PNCCharacter)target).variables[i].String);
                        if (GUILayout.Button("Set string default value"))
                            ((PNCCharacter)target).variables[i].stringDefault = true;
                    }
                    else
                    {
                        GUILayout.Label("string value : default", EditorStyles.boldLabel);
                        if (GUILayout.Button("Set string value"))
                        { 
                            ((PNCCharacter)target).variables[i].String = "";
                            ((PNCCharacter)target).variables[i].stringDefault = false;
                        }
                    }
                }

                if (GUILayout.Button("Delete " + ((PNCCharacter)target).variables[i].name))
                {
                    variables.DeleteArrayElementAtIndex(i);
                }
            }


        }


        if (GUILayout.Button("Create variable"))
        {
            //serializedObject.ApplyModifiedProperties();
            ((PNCCharacter)target).variables = ((PNCCharacter)target).variables.Append<PnCInteractuableVariables>(new PnCInteractuableVariables()).ToArray();
            //variables.arraySize++;
            showVariable = showVariable.Append(false).ToArray();
        }

        var group = ((PNCCharacter)target).variables.GroupBy(vari => vari.name, (vari) => new { Count = vari.name.Count() });
        bool repeated = false;

        foreach(var vari in group)
        {
            if (vari.Count() > 1)
                repeated = true;
            
        }
        if(repeated)
            GUILayout.Label("There are more than one variable with the same name", EditorStyles.boldLabel);


        serializedObject.ApplyModifiedProperties();

        if(GUI.changed)
        {
            EditorUtility.SetDirty(target);
        }

    }
}
