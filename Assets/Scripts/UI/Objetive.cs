using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class Objetive : MonoBehaviour
{
    [HideInInspector]public PNCInteractuable actualObject;
    Cursor cursor;

    // Start is called before the first frame update
    void Start()
    {
        cursor = GameObject.FindObjectOfType<Cursor>();
       
    }

    // Update is called once per frame
    void Update()
    {
        actualObject = null;
        RaycastHit2D[] hits = Physics2D.RaycastAll(cursor.transform.position,Vector2.zero);
        foreach(RaycastHit2D hit in hits)
        {
            if (hit.collider)
            {
                PNCInteractuable pncInteractuable = hit.collider.GetComponent<PNCInteractuable>();
                if (pncInteractuable)
                    actualObject = pncInteractuable;
            }
        }

    }
}
