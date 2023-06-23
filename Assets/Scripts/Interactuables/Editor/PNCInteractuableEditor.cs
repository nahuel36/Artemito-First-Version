using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System;

public class PNCInteractuableEditor : PNCVariablesContainerEditor
{
    Dictionary<string, ReorderableList> verbAttempsListDict = new Dictionary<string, ReorderableList>();
    Dictionary<string, ReorderableList> verbInteractionsListDict = new Dictionary<string, ReorderableList>();
    protected ReorderableList verbsList;

    Dictionary<string, ReorderableList> invAttempsListDict = new Dictionary<string, ReorderableList>();
    Dictionary<string, ReorderableList> invInteractionsListDict = new Dictionary<string, ReorderableList>();
    protected ReorderableList invList;

    protected void ShowInteractionVerbs()
    {
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUIStyle tittleStyle = new GUIStyle();
        tittleStyle.normal.textColor = Color.white;
        tittleStyle.fontSize = 14;
        GUILayout.Label("<b>Interactions</b>", tittleStyle);
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        PNCCharacter myTarget = (PNCCharacter)target;

        if (GUILayout.Button("Edit verbs"))
        {
            Selection.objects = new UnityEngine.Object[] { settings };
            EditorGUIUtility.PingObject(settings);
        }
        verbsList.DoLayoutList();
    }

    protected void ShowInventoryInteractions()
    {
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUIStyle tittleStyle = new GUIStyle();
        tittleStyle.normal.textColor = Color.white;
        tittleStyle.fontSize = 14;
        GUILayout.Label("<b>Inventory Interactions</b>", tittleStyle);
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        invList.DoLayoutList();
    }

    
    protected void InitializeInventoryInteractions() 
    {
        PNCInteractuable myTarget = (PNCInteractuable)target;

        SerializedProperty inv_serialized = serializedObject.FindProperty("inventoryActions");

        settings = Resources.Load<Settings>("Settings/Settings");

        InventoryList inventory = Resources.Load<InventoryList>("Inventory");

        invList = new ReorderableList(serializedObject, inv_serialized, true, true, true, true)
        {
            drawHeaderCallback = (rect) =>
            {
                EditorGUI.LabelField(rect, "inventory actions");
            },
            elementHeightCallback = (int indexInv) =>
            {
                float height = 0;
                if (myTarget.inventoryActions[indexInv].specialIndex == 0)
                    height += EditorGUIUtility.singleLineHeight;
                height += PNCEditorUtils.GetAttempsContainerHeight(inv_serialized, indexInv);
                return height;
            },
            drawElementCallback = (rect, indexInv, active, focus) =>
            {
                string[] content = new string[inventory.items.Length +1 ];
                content[0] = "(Scene Object)";
                for (int i = 0; i < inventory.items.Length; i++) 
                {
                    content[i+1] = inventory.items[i].itemName;
                }
                
                int selected = 0;
                if (myTarget.inventoryActions[indexInv].specialIndex != -1 && myTarget.inventoryActions[indexInv].specialIndex != 0)
                {
                    for (int i = 0; i < inventory.items.Length; i++)
                    {
                        if (inventory.items[i].specialIndex == myTarget.inventoryActions[indexInv].specialIndex)
                            selected = i+1;
                    }
                }
                rect.height = EditorGUIUtility.singleLineHeight;

                selected = EditorGUI.Popup(new Rect(rect.x + rect.width / 2.25f , rect.y, rect.width / 2, rect.height), "", selected, content);

                if (selected != 0)
                    myTarget.inventoryActions[indexInv].specialIndex = inventory.items[selected - 1].specialIndex;
                else
                {
                    myTarget.inventoryActions[indexInv].specialIndex = 0;
                }



                List<string> verbsContent = new List<string>();
                for (int i = 0; i < settings.verbs.Length; i++)
                {
                    if (settings.verbs[i].isLikeGive || settings.verbs[i].isLikeUse)
                        verbsContent.Add(settings.verbs[i].name);
                }
                int verbSelected = 0;
                if (myTarget.inventoryActions[indexInv].verb.index >= 0)
                {
                    for (int i = 0; i < settings.verbs.Length; i++)
                    {
                        if (settings.verbs[i].index == myTarget.inventoryActions[indexInv].verb.index)
                            verbSelected = i;
                    }
                }
                verbSelected = EditorGUI.Popup(new Rect(rect.x + 7, rect.y, rect.width / 2.5f, EditorGUIUtility.singleLineHeight), "", verbSelected, verbsContent.ToArray());

                myTarget.inventoryActions[indexInv].verb.index = settings.verbs[verbSelected].index;

                if(selected == 0)
                {
                    EditorGUI.PropertyField(new Rect(rect.x + rect.width/2 - 20, rect.y + EditorGUIUtility.singleLineHeight, rect.width/2, rect.height), inv_serialized.GetArrayElementAtIndex(indexInv).FindPropertyRelative("sceneObject"), GUIContent.none);
                    rect.y += EditorGUIUtility.singleLineHeight;
                }

                PNCEditorUtils.DrawElementAttempContainer(inv_serialized, indexInv, rect, invAttempsListDict, invInteractionsListDict, myTarget.inventoryActions[indexInv].attempsContainer.attemps, true);
            }
        };

    }

