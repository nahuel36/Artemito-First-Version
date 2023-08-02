using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Threading.Tasks;



public class UI_PNC_Manager : MonoBehaviour
{
    [System.Serializable]
    public class ActiveThing
    {
        private InventoryItem itemActive;
        private PNCInteractuable interactuableActive;
        private PNCInteractuable interactuableAsInventory;
        public bool IsActive()
        {
            return isInventoryItemActive() || interactuableActive != null || interactuableAsInventory != null;
        }
        public bool isInventoryItemActive()
        {
            return (itemActive != null && itemActive.specialIndex != -1);
        }    


        public InventoryItem GetInventoryActive()
        {
            if (isInventoryItemActive())
                return itemActive;
            else
                return null;
        }

        public PNCInteractuable GetInteractuable()
        {
            return interactuableActive;
        }

        public PNCInteractuable GetInteractuableAsInventory() 
        {
            return interactuableAsInventory;
        }

        public string GetName()
        {
            if (IsActive())
            {
                if (isInventoryItemActive())
                    return itemActive.itemName;
                if (interactuableActive != null)
                    return interactuableActive.name;
                if (interactuableAsInventory != null)
                    return interactuableAsInventory.name;
            }
            return "";
        }

        public void SetInteractuable(PNCInteractuable interactuable)
        {
            interactuableActive = interactuable;
            interactuableAsInventory = null;
            itemActive = null;
        }

        public void SetInventoryItem(InventoryItem item)
        {
            itemActive = item;
            interactuableActive = null;
            interactuableAsInventory = null;
        }

        public void SetInteractuableAsInventory(PNCInteractuable interactuable)
        {
            interactuableAsInventory = interactuable;
            itemActive = null;
            interactuableActive = null;
        }

        public void RunVerbInteraction(Verb verb)
        {
            if (isInventoryItemActive())
                itemActive.RunVerbInteraction(verb);
            if (interactuableActive != null)
                interactuableActive.RunVerbInteraction(verb);
        }

        public Verb[] GetActiveVerbs() {
            if (isInventoryItemActive())
                return itemActive.GetActiveVerbs();
            if (interactuableActive != null)
                return interactuableActive.GetActiveVerbs();
            return null;
        }

        public void Clear()
        {
            itemActive = null;
            interactuableActive = null;
            interactuableAsInventory = null;
        }
    }

    private static UI_PNC_Manager instance;

    public static UI_PNC_Manager Instance 
    {
        get
        {
            return instance;
        }
    }

