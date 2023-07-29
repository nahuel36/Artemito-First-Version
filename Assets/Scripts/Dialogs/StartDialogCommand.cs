using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class StartDialogCommand : ICommand
{
    DialogsUI dialogsUI;
    Dialog dialog;
    int subDialogIndex;

    public async Task Execute()
    {
        await Task.Yield();
        DialogsManager.Instance.activeDialog = dialog;
        DialogsManager.Instance.activeSubDialog = subDialogIndex;
        DialogsManager.Instance.waitingForTask = false;
        dialogsUI.StartDialog(dialog, subDialogIndex);
    }

    public void Skip()
    {
        
    }

    public void Queue(DialogsUI dialogsUI, Dialog dialog, int subDialogIndex)
    {
        this.dialogsUI = dialogsUI;
        this.dialog = dialog;
        this.subDialogIndex = subDialogIndex;
        CommandsQueue.Instance.AddCommand(this);
    }
}