    protected void InitializeVerbs(out ReorderableList verbsList, SerializedObject serializedObjectVerb, SerializedProperty serializedProperty, PNCInteractuable myTarget) 
    {
        settings = Resources.Load<Settings>("Settings/Settings");
       
        verbsList = new ReorderableList(serializedObjectVerb, serializedProperty, true, true, true, true)
        {
            drawHeaderCallback = (rect) =>
            {
                EditorGUI.LabelField(rect, "verbs");
            },
            elementHeightCallback = (int indexV) =>
            {
                return PNCEditorUtils.GetAttempsContainerHeight(serializedProperty, indexV);
            },
            drawElementCallback = (rect, indexV, active, focus) =>
            {
                PNCEditorUtils.DrawElementAttempContainer(serializedProperty, indexV, rect, verbAttempsListDict, verbInteractionsListDict, myTarget.verbs[indexV].attempsContainer.attemps, false);
            },
            onCanAddCallback = (list) =>
            {
                return myTarget.verbs.Count < settings.verbs.Length;
            },
            onAddDropdownCallback = (rect, list)=>
            {
                var menu = new GenericMenu();

                List<int> indexs = new List<int>();
                for (int i = 0; i < settings.verbs.Length; i++)
                {
                    bool founded = false;
                    for (int j = 0; j < myTarget.verbs.Count; j++)
                    {
                        if (settings.verbs[i].index == myTarget.verbs[j].verb.index)
                        {
                            founded = true;
                            break;
                        }
                    }
                    if (!founded)
                        indexs.Add(i);
                }

                for (int i = 0; i < indexs.Count; i++)
                {
                    menu.AddItem(new GUIContent(settings.verbs[indexs[i]].name), false, OnAddNewVerb, new NewVerbVariableParam() { index = indexs[i]});
                }

                menu.ShowAsContext();
            }
        };

        
        List<VerbInteractions> interactionsTempList = new List<VerbInteractions>();
        List<int> interactionsAdded = new List<int>();
        for (int i = 0; i < myTarget.verbs.Count; i++)
        {
            for (int j = 0; j < settings.verbs.Length; j++)
            {
                if (myTarget.verbs[i].verb.index == settings.verbs[j].index)
                {
                    VerbInteractions tempVerb = new VerbInteractions();
                    tempVerb.verb = new Verb();
                    tempVerb.verb.name = settings.verbs[j].name;
                    tempVerb.verb.isLikeUse = settings.verbs[j].isLikeUse;
                    tempVerb.verb.isLikeGive = settings.verbs[j].isLikeGive;
                    tempVerb.verb.index = settings.verbs[j].index;
                    tempVerb.attempsContainer = myTarget.verbs[i].attempsContainer;
                    if (!interactionsAdded.Contains(settings.verbs[j].index))
                    {
                        interactionsAdded.Add(settings.verbs[j].index);
                        interactionsTempList.Add(tempVerb);
                    }
                    break;
                }
            }
        }

        myTarget.verbs = interactionsTempList;
    }

    private void OnAddNewVerb(object var)
    {
        NewVerbVariableParam variable = (NewVerbVariableParam)var;
        int elementIndex = verbsList.serializedProperty.arraySize;
        int settingsVerbIndex = variable.index;

        verbsList.serializedProperty.arraySize++;
        verbsList.index = elementIndex;
        var element = verbsList.serializedProperty.GetArrayElementAtIndex(elementIndex);
        element.FindPropertyRelative("verb").FindPropertyRelative("name").stringValue = settings.verbs[settingsVerbIndex].name;
        element.FindPropertyRelative("verb").FindPropertyRelative("isLikeUse").boolValue = settings.verbs[settingsVerbIndex].isLikeUse;
        element.FindPropertyRelative("verb").FindPropertyRelative("isLikeGive").boolValue = settings.verbs[settingsVerbIndex].isLikeGive;
        element.FindPropertyRelative("verb").FindPropertyRelative("index").intValue = settings.verbs[settingsVerbIndex].index;
        
        serializedObject.ApplyModifiedProperties();
    }
}

internal class NewVerbVariableParam
{
    public int index { get; set; }
    public ReorderableList list;
}