    Settings settings;
    VerbsUI verbsUI;
    Objetive objetive;
    PointAndWalk pointAndWalk;
    ActiveThing activeThing;
    UI_Text ui_text;
    InventoryUI inventoryUI;
    DialogsUI dialogsUI;
    float holdingCounter;
    bool showingVerbs;
    string cursorTextString;
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        activeThing = new ActiveThing();
        cursorTextString = "";
        settings = Resources.Load<Settings>("Settings/Settings");
        verbsUI = FindObjectOfType<VerbsUI>();
        objetive = FindObjectOfType<Objetive>();
        pointAndWalk = FindObjectOfType<PointAndWalk>();
        ui_text = FindObjectOfType<UI_Text>();
        inventoryUI = FindObjectOfType<InventoryUI>();
        dialogsUI = FindObjectOfType<DialogsUI>();
    }

    public void ReInitialize() {
        pointAndWalk = FindObjectOfType<PointAndWalk>();
    }

    void ProcessPointer(ref UnityAction onClickDown,ref UnityAction onClickUp, ref UnityAction onClickHold) 
    {

        if (settings.interactionExecuteMethod == Settings.InteractionExecuteMethod.FirstObjectThenAction)
        { 

            onClickHold = () =>
            {
                if (!showingVerbs && objetive.overGoToScene == null)
                {
                    if (inventoryUI.overInventory != null && (!activeThing.IsActive() || activeThing.GetInventoryActive() != inventoryUI.overInventory))
                    {
                        activeThing.SetInventoryItem(inventoryUI.overInventory);
                        holdingCounter = 0;
                    }
                    else if (objetive.overInteractuable != null && (!activeThing.IsActive() || activeThing.GetInteractuable() != objetive.overInteractuable))
                    {
                        activeThing.SetInteractuable(objetive.overInteractuable);
                        holdingCounter = 0;
                    }
                    else if (objetive.overInteractuable != null || inventoryUI.overInventory != null)
                    {
                        holdingCounter += Time.deltaTime;
                    }

                    if (holdingCounter > 0.75f && activeThing.IsActive())
                    {
                        showingVerbs = true;
                        verbsUI.ShowVerbs(activeThing.GetActiveVerbs());
                    }
                }
            };

            onClickDown = () =>
            {
                if (!showingVerbs)
                {
                    if (objetive.overGoToScene != null)
                    {
                        objetive.overGoToScene.Go();
                        objetive.overGoToScene = null;
                    }
                    else if (activeThing.GetInventoryActive() != null || activeThing.GetInteractuableAsInventory() != null)
                    {
                        if (inventoryUI.overInventory != null && inventoryUI.overInventory != activeThing.GetInventoryActive())
                        {
                            InventoryManager.Instance.RunInventoryInteraction(inventoryUI.overInventory, activeThing.GetInventoryActive(), verbsUI.selectedVerb);
                        }
                        else if (activeThing.GetInventoryActive() != null && inventoryUI.overInventory == null && objetive.overInteractuable != null
                        && (verbsUI.selectedVerb.isLikeUse || (verbsUI.selectedVerb.isLikeGive && objetive.overInteractuable is PNCCharacter)))
                        {
                            objetive.overInteractuable.RunInventoryInteraction(activeThing.GetInventoryActive(), verbsUI.selectedVerb);
                        }
                        else if (activeThing.GetInteractuableAsInventory() != null && inventoryUI.overInventory == null && objetive.overInteractuable != null)
                        {
                            objetive.overInteractuable.RunObjectAsInventoryInteraction(activeThing.GetInteractuableAsInventory(), verbsUI.selectedVerb);
                        }
                        activeThing.Clear();
                    }
                    else if(inventoryUI.overInventory == null && objetive.overInteractuable == null && !DialogsManager.Instance.InActiveDialog)
                    {
                        pointAndWalk.WalkCancelable();
                    }
                    
                    holdingCounter = 0;
                }
                else
                {
                    if (verbsUI.overCursorVerb != null)
                    {
                        if (inventoryUI.overInventory != null && verbsUI.overCursorVerb.isLikeUse || verbsUI.overCursorVerb.isLikeGive)
                        {
                            activeThing.SetInventoryItem(inventoryUI.overInventory);
                            verbsUI.selectedVerb = verbsUI.overCursorVerb;
                        }
                        else
                        {
                            activeThing.RunVerbInteraction(verbsUI.overCursorVerb);
                            if (activeThing.GetInteractuableAsInventory() == null)
                            { 
                                activeThing.Clear();
                            }
                            verbsUI.selectedVerb = verbsUI.overCursorVerb;
                        }
                    }
                    holdingCounter = 0;
                    showingVerbs = false;
                    verbsUI.HideAllVerbs();
                }
            };


            if (objetive.overGoToScene)
            {
                cursorTextString = objetive.overGoToScene.cursorName;
            }
            else if (verbsUI.overCursorVerb != null && activeThing.IsActive())
            { 
                cursorTextString = verbsUI.overCursorVerb.name + " " + activeThing.GetName();
            }
            else if (activeThing.GetInteractuable() != null && showingVerbs)
            {
                cursorTextString = activeThing.GetName();
            }
            else if (activeThing.GetInventoryActive() != null && !showingVerbs && !string.IsNullOrEmpty(verbsUI.selectedVerb.name))
            {
                if (inventoryUI.overInventory != null && activeThing.GetInventoryActive() != null && inventoryUI.overInventory != activeThing.GetInventoryActive())
                    cursorTextString = verbsUI.selectedVerb.name + " " + activeThing.GetName() + " on " + inventoryUI.overInventory.itemName;
                else if (objetive.overInteractuable != null && activeThing.GetInventoryActive() != null && inventoryUI.overInventory == null)
                    cursorTextString = verbsUI.selectedVerb.name + " " + activeThing.GetName() + " on " + objetive.overInteractuable.name;
                else if (inventoryUI.overInventory == null)
                    cursorTextString = verbsUI.selectedVerb.name + " " + activeThing.GetName() + " on ";
            }
            else if (activeThing.GetInteractuableAsInventory() != null && !showingVerbs)
            {
                if (objetive.overInteractuable != null && activeThing.GetInteractuableAsInventory() != null && inventoryUI.overInventory == null && objetive.overInteractuable != activeThing.GetInteractuableAsInventory())
                    cursorTextString = verbsUI.selectedVerb.name + " " + activeThing.GetName() + " on " + objetive.overInteractuable.name;
                else if (inventoryUI.overInventory == null)
                    cursorTextString = verbsUI.selectedVerb.name + " " + activeThing.GetName() + " on ";
            }
            else if (!showingVerbs && inventoryUI.overInventory != null)
                cursorTextString = inventoryUI.overInventory.itemName;
            else if (objetive.overInteractuable)
                cursorTextString = objetive.overInteractuable.name;
            else
                cursorTextString = "";

        }
    }

    public async Task SetInteractuableAsInventory() {
        await Task.Delay(250);//TODO: Check in other platforms
        activeThing.SetInteractuableAsInventory(activeThing.GetInteractuable());
    }


    // Update is called once per frame
    void Update()
    {
        if (CommandsQueue.Instance.Executing()) return; //No permite cancelar caminata    

        UnityAction OnMouseDown = ()=> { };
        UnityAction OnMouseUp = () => { };
        UnityAction OnMouseHold = () => { };
        
        ProcessPointer(ref OnMouseDown,ref OnMouseUp, ref OnMouseHold);

        ui_text.text.text = cursorTextString;
        
        if (Input.GetMouseButtonDown(0))
            OnMouseDown.Invoke();
        if(Input.GetMouseButton(0))
            OnMouseHold.Invoke();
        if (Input.GetMouseButtonUp(0))
            OnMouseUp.Invoke();
        /*
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
                if (verbsUI.overCursorVerb != null)
                    return;
                if (verbsUI.selectedVerb != null)
                {
                    if (objetive.actualObject != null)
                    {
                        objetive.actualObject.RunVerbInteraction(verbsUI.selectedVerb);
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
                    if (verbsUI.overCursorVerb != null)
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
        }*/

    }
}
