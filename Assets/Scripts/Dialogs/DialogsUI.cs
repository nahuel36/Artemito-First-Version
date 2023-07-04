using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DialogsUI : MonoBehaviour
{
    public GameObject option;
    public Dialog dialog;

    UnityEngine.UI.GraphicRaycaster raycaster;
    EventSystem eventSystem;
    [SerializeField] PNCCursor cursor;
    List<GameObject> options = new List<GameObject>();
    TMPro.TextMeshProUGUI lastOption;
    [SerializeField] Transform dialogsContainer;
    ScrollRect scrollRect;
    private void Start()
    {
        StartDialog(dialog);
        raycaster = GetComponentInParent<UnityEngine.UI.GraphicRaycaster>();

        eventSystem = FindObjectOfType<EventSystem>();
        scrollRect = GetComponentInChildren<ScrollRect>();
    }

    // Start is called before the first frame update
    void StartDialog(Dialog dialog)
    {
        options.Clear();
        options.Add(option);
        option.SetActive(true);
        option.GetComponent<TMPro.TextMeshProUGUI>().text = dialog.GetSubDialogByIndex(dialog.entryDialogIndex).options[0].text;
        for (int i = 1; i < dialog.GetSubDialogByIndex(dialog.entryDialogIndex).options.Count; i++)
        {
            GameObject optionGO = Instantiate(option, dialogsContainer);
            optionGO.GetComponent<TMPro.TextMeshProUGUI>().text = dialog.GetSubDialogByIndex(dialog.entryDialogIndex).options[i].text;
            options.Add(optionGO);
        }
        dialogsContainer.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(this.GetComponent<RectTransform>().sizeDelta.x, 50 * dialog.GetSubDialogByIndex(dialog.entryDialogIndex).options.Count);
    }

    public void MoveUp()
    {
        scrollRect.verticalNormalizedPosition += (1 / (float)options.Count) * dialogsContainer.transform.localScale.y * 2 ;
    }

    public void MoveDown() 
    {
        scrollRect.verticalNormalizedPosition -= (1 / (float)options.Count) * dialogsContainer.transform.localScale.y * 2;
    }

    private void Update()
    {
        PointerEventData pointerData = new PointerEventData(eventSystem);
        pointerData.position = cursor.GetPosition();

        List<RaycastResult> results = new List<RaycastResult>();
        raycaster.Raycast(pointerData, results);
        bool founded = false;

        if (lastOption != null)
        { 
            lastOption.color = Color.white;
        }

        TMPro.TextMeshProUGUI actualOption = null;

        foreach (RaycastResult result in results)
        {
            if (options.Contains(result.gameObject))
            { 
                actualOption = result.gameObject.GetComponent<TMPro.TextMeshProUGUI>();
                lastOption = actualOption;
            }
        }
        if(actualOption != null)
            actualOption.color = Color.gray;
    }


}
