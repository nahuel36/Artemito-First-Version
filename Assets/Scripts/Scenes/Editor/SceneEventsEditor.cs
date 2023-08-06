using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System;

[CustomEditor(typeof(SceneEvents))]
public class SceneEventsEditor : Editor
{
    protected Settings settings;

    Dictionary<string, ReorderableList> attempsListDict = new Dictionary<string, ReorderableList>();
    Dictionary<string, ReorderableList> interactionsListDict = new Dictionary<string, ReorderableList>();
        
    ReorderableList eventsList;


    Dictionary<string, ReorderableList> customScriptInteractionDict = new Dictionary<string, ReorderableList>();
    public void OnEnable()
    {
        SceneEvents myTarget = (SceneEvents)target;



        eventsList = new ReorderableList(serializedObject, serializedObject.FindProperty("events"), true, true, true, true)
        {
            drawElementCallback = (rect, indexEv, active, focus) =>
            {
                rect.height = EditorGUIUtility.singleLineHeight;

                EditorGUI.LabelField(new Rect(rect.x+7, rect.y, rect.width, rect.height), serializedObject.FindProperty("events").GetArrayElementAtIndex(indexEv).FindPropertyRelative("sceneEvent").enumDisplayNames[serializedObject.FindProperty("events").GetArrayElementAtIndex(indexEv).FindPropertyRelative("sceneEvent").enumValueIndex]);

                PNCEditorUtils.DrawArrayWithAttempContainer(serializedObject.FindProperty("events"), indexEv, rect, attempsListDict, interactionsListDict, customScriptInteractionDict, myTarget.events[indexEv].attempsContainer.attemps, true);
            },
            elementHeightCallback = (indexEv) =>
            {
                return PNCEditorUtils.GetAttempsContainerHeight(serializedObject.FindProperty("events"), indexEv);
            }, 
            onCanAddCallback = (list) =>
            {
                return myTarget.events.Count < Enum.GetValues(typeof(SceneEventInteraction.SceneEvent)).Length;
            },
            onAddDropdownCallback = (rect, list) =>
            {
                OnAddEventDropdown(list, myTarget.events, serializedObject);
            }
        };


    }

    void OnAddEventDropdown(ReorderableList list, List<SceneEventInteraction> events, SerializedObject serializedObject)
    {
        var menu = new GenericMenu();

        List<SceneEventInteraction.SceneEvent> indexs = new List<SceneEventInteraction.SceneEvent>();
        for (int i = 0; i < Enum.GetValues(typeof(SceneEventInteraction.SceneEvent)).Length; i++)
        {
            bool founded = false;
            SceneEventInteraction.SceneEvent value = (SceneEventInteraction.SceneEvent)Enum.GetValues(typeof(SceneEventInteraction.SceneEvent)).GetValue(i);
            for (int j = 0; j < events.Count; j++)
            {
                if (value == events[j].sceneEvent)
                {
                    founded = true;
                    break;
                }
            }
            if (!founded)
                indexs.Add(value);
        }

        for (int i = 0; i < indexs.Count; i++)
        {
            menu.AddItem(new GUIContent(Enum.GetName(typeof(SceneEventInteraction.SceneEvent),indexs[i])), false, OnAddNewEvent, new NewEventVariableParam() { list = list, serializedObject = serializedObject, sceneEvent = indexs[i] });
        }

        menu.ShowAsContext();
    }


    private static void OnAddNewEvent(object var)
    {
        NewEventVariableParam variable = (NewEventVariableParam)var;
        ReorderableList eventsList = variable.list;
        int elementIndex = eventsList.serializedProperty.arraySize;

        eventsList.serializedProperty.arraySize++;
        eventsList.index = elementIndex;
        var element = eventsList.serializedProperty.GetArrayElementAtIndex(elementIndex);
        element.FindPropertyRelative("sceneEvent").enumValueIndex = (int)variable.sceneEvent;

        variable.serializedObject.ApplyModifiedProperties();
    }

    internal class NewEventVariableParam
    {
        public ReorderableList list;
        public SerializedObject serializedObject;
        public SceneEventInteraction.SceneEvent sceneEvent;
    }

    public override void OnInspectorGUI()
    {
        SceneEvents myTarget = (SceneEvents)target;

        Rect rect = new Rect(15, 0, EditorGUIUtility.fieldWidth, EditorGUIUtility.singleLineHeight);

        eventsList.DoLayoutList();


        serializedObject.ApplyModifiedProperties();

        if (GUI.changed)
        {
            EditorUtility.SetDirty(target);
        }

    }

}
