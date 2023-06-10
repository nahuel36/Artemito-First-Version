using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using UnityEditorInternal;
[CustomEditor(typeof(PNCVariablesContainer))]
public class PNCVariablesContainerEditor : Editor
{
    protected Settings settings;
    protected SerializedProperty local_variables_serialized;
    protected SerializedProperty global_variables_serialized;
    protected ReorderableList localVariablesList;


    private void OnEnable()
    {
        local_variables_serialized = serializedObject.FindProperty("local_variables");
        global_variables_serialized = serializedObject.FindProperty("global_variables");

        PNCEditorUtils.InitializeLocalVariables(out localVariablesList, serializedObject, serializedObject.FindProperty("local_variables"));

        PNCEditorUtils.InitializeGlobalVariables(GlobalVariableProperty.object_types.variableContainer, ref ((PNCVariablesContainer)target).global_variables);
    }


    public override void OnInspectorGUI()
    {
        PNCVariablesContainer myTarget = (PNCVariablesContainer)target;

        PNCEditorUtils.ShowLocalVariables(localVariablesList, ref myTarget.local_variables, ref local_variables_serialized);

        PNCEditorUtils.ShowGlobalVariables(GlobalVariableProperty.object_types.variableContainer, ref myTarget.global_variables, ref global_variables_serialized);

        serializedObject.ApplyModifiedProperties();

        if (GUI.changed)
        {
            EditorUtility.SetDirty(target);
        }
    }

}
