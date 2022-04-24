using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointAndWalk : MonoBehaviour
{


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
       if(Input.GetMouseButtonDown(0))
        {
            Vector3 point = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            FindObjectOfType<PNCCharacter>().Walk(point.x * Vector3.right + point.y * Vector3.up);
        }
    }
}
