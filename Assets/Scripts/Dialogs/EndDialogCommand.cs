using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class EndDialogCommand : ICommand
{
    DialogsUI dialogsUI;
    public async Task Execute()
    {
        await Task.Yield();
        dialogsUI.EndDialog();
    }

    public void Skip()
    {
        
    }

    public void Queue(DialogsUI dialogsUI)
    {
        this.dialogsUI = dialogsUI;
        CommandsQueue.Instance.AddCommand(this);
    }
}
