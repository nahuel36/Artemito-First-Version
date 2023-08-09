using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SayForInventory : MonoBehaviour
{
    public void Say(List<CustomArgument> arguments)
    {
        PNCInteractuable interactuable = null;
        foreach (PNCInteractuable inter in FindObjectsOfType<PNCInteractuable>())
        {
            if (inter.GetInstanceID() == arguments[0].interactuableID)
            {
                interactuable = inter;
            }
        }

        foreach (PNCCharacter charact in FindObjectsOfType<PNCCharacter>())
        {
            if (charact.isPlayerCharacter)
            {
               charact.Talk(arguments[0].stringArgument + " " + interactuable.interactuableName);
            }
        }
    }
}
