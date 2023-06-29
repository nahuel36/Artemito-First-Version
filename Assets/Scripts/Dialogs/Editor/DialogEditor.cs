using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

[CustomEditor(typeof(Dialog))]
public class DialogEditor : Editor
{
    ReorderableList allSubDialogsList;
    Dictionary<int, ReorderableList> subDialogDict = new Dictionary<int, ReorderableList>();
    Dictionary<string, ReorderableList> optionAttempsListDict = new Dictionary<string, ReorderableList>();
    Dictionary<string, ReorderableList> optionInteractionListDict = new Dictionary<string, ReorderableList>();

    private void OnEnable()
    {
        CheckNodes();
    }

    private void CheckNodes()
    {
        Dialog myTarget = (Dialog)target;

        allSubDialogsList = new ReorderableList(serializedObject, serializedObject.FindProperty("nodes"), true, true, true, true)
        {
            drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                int key = serializedObject.FindProperty("nodes").GetArrayElementAtIndex(index).FindPropertyRelative("index").intValue;
                SerializedProperty options = serializedObject.FindProperty("nodes").GetArrayElementAtIndex(index).FindPropertyRelative("options");
                
                    var optionList = new ReorderableList(options.serializedObject, options, true, true, true, true)
                    {
                        drawElementCallback = (Rect recOpt, int indexOpt, bool isActiveOpt, bool isFocusedOpt) =>
                        {
                            PNCEditorUtils.DrawElementAttempContainer(options, indexOpt, recOpt, optionAttempsListDict, optionInteractionListDict, myTarget.nodes[index].options[indexOpt].attempsContainer.attemps, true);

                        },
                        elementHeightCallback = (int indexOpt) =>
                        {
                            return PNCEditorUtils.GetAttempsContainerHeight(options, indexOpt);
                        }
                    };

                if (!subDialogDict.ContainsKey(key))
                {
                    subDialogDict.Add(key, optionList);
                }
                else
                    subDialogDict[key] = optionList;
               
                subDialogDict[key].DoList(rect);
            }
            ,
            elementHeightCallback = (int index) =>
            {
                int key = serializedObject.FindProperty("nodes").GetArrayElementAtIndex(index).FindPropertyRelative("index").intValue;

                float height = EditorGUIUtility.singleLineHeight * 4;

                if (subDialogDict.ContainsKey(key))
                { 
                    for(int i= 0;i<subDialogDict[key].count;i++)
                        height += subDialogDict[key].elementHeightCallback(i);
                }
                               
                return height;
            }
            ,
            onAddCallback = (list)=>
            {
                ReorderableList.defaultBehaviours.DoAddButton(list);
                int specialindex = serializedObject.FindProperty("nodeIndex").intValue;
                serializedObject.FindProperty("nodes").GetArrayElementAtIndex(list.index).FindPropertyRelative("index").intValue = specialindex;
                serializedObject.FindProperty("nodeIndex").intValue++;
            }
        };
    }

    

    public override void OnInspectorGUI()
    {
        Dialog myTarget = (Dialog)target;

        if (subDialogDict != null && myTarget.nodes != null && subDialogDict.Keys.Count < myTarget.nodes.Count)
        {
            EditorUtility.SetDirty(target);
            serializedObject.ApplyModifiedProperties();
            serializedObject.Update();
            CheckNodes();
        }
        

        allSubDialogsList.DoLayoutList();
        

        serializedObject.ApplyModifiedProperties();

        if (GUILayout.Button("View nodes")) 
        {
            NodeBasedEditor nodeBase = CreateInstance<NodeBasedEditor>();
            nodeBase.OpenWindow((Dialog)target);
        }
        if (GUI.changed)
        { 
            EditorUtility.SetDirty(target);
        }

    }
}
