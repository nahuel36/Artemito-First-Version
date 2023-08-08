using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SayForInventory : MonoBehaviour
{
    public void Say(List<CustomArgument> arguments)
    {
        foreach (PNCCharacter charact in FindObjectsOfType<PNCCharacter>())
        {
            if (charact.isPlayerCharacter)
            {
                charact.Talk(arguments[0].stringArgument);
            }
        }
    }
}
