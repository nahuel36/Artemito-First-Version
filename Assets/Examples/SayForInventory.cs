using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SayForInventory : MonoBehaviour
{
    public void Say(string text)
    {
        Debug.Log(text);
        FindObjectOfType<PNCCharacter>().Talk(text);
    }
}
