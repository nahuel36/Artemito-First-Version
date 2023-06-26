using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialogOption
{
    public string name;
    public AttempsContainer attempsContainer;
}


[CreateAssetMenu(fileName = "New Dialog", menuName = "Pnc/Dialog", order = 1)]
public class Dialog : ScriptableObject
{
    public List<DialogOption> options;
    
    
}
