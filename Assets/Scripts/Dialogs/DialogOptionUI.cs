using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DialogOptionUI : MonoBehaviour
{
    public TMPro.TextMeshProUGUI textContainer;
    [HideInInspector]
    public DialogOption dialogOption;
    public GameObject container;
}
