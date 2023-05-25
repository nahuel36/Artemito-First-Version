using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
public class VerbsUI : MonoBehaviour
{
    Settings settings;
    [HideInInspector]public string actualVerb;
    [HideInInspector] public string overCursorVerb;
    UnityEngine.UI.GraphicRaycaster raycaster;
    EventSystem eventSystem;
    PNCCursor cursor;
    List<Button> activeButtons;
    List<string> activeVerbs;
    // Start is called before the first frame update
    void Start()
    {
        settings = Resources.Load<Settings>("Settings/Settings");

        if (settings.interactionExecuteMethod == Settings.InteractionExecuteMethod.FirstObjectThenAction)
            HideAllVerbs();
        else if (settings.interactionExecuteMethod == Settings.InteractionExecuteMethod.FirstActionThenObject)
            ShowAllVerbs();

        raycaster = GetComponentInParent<UnityEngine.UI.GraphicRaycaster>();
        eventSystem = FindObjectOfType<EventSystem>();
        cursor = GameObject.FindObjectOfType<PNCCursor>();

    }

    public void HideAllVerbs()
    {
        Button[] buttons = GetComponentsInChildren<Button>();
        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].gameObject.SetActive(false);
        }
        
    }

    public void ShowAllVerbs()
    {
        activeButtons = new List<Button>();
        activeVerbs = new List<string>();
        Button[] buttons = GetComponentsInChildren<Button>(true);
        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].gameObject.SetActive(true);
            activeButtons.Add(buttons[i]);
            string verb = settings.verbs[i];
            buttons[i].onClick.AddListener(() => ChangeActiveVerb(verb));
            activeVerbs.Add(settings.verbs[i]);
        }
    }

    public void ChangeActiveVerb(string verb) 
    {
        actualVerb = verb;
    }

    internal void ResetActualVerb()
    {
        actualVerb = "";
    }

    public void ShowVerbs(string[] verbs)
    {
        gameObject.SetActive(true);

        activeButtons = new List<Button>();
        activeVerbs = new List<string>();

        Button[] buttons = GetComponentsInChildren<Button>(true);
        for (int i = 0; i < verbs.Length; i++)
        {
            buttons[i].gameObject.SetActive(true);
            buttons[i].onClick.RemoveAllListeners();
            string verb = verbs[i];
            buttons[i].onClick.AddListener(() => ChangeActiveVerb(verb));
            activeButtons.Add(buttons[i]);
            activeVerbs.Add(verbs[i]);
        }

    }
    private void Update()
    {
        overCursorVerb = "";
        PointerEventData pointerData = new PointerEventData(eventSystem);
        pointerData.position = cursor.GetPosition();

        List<RaycastResult> results = new List<RaycastResult>();
        raycaster.Raycast(pointerData, results);
        foreach (RaycastResult result in results)
        {
            for (int i = 0; i < activeButtons.Count; i++)
            {
                if (result.gameObject == activeButtons[i].gameObject)
                {
                    overCursorVerb = activeVerbs[i];
                }
            }
        }
    }
}
