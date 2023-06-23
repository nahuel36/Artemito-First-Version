using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using UnityEngine.UIElements;
using UnityEditorInternal;
[CustomEditor(typeof(PNCCharacter))]
public class PnCCharacterEditor : PNCInteractuableEditor
{




    public void OnEnable()
    {
        InitializeVerbs(out verbsList, serializedObject , serializedObject.FindProperty("verbs"),((PNCCharacter)target));

        PNCEditorUtils.InitializeGlobalVariables(GlobalVariableProperty.object_types.characters, ref ((PNCCharacter)target).global_variables);
        PNCEditorUtils.InitializeLocalVariables(out localVariablesList, serializedObject, serializedObject.FindProperty("local_variables"));


        local_variables_serialized = serializedObject.FindProperty("local_variables");
        global_variables_serialized = serializedObject.FindProperty("global_variables");

        InitializeInventoryInteractions();
    }


    
    public override void OnInspectorGUI()
    {
        PNCCharacter myTarget = (PNCCharacter)target;


        EditorGUILayout.PropertyField(serializedObject.FindProperty("name"));

        ShowInteractionVerbs();


        ShowInventoryInteractions();


        if (settings.speechStyle == Settings.SpeechStyle.Sierra)
        { 
            GUILayout.Box(myTarget.SierraTextFace.texture,GUILayout.MaxHeight(100),GUILayout.MaxWidth(100),GUILayout.MinHeight(100),GUILayout.MinWidth(100));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("SierraTextFace"));
        }

        PNCEditorUtils.ShowLocalVariables(localVariablesList, ref myTarget.local_variables, ref local_variables_serialized);

        PNCEditorUtils.ShowGlobalVariables(GlobalVariableProperty.object_types.characters, ref myTarget.global_variables, ref global_variables_serialized);

        serializedObject.ApplyModifiedProperties();

        if(GUI.changed)
        {
            EditorUtility.SetDirty(target);
            EditorUtility.SetDirty(settings);
        }

    }

}
