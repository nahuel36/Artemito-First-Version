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
    NodeBasedEditor nodeBase;

    private void OnEnable()
    {
        CheckNodes();
    }

    private void CheckNodes()
    {
        Dialog myTarget = (Dialog)target;

        allSubDialogsList = new ReorderableList(serializedObject, serializedObject.FindProperty("subDialogs"), true, true, true, true)
        {
            drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                EditorGUI.PropertyField(new Rect(rect.x+7, rect.y, rect.width-7, EditorGUIUtility.singleLineHeight), serializedObject.FindProperty("subDialogs").GetArrayElementAtIndex(index).FindPropertyRelative("text"), new GUIContent { text = "sub-dialog " + (index + 1) });

                SerializedProperty expanded = serializedObject.FindProperty("subDialogs").GetArrayElementAtIndex(index).FindPropertyRelative("expandedInInspector");

                expanded.boolValue = EditorGUI.Foldout(new Rect(rect.x+7, rect.y, rect.width, EditorGUIUtility.singleLineHeight), expanded.boolValue, GUIContent.none);

                rect.y += EditorGUIUtility.singleLineHeight;

                if (expanded.boolValue)
                { 
                    int key = serializedObject.FindProperty("subDialogs").GetArrayElementAtIndex(index).FindPropertyRelative("index").intValue;
                    SerializedProperty options = serializedObject.FindProperty("subDialogs").GetArrayElementAtIndex(index).FindPropertyRelative("options");
                
                        var optionList = new ReorderableList(options.serializedObject, options, true, true, true, true)
                        {
                            drawElementCallback = (Rect recOpt, int indexOpt, bool isActiveOpt, bool isFocusedOpt) =>
                            {
                                EditorGUI.PropertyField(new Rect(recOpt.x + 7, recOpt.y, recOpt.width -7, EditorGUIUtility.singleLineHeight), options.GetArrayElementAtIndex(indexOpt).FindPropertyRelative("text"), new GUIContent { text= "option " + (indexOpt+1)});            
                                PNCEditorUtils.DrawElementAttempContainer(options, indexOpt, recOpt, optionAttempsListDict, optionInteractionListDict, myTarget.subDialogs[index].options[indexOpt].attempsContainer.attemps, true);

                            },
                            elementHeightCallback = (int indexOpt) =>
                            {
                                return PNCEditorUtils.GetAttempsContainerHeight(options, indexOpt);
                            },
                            drawHeaderCallback = (rect) => 
                            {
                                EditorGUI.LabelField(rect, "options");
                            },
                            onAddCallback = (list) =>
                            {
                                serializedObject.FindProperty("subDialogs").GetArrayElementAtIndex(index).FindPropertyRelative("optionSpecialIndex").intValue++;
                                ReorderableList.defaultBehaviours.DoAddButton(list);
                                int specialIndex = serializedObject.FindProperty("subDialogs").GetArrayElementAtIndex(index).FindPropertyRelative("optionSpecialIndex").intValue;
                                options.GetArrayElementAtIndex(list.index).FindPropertyRelative("index").intValue = specialIndex;
                                if (nodeBase)
                                {
                                    Debug.Log("agregar esto");
                                }
                            },
                            onReorderCallback = (list) =>
                            {
                                if (nodeBase)
                                {
                                    nodeBase.InitializeConnections();
                                }
                            }
                        };

                    if (!subDialogDict.ContainsKey(key))
                    {
                        subDialogDict.Add(key, optionList);
                    }
               
                    subDialogDict[key].DoList(rect);
                }
            }
            ,
            elementHeightCallback = (int index) =>
            {
                int key = serializedObject.FindProperty("subDialogs").GetArrayElementAtIndex(index).FindPropertyRelative("index").intValue;

                float height = EditorGUIUtility.singleLineHeight * 1;

                SerializedProperty expanded = serializedObject.FindProperty("subDialogs").GetArrayElementAtIndex(index).FindPropertyRelative("expandedInInspector");

                if (expanded.boolValue) 
                {
                    height = EditorGUIUtility.singleLineHeight * 5;

                    if (subDialogDict.ContainsKey(key))
                    {
                        for (int i = 0; i < subDialogDict[key].count; i++)
                            height += subDialogDict[key].elementHeightCallback(i);
                    }
                }
                               
                return height;
            }
            ,
            onAddCallback = (list)=>
            {
                serializedObject.FindProperty("subDialogIndex").intValue++;
                ReorderableList.defaultBehaviours.DoAddButton(list);
                int specialindex = serializedObject.FindProperty("subDialogIndex").intValue;
                serializedObject.FindProperty("subDialogs").GetArrayElementAtIndex(list.index).FindPropertyRelative("index").intValue = specialindex;
                if (nodeBase)
                {
                    nodeBase.InitializeNodes();
                }
            },
            onRemoveCallback = (list) =>
            {
                ReorderableList.defaultBehaviours.DoRemoveButton(list);
                if (nodeBase)
                {
                    nodeBase.InitializeNodes();
                }
            }
            ,
            drawHeaderCallback = (rect) =>
            {
                EditorGUI.LabelField(rect, "sub-dialogs");
            }

        };
    }

    

    public override void OnInspectorGUI()
    {
        Dialog myTarget = (Dialog)target;

        if (subDialogDict != null && myTarget.subDialogs != null && subDialogDict.Keys.Count < myTarget.subDialogs.Count)
        {
            EditorUtility.SetDirty(target);
            serializedObject.ApplyModifiedProperties();
            serializedObject.Update();
        }
        
        serializedObject.Update();

        if (serializedObject.hasModifiedProperties)
        {
            CheckNodes();
        }

        allSubDialogsList.DoLayoutList();
        

        serializedObject.ApplyModifiedProperties();

        if (GUILayout.Button("View nodes")) 
        {
            if(nodeBase == null)
                nodeBase = CreateInstance<NodeBasedEditor>();
            nodeBase.OpenWindow((Dialog)target, serializedObject);
        }
        if (GUI.changed)
        { 
            EditorUtility.SetDirty(target);
        }

    }
}
