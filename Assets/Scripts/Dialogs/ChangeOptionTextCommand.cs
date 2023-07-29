using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
public class ChangeOptionTextCommand : ICommand
{
    Dialog dialog;
    int subDialogIndex;
    int optionIndex;
    string newText;

    public async Task Execute()
    {
        await Task.Yield();

        for (int i = 0; i < dialog.subDialogs.Count; i++)
        {
            if (dialog.subDialogs[i].index == subDialogIndex)
            {
                for (int j = 0; j < dialog.subDialogs[i].options.Count; j++)
                {
                    if (dialog.subDialogs[i].options[j].index == optionIndex)
                    {
                        dialog.subDialogs[i].options[j].currentText = newText;
                    }
                }
            }
        }
    }

    public void Skip()
    {

    }

    public void Queue(Dialog dialog, int subDialogIndex, int optionIndex, string newText)
    {
        this.dialog = dialog;
        this.subDialogIndex = subDialogIndex;
        this.optionIndex = optionIndex;
        this.newText = newText;
        CommandsQueue.Instance.AddCommand(this);
    }
}
