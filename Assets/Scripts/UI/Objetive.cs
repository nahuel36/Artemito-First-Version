using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class Objetive : MonoBehaviour
{
    [HideInInspector]public PNCInteractuable overInteractuable;
    PNCCursor cursor;

    // Start is called before the first frame update
    void Start()
    {
        cursor = GameObject.FindObjectOfType<PNCCursor>();
       
    }

    // Update is called once per frame
    void Update()
    {
        overInteractuable = null;
        RaycastHit2D[] hits = Physics2D.RaycastAll(cursor.transform.position,Vector2.zero);
        int bestpriority = -1;
        foreach(RaycastHit2D hit in hits)
        {
            if (hit.collider)
            {
                PNCInteractuable pncInteractuable = hit.collider.GetComponent<PNCInteractuable>();
                if (pncInteractuable && pncInteractuable.priority > bestpriority)
                { 
                    overInteractuable = pncInteractuable;
                    bestpriority = pncInteractuable.priority;
                }
            }
        }

    }
}
