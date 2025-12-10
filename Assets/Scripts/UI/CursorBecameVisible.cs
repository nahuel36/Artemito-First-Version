using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorBecameVisible : MonoBehaviour
{


    private void OnBecameVisible()
    {
        Cursor.visible = false;
    }

    private void OnBecameInvisible()
    {
        Cursor.visible = true;
    }
   
}
