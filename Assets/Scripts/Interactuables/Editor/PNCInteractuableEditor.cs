using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System;

public class PNCInteractuableEditor : PNCPropertiesContainerEditor
{
    Dictionary<string, ReorderableList> verbAttempsListDict = new Dictionary<string, ReorderableList>();
    Dictionary<string, ReorderableList> verbInteractionsListDict = new Dictionary<string, ReorderableList>();
    protected ReorderableList verbsList;

    Dictionary<string, ReorderableList> invAttempsListDict = new Dictionary<string, ReorderableList>();
    Dictionary<string, ReorderableList> invInteractionsListDict = new Dictionary<string, ReorderableList>();
    protected ReorderableList invList;

    Dictionary<string, ReorderableList> customScriptInteractionDict = new Dictionary<string, ReorderableList>();

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
                rect.height = EditorGUIUtility.singleLineHeight;
                int selected = inv_serialized.GetArrayElementAtIndex(indexInv).FindPropertyRelative("specialIndex").intValue = PNCEditorUtils.GetInventoryWithPopUp(rect, inventory, inv_serialized.GetArrayElementAtIndex(indexInv).FindPropertyRelative("specialIndex").intValue, true);
                                
                myTarget.inventoryActions[indexInv].verb.index = PNCEditorUtils.SetVerbWithPopUp(rect, settings.verbs, myTarget.inventoryActions[indexInv].verb.index);

                if(selected == 0)
                {

                    EditorGUI.PropertyField(new Rect(rect.x + rect.width/2 - 20, rect.y + EditorGUIUtility.singleLineHeight, rect.width/2, rect.height), inv_serialized.GetArrayElementAtIndex(indexInv).FindPropertyRelative("sceneObject"), GUIContent.none);
                    rect.y += EditorGUIUtility.singleLineHeight;
                }

                PNCEditorUtils.DrawArrayWithAttempContainer(inv_serialized, indexInv, rect, invAttempsListDict, invInteractionsListDict, customScriptInteractionDict, myTarget.inventoryActions[indexInv].attempsContainer.attemps, true);
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
                PNCEditorUtils.DrawArrayWithAttempContainer(serializedProperty, indexV, rect, verbAttempsListDict, verbInteractionsListDict, customScriptInteractionDict, myTarget.verbs[indexV].attempsContainer.attemps, false);
            },
            onCanAddCallback = (list) =>
            {
                return myTarget.verbs.Count < settings.verbs.Length;
            },
            onAddDropdownCallback = (rect, list)=>
            {
                PNCEditorUtils.OnAddVerbDropdown(list, myTarget.verbs, serializedObject);
            }
        };


        PNCEditorUtils.CheckVerbs(ref myTarget.verbs);
    }


}

internal class NewVerbVariableParam
{
    public int index { get; set; }
    public ReorderableList list;
    public SerializedObject serializedObject;
}