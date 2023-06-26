using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

[CustomEditor(typeof(Dialog))]
public class DialogEditor : Editor
{
    ReorderableList optionsList;
    Dictionary<string, ReorderableList> optionAttempsListDict = new Dictionary<string, ReorderableList>();
    Dictionary<string, ReorderableList> optionInteractionListDict = new Dictionary<string, ReorderableList>();

    private void OnEnable()
    {
        Dialog myTarget = (Dialog)target;
        optionsList = new ReorderableList(serializedObject, serializedObject.FindProperty("options"), true, true, true, true) { 
            drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                PNCEditorUtils.DrawElementAttempContainer(serializedObject.FindProperty("options"), index, rect, optionAttempsListDict, optionInteractionListDict, myTarget.options[index].attempsContainer.attemps, true);
            },
            elementHeightCallback = (int indexInv) =>
            {
                return PNCEditorUtils.GetAttempsContainerHeight(serializedObject.FindProperty("options"), indexInv);
            }
        };

    }

    public override void OnInspectorGUI()
    {
        optionsList.DoLayoutList();

        serializedObject.ApplyModifiedProperties();
        if (GUI.changed)
        {
            EditorUtility.SetDirty(target);
        }

    }
}
