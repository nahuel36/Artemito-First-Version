using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialogOption
{
    public string text;
    public AttempsContainer attempsContainer;
    public int subDialogDestinyIndex;
}

[System.Serializable]
public class SubDialog
{
    public string text;
    public int index;
    public List<DialogOption> options;

}


[CreateAssetMenu(fileName = "New Dialog", menuName = "Pnc/Dialog", order = 1)]
public class Dialog : ScriptableObject
{
    public List<SubDialog> subDialogs;
    public int subDialogIndex;
}
