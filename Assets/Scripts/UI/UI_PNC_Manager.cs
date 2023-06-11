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
    string GetPointerString() {
        for (int i = 0; i < settings.cursorPrioritys.Count; i++)
        {
            switch (settings.cursorPrioritys[i])
            {
                case Settings.PriorityOnCursor.VerbSelected:
                    if (!string.IsNullOrEmpty(verbsUI.actualVerb))
                        return verbsUI.actualVerb;
                    break;
                case Settings.PriorityOnCursor.OverVerb:
                    if (!string.IsNullOrEmpty(verbsUI.overCursorVerb))
                        return verbsUI.overCursorVerb;
                    break;
                case Settings.PriorityOnCursor.InventorySelected:
                    if (itemActive != null)
                        return itemActive.itemName;
                    break;
                case Settings.PriorityOnCursor.OverInventory:
                    if (inventoryUI.overInventory != null)
                        return inventoryUI.overInventory.itemName;
                    break;
                case Settings.PriorityOnCursor.CharacterOrObjectSelected:
                    if (objetiveClicked != null)
                        return objetiveClicked.name;
                    break;
                case Settings.PriorityOnCursor.OverCharacterOrObject:
                    if (objetive.actualObject != null)
                        return objetive.actualObject.name;
                    break;
                default:
                    break;
            }
        }
        return "";
    }

    // Update is called once per frame
    void Update()
    {
        if (CommandsQueue.Instance.Executing()) return; //No permite cancelar caminata    

        ui_text.text.text = GetPointerString();
        
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
                        objetive.actualObject.RunVerbInteraction(verbsUI.actualVerb);
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
                        objetiveClicked.RunVerbInteraction(verbsUI.overCursorVerb);

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
