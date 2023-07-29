using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DialogsUI : MonoBehaviour
{
    public DialogOptionUI first_option;
    UnityEngine.UI.GraphicRaycaster raycaster;
    EventSystem eventSystem;
    [SerializeField] PNCCursor cursor;
    List<DialogOptionUI> active_ui_options = new List<DialogOptionUI>();
    DialogOptionUI lastOption;
    [SerializeField] Transform dialogContainer;
    [SerializeField] Transform optionsContainer;
    ScrollRect scrollRect;
    [SerializeField] int visibleOptions = 3;
    private float clickInOptionDelayCounter;

    private void Start()
    {
        dialogContainer.gameObject.SetActive(false);
        raycaster = GetComponentInParent<UnityEngine.UI.GraphicRaycaster>();
        eventSystem = FindObjectOfType<EventSystem>();
        scrollRect = dialogContainer.GetComponentInChildren<ScrollRect>();
    }

    // Start is called before the first frame update
    public void StartDialog(Dialog dialog, int subDialogIndex)
    {
        dialogContainer.gameObject.SetActive(true);
        clickInOptionDelayCounter = 0.5f;
        active_ui_options.Clear();
        for (int i = optionsContainer.childCount - 1; i > 0; i--)
        {
            Destroy(optionsContainer.GetChild(i).gameObject);
        }
        int j = 0;
        int k = 0;
        while(j < dialog.GetSubDialogByIndex(subDialogIndex).options.Count)
        {
            if (dialog.GetSubDialogByIndex(subDialogIndex).options[j].currentState == DialogOption.current_state.disabled || 
                dialog.GetSubDialogByIndex(subDialogIndex).options[j].currentState == DialogOption.current_state.disabled_forever)
            {
                j++;
                continue;
            }

            GameObject optionGO;
            if (k == 0)
                optionGO = first_option.container;
            else
                optionGO = Instantiate(first_option.container, optionsContainer);
            optionGO.SetActive(true);
            DialogOptionUI dialogOptionUI = optionGO.GetComponent<DialogOptionUI>();
            dialogOptionUI.textContainer.text = dialog.GetSubDialogByIndex(subDialogIndex).options[j].currentText;
            dialogOptionUI.textContainer.color = Color.white;
            dialogOptionUI.container = optionGO;
            dialogOptionUI.dialogOption = dialog.GetSubDialogByIndex(subDialogIndex).options[j];
            active_ui_options.Add(dialogOptionUI);
            j++;
            k++;
        }
        scrollRect.verticalNormalizedPosition = 1;
    }

    public void HideDialog()
    {
        active_ui_options.Clear();
        for (int i = optionsContainer.childCount - 1; i > 0; i--)
        {
            Destroy(optionsContainer.GetChild(i).gameObject);
        }
        first_option.container.SetActive(false);
        dialogContainer.gameObject.SetActive(false);
    }

    public void MoveUp()
    {
        if(active_ui_options.Count - visibleOptions > 0)
            scrollRect.verticalNormalizedPosition += ((float)1 / ((float)active_ui_options.Count-visibleOptions));
    }

    public void MoveDown() 
    {
        if (active_ui_options.Count - visibleOptions > 0)
            scrollRect.verticalNormalizedPosition -= ((float)1 / ((float)active_ui_options.Count-visibleOptions));
    }

    private void Update()
    {
        PointerEventData pointerData = new PointerEventData(eventSystem);
        pointerData.position = cursor.GetPosition();

        List<RaycastResult> results = new List<RaycastResult>();
        raycaster.Raycast(pointerData, results);

        if (clickInOptionDelayCounter > 0)
        {
            clickInOptionDelayCounter -= Time.deltaTime;
        }

        DialogOptionUI actualOption = null;

        bool buttonFounded = false;
        foreach (RaycastResult result in results)
        {
            DialogOptionUI overOption = result.gameObject.GetComponent<DialogOptionUI>();
            if (overOption != null && active_ui_options.Contains(overOption))
            {
                if (lastOption != null)
                    lastOption.textContainer.color = Color.white;
                actualOption = overOption;
                lastOption = actualOption;
            }
            if (result.gameObject.GetComponent<Button>())
            {
                buttonFounded = true;
            }
        }
        if (buttonFounded && actualOption != null)
        {
            actualOption.textContainer.color = Color.white;
            actualOption = null;
            if (lastOption != null)
                lastOption.textContainer.color = Color.white;
        }
        if (actualOption != null)
        { 
            actualOption.textContainer.color = Color.gray;
            if (Input.GetMouseButtonUp(0) && clickInOptionDelayCounter <= 0)
            {
                DialogsManager.Instance.OnClickOnOption(actualOption.dialogOption);
                
            }
        }
       
    }


}
