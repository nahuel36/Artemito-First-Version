using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PNCCharacter))]
public class PnCCharacterEditor : Editor
{
    Settings settings;
    SerializedProperty interactions;

    public void OnEnable()
    {
        settings = AssetDatabase.LoadAssetAtPath<Settings>("Assets/PnC/Settings/Settings.asset");
        interactions = serializedObject.FindProperty("interactions");
        int plusSize = interactions.arraySize;
        while (plusSize < settings.modes.Length)
            plusSize++;
        interactions.arraySize = plusSize;
        Debug.Log("array size" + interactions.arraySize);
        serializedObject.ApplyModifiedProperties();

        for (int i = 0; i < settings.modes.Length; i++)
        {
            ((PNCCharacter)target).interactions[i].name = settings.modes[i];

        }
    }

    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI();
        //EditorGUILayout.PropertyField(serializedObject.FindProperty("target"));
        //EditorGUILayout.ObjectField(serializedObject.FindProperty("target"));
        

        for (int i = 0; i < settings.modes.Length; i++)
        {
           //EditorGUILayout.LabelField(settings.modes[i]);
            EditorGUILayout.PropertyField(interactions.GetArrayElementAtIndex(i),true);
        }
        /*
        foreach(string mode in set.modes)
        {
            EditorGUILayout.LabelField(mode);
            (target as PNCCharacter).interactions[]
            EditorGUILayout.PropertyField(serializedObject.FindProperty("interactions"));
        }
        */
        serializedObject.ApplyModifiedProperties();

    }
}
