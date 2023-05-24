using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_PNC_Manager : MonoBehaviour
{
    Settings settings;
    VerbsUI verbsUI;
    Objetive objetive;
    PointAndWalk pointAndWalk;
    PNCInteractuable objetiveClicked;
    UI_Text ui_text;
    // Start is called before the first frame update
    void Start()
    {
        settings = Resources.Load<Settings>("Settings/Settings");
        verbsUI = FindObjectOfType<VerbsUI>();
        objetive = FindObjectOfType<Objetive>();
        pointAndWalk = FindObjectOfType<PointAndWalk>();
        ui_text = FindObjectOfType<UI_Text>();
    }

    // Update is called once per frame
    void Update()
    {
        ui_text.text.text = "";

        if (!string.IsNullOrEmpty(verbsUI.overCursorVerb))
        {
            ui_text.text.text = verbsUI.overCursorVerb;
            if (objetiveClicked)
                ui_text.text.text += " " + objetiveClicked.name;        
        }
        else if (objetiveClicked)
        {
            ui_text.text.text = objetiveClicked.name;
        }
        else if (objetive.actualObject)
        {
            ui_text.text.text = objetive.actualObject.name;
        }
        
        if (Input.GetMouseButtonDown(0))
        {
            if (settings.interactionExecuteMethod == Settings.InteractionExecuteMethod.FirstActionThenObject)
            {
                if (!string.IsNullOrEmpty(verbsUI.overCursorVerb))
                {
                    if (objetive.actualObject != null)
                    {
                        objetive.actualObject.RunInteraction(verbsUI.actualVerb);
                    }
                    else
                    {
                        verbsUI.ResetActualVerb();
                    }
                }
                else
                {
                    pointAndWalk.WalkCancelable();
                }
            }
            else if (settings.interactionExecuteMethod == Settings.InteractionExecuteMethod.FirstObjectThenAction)
            {
                if (objetiveClicked != null)
                {
                    if(!string.IsNullOrEmpty(verbsUI.overCursorVerb))
                        objetiveClicked.RunInteraction(verbsUI.overCursorVerb);
                    objetiveClicked = null;
                    verbsUI.HideAllVerbs();
                }
                else if (objetive.actualObject != null)
                {
                     verbsUI.ShowVerbs(objetive.actualObject.getActiveVerbs());
                     objetiveClicked = objetive.actualObject;
                }
                else 
                {
                    objetiveClicked = null;
                    verbsUI.HideAllVerbs();
                    pointAndWalk.WalkCancelable();
                }
            }
        }

    }
}
