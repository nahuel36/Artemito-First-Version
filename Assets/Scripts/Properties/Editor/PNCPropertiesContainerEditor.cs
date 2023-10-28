using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using UnityEditorInternal;
[CustomEditor(typeof(PNCPropertiesContainer))]
public class PNCPropertiesContainerEditor : Editor
{
    protected Settings settings;
    protected SerializedProperty local_properties_serialized;
    protected SerializedProperty global_properties_serialized;
    protected ReorderableList localPropertiesList;


    private void OnEnable()
    {
        local_properties_serialized = serializedObject.FindProperty("local_properties");
        global_properties_serialized = serializedObject.FindProperty("global_properties");

        PNCEditorUtils.InitializeLocalProperties(out localPropertiesList, serializedObject, serializedObject.FindProperty("local_properties"));

        PNCEditorUtils.InitializeGlobalProperties(GlobalPropertyConfig.object_types.propertiesContainer, ref ((PNCPropertiesContainer)target).global_properties);
    }


    public override void OnInspectorGUI()
    {
        PNCPropertiesContainer myTarget = (PNCPropertiesContainer)target;

        PNCEditorUtils.ShowLocalPropertiesOnRect(localPropertiesList, ref myTarget.local_properties, ref local_properties_serialized);

        PNCEditorUtils.ShowGlobalProperties(GlobalPropertyConfig.object_types.propertiesContainer, ref myTarget.global_properties, ref global_properties_serialized);

        serializedObject.ApplyModifiedProperties();

        if (GUI.changed)
        {
            EditorUtility.SetDirty(target);
        }
    }

}
