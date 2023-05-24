using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Text : MonoBehaviour
{
    // Start is called before the first frame update
    public TMPro.TextMeshProUGUI text;

    private void Start()
    {
        text = GetComponentInChildren<TMPro.TextMeshProUGUI>();
    }
}
