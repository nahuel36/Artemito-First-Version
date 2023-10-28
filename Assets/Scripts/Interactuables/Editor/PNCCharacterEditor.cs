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

        PNCEditorUtils.InitializeGlobalProperties(GlobalPropertyConfig.object_types.characters, ref ((PNCCharacter)target).global_properties);
        PNCEditorUtils.InitializeLocalProperties(out localPropertiesList, serializedObject, serializedObject.FindProperty("local_properties"));


        local_properties_serialized = serializedObject.FindProperty("local_properties");
        global_properties_serialized = serializedObject.FindProperty("global_properties");

        InitializeInventoryInteractions();
    }


    
    public override void OnInspectorGUI()
    {
        PNCCharacter myTarget = (PNCCharacter)target;


        EditorGUILayout.PropertyField(serializedObject.FindProperty("interactuableName"));

        ShowInteractionVerbs();


        ShowInventoryInteractions();


        if (settings.speechStyle == Settings.SpeechStyle.Sierra)
        { 
            GUILayout.Box(myTarget.SierraTextFace.texture,GUILayout.MaxHeight(100),GUILayout.MaxWidth(100),GUILayout.MinHeight(100),GUILayout.MinWidth(100));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("SierraTextFace"));
        }

        PNCEditorUtils.ShowLocalPropertiesOnRect(localPropertiesList, ref myTarget.local_properties, ref local_properties_serialized);

        PNCEditorUtils.ShowGlobalPropertiesOnRect(GlobalPropertyConfig.object_types.characters, ref myTarget.global_properties, ref global_properties_serialized);

        serializedObject.ApplyModifiedProperties();

        if(GUI.changed)
        {
            EditorUtility.SetDirty(target);
            EditorUtility.SetDirty(settings);
        }

    }

}
