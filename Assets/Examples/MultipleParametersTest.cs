using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultipleParametersTest : MultipleParametersScript
{
    
    public void MultiplesTest()
    { 
        foreach (PNCCharacter charact in FindObjectsOfType<PNCCharacter>())
        {
            if (charact.isPlayerCharacter)
            {
                charact.Talk(interaction.customActionArguments[0].stringArgument);
                charact.Talk(interaction.customActionArguments[1].stringArgument);
            }
        
        }
    }
}
