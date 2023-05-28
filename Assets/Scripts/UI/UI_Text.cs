using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Text : MonoBehaviour
{
    // Start is called before the first frame update
    public TMPro.TextMeshProUGUI text;
    Settings settings;
    RectTransform cursorRect;
    RectTransform textRect;
    private void Start()
    {
        text = GetComponentInChildren<TMPro.TextMeshProUGUI>();
        settings = Resources.Load<Settings>("Settings/Settings");
        cursorRect = FindObjectOfType<PNCCursor>().GetComponent<RectTransform>();
        textRect = GetComponent<RectTransform>();
    }

    private void Update()
    {
        if (settings.objetivePosition == Settings.ObjetivePosition.overCursor)
            textRect.anchoredPosition = cursorRect.anchoredPosition;
    }
}
