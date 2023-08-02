using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomSayWalkScene : MonoBehaviour, SayScript
{
    public string SayWithScript(List<CustomArgument> arguments)
    {
        return "el tiempo del juego es " +Time.realtimeSinceStartup.ToString() + " " + arguments[0].stringArgument ;
    }
}
