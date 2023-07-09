using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DialogsUI : MonoBehaviour
{
    public DialogOptionUI first_option;
    public Dialog dialog;
    public bool inActiveDialog;

    UnityEngine.UI.GraphicRaycaster raycaster;
    EventSystem eventSystem;
    [SerializeField] PNCCursor cursor;
    List<DialogOptionUI> options = new List<DialogOptionUI>();
    DialogOptionUI lastOption;
    [SerializeField] Transform dialogsContainer;
    ScrollRect scrollRect;
    [SerializeField] int visibleOptions = 3;
    private float initializedCounter;
    private void Start()
    {
        raycaster = GetComponentInParent<UnityEngine.UI.GraphicRaycaster>();
        eventSystem = FindObjectOfType<EventSystem>();
        scrollRect = GetComponentInChildren<ScrollRect>();
        DialogsManager.Instance.Initialize();
    }

    // Start is called before the first frame update
    public void StartDialog(Dialog dialog, int subDialogIndex)
    {
        initializedCounter = 0.5f;
        inActiveDialog = true;
        options.Clear();
        for (int i = dialogsContainer.childCount - 1; i > 0; i--)
        {
            Destroy(dialogsContainer.GetChild(i).gameObject);
        }
        int j = 0;
        int k = 0;
        while(j < dialog.GetSubDialogByIndex(subDialogIndex).options.Count)
        {
            if (dialog.GetSubDialogByIndex(subDialogIndex).options[j].currentState == DialogOption.current_state.initial)
            {
                if (dialog.GetSubDialogByIndex(subDialogIndex).options[j].initialState == DialogOption.state.disabled || 
                    dialog.GetSubDialogByIndex(subDialogIndex).options[j].initialState == DialogOption.state.disabled_forever)
                { 
                    j++;
                continue;
                }
            }
            else if (dialog.GetSubDialogByIndex(subDialogIndex).options[j].currentState == DialogOption.current_state.disabled || 
                dialog.GetSubDialogByIndex(subDialogIndex).options[j].currentState == DialogOption.current_state.disabled_forever)
            {
                j++;
                continue;
            }

            GameObject optionGO;
            if (k == 0)
                optionGO = first_option.container;
            else
                optionGO = Instantiate(first_option.container, dialogsContainer);
            optionGO.SetActive(true);
            DialogOptionUI dialogOptionUI = optionGO.GetComponent<DialogOptionUI>();
            dialogOptionUI.textContainer.text = dialog.GetSubDialogByIndex(subDialogIndex).options[j].text;
            dialogOptionUI.textContainer.color = Color.white;
            dialogOptionUI.container = optionGO;
            dialogOptionUI.dialogOption = dialog.GetSubDialogByIndex(subDialogIndex).options[j];
            options.Add(dialogOptionUI);
            InteractionUtils.InitializeInteractions(ref dialog.GetSubDialogByIndex(subDialogIndex).options[j].attempsContainer.attemps);
            j++;
            k++;
        }
    }

    public void MoveUp()
    {
        if(options.Count - visibleOptions > 0)
            scrollRect.verticalNormalizedPosition += ((float)1 / ((float)options.Count-visibleOptions));
    }

    public void MoveDown() 
    {
        if (options.Count - visibleOptions > 0)
            scrollRect.verticalNormalizedPosition -= ((float)1 / ((float)options.Count-visibleOptions));
    }

    private void Update()
    {
        PointerEventData pointerData = new PointerEventData(eventSystem);
        pointerData.position = cursor.GetPosition();

        List<RaycastResult> results = new List<RaycastResult>();
        raycaster.Raycast(pointerData, results);

        if (initializedCounter > 0)
        {
            initializedCounter -= Time.deltaTime;
        }

        DialogOptionUI actualOption = null;

        foreach (RaycastResult result in results)
        {
            DialogOptionUI overOption = result.gameObject.GetComponent<DialogOptionUI>();
            if (overOption != null && options.Contains(overOption))
            {
                if(lastOption != null)
                    lastOption.textContainer.color = Color.white;
                actualOption = overOption;
                lastOption = actualOption;
            }
        }
        if (actualOption != null)
        { 
            actualOption.textContainer.color = Color.gray;
            if (Input.GetMouseButtonUp(0) && initializedCounter <= 0)
            {
                InteractionUtils.RunAttempsInteraction(actualOption.dialogOption.attempsContainer);
                int destiny = actualOption.dialogOption.subDialogDestinyIndex;
                if(destiny > 0)
                    DialogsManager.Instance.StartDialog(dialog, destiny);//queue
            }
        }

    }


}
