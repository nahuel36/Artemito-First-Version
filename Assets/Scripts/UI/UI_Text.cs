using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Text : MonoBehaviour
{
    // Start is called before the first frame update
    public TMPro.TextMeshProUGUI text;
    Settings settings;
    PNCCursor cursor;
    private void Start()
    {
        text = GetComponentInChildren<TMPro.TextMeshProUGUI>();
        settings = Resources.Load<Settings>("Settings/Settings");
        cursor = FindObjectOfType<PNCCursor>();
    }

    private void Update()
    {
        if (settings.objetivePosition == Settings.ObjetivePosition.overCursor)
            GetComponent<RectTransform>().anchoredPosition = cursor.GetComponent<RectTransform>().anchoredPosition;
    }
}
