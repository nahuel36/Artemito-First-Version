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
    public Rect nodeRect;
}


[CreateAssetMenu(fileName = "New Dialog", menuName = "Pnc/Dialog", order = 1)]
public class Dialog : ScriptableObject
{
    public List<SubDialog> subDialogs;
    public int subDialogIndex;

    public void ChangeRect(int index, Rect rect)
    {
        for (int i = 0; i < subDialogs.Count; i++)
        {
            if (subDialogs[i].index == index)
            {
                subDialogs[i].nodeRect = rect;
            }
        }
    }


    public void ChangeText(int index, string text)
    {
        for (int i = 0; i < subDialogs.Count; i++)
        {
            if (subDialogs[i].index == index)
            {
                subDialogs[i].text = text;
            }
        }
    }

    public string GetText(int index)
    {
        for (int i = 0; i < subDialogs.Count; i++)
        {
            if (subDialogs[i].index == index)
            {
                return subDialogs[i].text;
            }
        }
        return "";
    }

}
