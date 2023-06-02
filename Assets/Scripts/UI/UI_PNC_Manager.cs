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
    InventoryUI inventoryUI;
    InventoryItem itemActive;
    // Start is called before the first frame update
    void Start()
    {
        settings = Resources.Load<Settings>("Settings/Settings");
        verbsUI = FindObjectOfType<VerbsUI>();
        objetive = FindObjectOfType<Objetive>();
        pointAndWalk = FindObjectOfType<PointAndWalk>();
        ui_text = FindObjectOfType<UI_Text>();
        inventoryUI = FindObjectOfType<InventoryUI>();
    }

    // Update is called once per frame
    void Update()
    {
        if (CommandsQueue.Instance.Executing()) return; //No permite cancelar caminata    

        ui_text.text.text = "";

        if (inventoryUI.overInventory != null)
        {
            ui_text.text.text = inventoryUI.overInventory.itemName;
        }
        if (itemActive != null)
        {
            ui_text.text.text = itemActive.itemName;
            if (objetive.actualObject != null)
                ui_text.text.text += " en " + objetive.actualObject.name;
            else if(inventoryUI.overInventory != null && inventoryUI.overInventory != itemActive)
                ui_text.text.text += " en " + inventoryUI.overInventory.itemName;
        }
        else if (!string.IsNullOrEmpty(verbsUI.actualVerb))
        {
            ui_text.text.text = verbsUI.actualVerb;
            if (objetive.actualObject != null)
                ui_text.text.text += " " + objetive.actualObject.name;
        }
        else if (!string.IsNullOrEmpty(verbsUI.overCursorVerb))
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
            if (inventoryUI.overInventory != null && itemActive == null)
            {
                itemActive = inventoryUI.overInventory;
                objetiveClicked = null;
                verbsUI.ResetActualVerb();
                return;
            }

            if (settings.interactionExecuteMethod == Settings.InteractionExecuteMethod.FirstActionThenObject)
            {
                if (!string.IsNullOrEmpty(verbsUI.overCursorVerb))
                    return;
                if (!string.IsNullOrEmpty(verbsUI.actualVerb))
                {
                    if (objetive.actualObject != null)
                    {
                        objetive.actualObject.RunInteraction(verbsUI.actualVerb);
                        verbsUI.ResetActualVerb();
                    }
                    else
                    {
                        verbsUI.ResetActualVerb();
                    }
                }
                else if (itemActive != null)
                {
                    if (objetive.actualObject != null)
                    {
                        objetive.actualObject.RunInventoryInteraction(itemActive);
                        itemActive = null;
                    }
                    else if (inventoryUI.overInventory != null)
                    {
                        InventoryManager.Instance.RunInventoryInteraction(itemActive, inventoryUI.overInventory);
                        itemActive = null;
                    }
                    else
                        itemActive = null;
                }
                else
                {
                    pointAndWalk.WalkCancelable();
                    verbsUI.ResetActualVerb();
                }
            }
            else if (settings.interactionExecuteMethod == Settings.InteractionExecuteMethod.FirstObjectThenAction)
            {
                if (itemActive != null && objetive.actualObject != null)
                {
                    objetive.actualObject.RunInventoryInteraction(itemActive);
                    itemActive = null;
                }
                else if (itemActive != null && inventoryUI.overInventory != null)
                {
                    InventoryManager.Instance.RunInventoryInteraction(itemActive, inventoryUI.overInventory);
                    itemActive = null;
                }
                else if (objetiveClicked != null)
                {
                    if (!string.IsNullOrEmpty(verbsUI.overCursorVerb))
                        objetiveClicked.RunInteraction(verbsUI.overCursorVerb);

                    objetiveClicked = null;
                    verbsUI.HideAllVerbs();
                    verbsUI.ResetActualVerb();
                }
                else if (objetive.actualObject != null)
                {
                    if (itemActive == null)
                    {
                        verbsUI.ShowVerbs(objetive.actualObject.getActiveVerbs());
                        objetiveClicked = objetive.actualObject;
                    }

                }
                else
                {
                    itemActive = null;
                    objetiveClicked = null;
                    verbsUI.HideAllVerbs();
                    verbsUI.ResetActualVerb();
                    pointAndWalk.WalkCancelable();
                }
            }
        }

    }
}
