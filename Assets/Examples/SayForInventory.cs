using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SayForInventory : MultipleParametersScript
{
    public void Say(string text)
    {
        foreach (PNCCharacter charact in FindObjectsOfType<PNCCharacter>())
        {
            if (charact.isPlayerCharacter)
            {
                charact.Talk(text);
            }
        }
    }
}
