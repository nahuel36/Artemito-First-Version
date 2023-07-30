using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultipleParametersTest : MonoBehaviour
{
    
    public void MultiplesTest(List<CustomArgument> arguments)
    {
        arguments[0].resultBool = false;

        foreach (PNCCharacter charact in FindObjectsOfType<PNCCharacter>())
        {
            if (charact.isPlayerCharacter)
            {
                charact.Talk(arguments[0].stringArgument);
                charact.Talk(arguments[1].stringArgument);
            }
        }
    }
}
