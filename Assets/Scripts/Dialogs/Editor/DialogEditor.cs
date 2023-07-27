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
        subDialogDict = new Dictionary<int, ReorderableList>();

        optionAttempsListDict = new Dictionary<string, ReorderableList>();

        optionInteractionListDict = new Dictionary<string, ReorderableList>();

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
                            EditorGUI.PropertyField(new Rect(recOpt.x + 7, recOpt.y, recOpt.width - 7, EditorGUIUtility.singleLineHeight), options.GetArrayElementAtIndex(indexOpt).FindPropertyRelative("text"), new GUIContent { text = "option " + (indexOpt + 1) });
                            var verbExpanded = options.GetArrayElementAtIndex(indexOpt).FindPropertyRelative("attempsContainer").FindPropertyRelative("expandedInInspector");
                            if (verbExpanded.boolValue)
                            {
                                recOpt.y += EditorGUIUtility.singleLineHeight;
                                EditorGUI.PropertyField(new Rect(recOpt.x +7,recOpt.y, recOpt.width - 7, EditorGUIUtility.singleLineHeight), options.GetArrayElementAtIndex(indexOpt).FindPropertyRelative("initialState"));
                                recOpt.y += EditorGUIUtility.singleLineHeight;
                                EditorGUI.PropertyField(new Rect(recOpt.x + 7, recOpt.y, recOpt.width - 7, EditorGUIUtility.singleLineHeight), options.GetArrayElementAtIndex(indexOpt).FindPropertyRelative("say"));
                            }
                            PNCEditorUtils.DrawElementAttempContainer(options, indexOpt, recOpt, optionAttempsListDict, optionInteractionListDict, myTarget.subDialogs[index].options[indexOpt].attempsContainer.attemps, false, true);
                        },
                        elementHeightCallback = (int indexOpt) =>
                        {
                            var verbExpanded = options.GetArrayElementAtIndex(indexOpt).FindPropertyRelative("attempsContainer").FindPropertyRelative("expandedInInspector");
                            if (verbExpanded.boolValue)
                                return EditorGUIUtility.singleLineHeight * 2 + PNCEditorUtils.GetAttempsContainerHeight(options, indexOpt);
                            return EditorGUIUtility.singleLineHeight;
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
                            options.GetArrayElementAtIndex(list.index).FindPropertyRelative("say").boolValue = true;
                            options.GetArrayElementAtIndex(list.index).FindPropertyRelative("index").intValue = specialIndex;
                            serializedObject.ApplyModifiedProperties();
                            serializedObject.Update();
                            if (nodeBase)
                            {
                                nodeBase.InitializeNodes();
                                nodeBase.InitializeConnections();
                            }
                        },
                        onReorderCallback = (list) =>
                        {
                            if (nodeBase)
                            {
                                nodeBase.InitializeConnections();
                            }
                        },
                        onRemoveCallback = (list) =>
                            {
                                ReorderableList.defaultBehaviours.DoRemoveButton(list);
                                serializedObject.ApplyModifiedProperties();
                                serializedObject.Update();
                                if (nodeBase)
                                {
                                    nodeBase.InitializeNodes();
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
            onRemoveCallback = (list) =>
            {
                int index = serializedObject.FindProperty("subDialogs").GetArrayElementAtIndex(list.index).FindPropertyRelative("index").intValue;

                for (int i = 0; i < serializedObject.FindProperty("subDialogs").arraySize; i++)
                {
                    for (int j = 0; j < serializedObject.FindProperty("subDialogs").GetArrayElementAtIndex(i).FindPropertyRelative("options").arraySize; j++)
                    {
                        if (serializedObject.FindProperty("subDialogs").GetArrayElementAtIndex(i).FindPropertyRelative("options").GetArrayElementAtIndex(j).FindPropertyRelative("subDialogDestinyIndex").intValue == index)
                        {
                            serializedObject.FindProperty("subDialogs").GetArrayElementAtIndex(i).FindPropertyRelative("options").GetArrayElementAtIndex(j).FindPropertyRelative("subDialogDestinyIndex").intValue = 0;
                        }
                    }
                }

                if (serializedObject.FindProperty("entryDialogIndex").intValue == index)
                {
                    serializedObject.FindProperty("entryDialogIndex").intValue = 0;
                }

                ReorderableList.defaultBehaviours.DoRemoveButton(list);
                serializedObject.ApplyModifiedProperties();
                CheckNodes();
                OnInspectorGUI();

                if (nodeBase)
                {
                    nodeBase.InitializeNodes();
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

                    if (subDialogDict.ContainsKey(key) && subDialogDict[key] != null && subDialogDict[key].count > 0)
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
