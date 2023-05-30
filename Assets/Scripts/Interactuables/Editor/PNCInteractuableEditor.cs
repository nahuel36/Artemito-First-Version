using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
public class PNCInteractuableEditor : PNCVariablesContainerEditor
{
    Dictionary<string, ReorderableList> verbAttempsListDict = new Dictionary<string, ReorderableList>();
    Dictionary<string, ReorderableList> verbInteractionsListDict = new Dictionary<string, ReorderableList>();
    protected ReorderableList verbsList;


    
    protected void InitializeVerbs() 
    {
        PNCInteractuable myTarget = (PNCInteractuable)target;

        SerializedProperty verbs_serialized = serializedObject.FindProperty("verbs");

        settings = Resources.Load<Settings>("Settings/Settings");

        verbsList = new ReorderableList(serializedObject, verbs_serialized, true, true, false, false)
        {
            drawHeaderCallback = (rect) =>
            {
                EditorGUI.LabelField(rect, "verbs");
            },
            elementHeightCallback = (int indexV) =>
            {
                return PNCEditorUtils.GetAttempsContainerHeight(verbs_serialized, myTarget.verbs[indexV].attemps,indexV);
            },
            drawElementCallback = (rect, indexV, active, focus) =>
            {
                PNCEditorUtils.DrawElementAttempContainer(verbs_serialized, indexV, rect, verbAttempsListDict, verbInteractionsListDict, myTarget.verbs[indexV].attemps);
            }
        };



        bool verbAdded = false;
        List<Verb> interactionsTempList = new List<Verb>();
        for (int i = 0; i < settings.verbs.Length; i++)
        {
            bool founded = false;
            for (int j = 0; j < myTarget.verbs.Count; j++)
            {
                if (myTarget.verbs[j].name == settings.verbs[i])
                {
                    interactionsTempList.Add((myTarget).verbs[j]);
                    founded = true;
                }
            }
            if (founded == false)
            {
                verbAdded = true;
                Verb tempVerb = new Verb();
                tempVerb.name = settings.verbs[i];
                tempVerb.attemps = new List<InteractionsAttemp>();
                interactionsTempList.Add(tempVerb);
            }
        }

        bool verbAdded2 = false;
        for (int i = 0; i < myTarget.verbs.Count; i++)
        {
            bool contains = false;
            for (int j = 0; j < interactionsTempList.Count; j++)
            {
                if (myTarget.verbs[i].name == interactionsTempList[j].name)
                {
                    contains = true;
                }
            }
            if (contains == false)
            {
                verbAdded2 = true;
                interactionsTempList.Add(myTarget.verbs[i]);
            }
        }

        if (verbAdded || verbAdded2)
            myTarget.verbs = interactionsTempList;

    }




}
