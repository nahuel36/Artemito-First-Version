using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class ChangeEntryDialogCommand : ICommand
{
    Dialog dialog;
    int entry;


    public async Task Execute()
    {
        await Task.Yield();
        dialog.current_entryDialogIndex = entry;
    }

    public void Skip()
    {
        
    }

    public void Queue(Dialog dialog, int entry)
    {
        this.dialog = dialog;
        this.entry = entry;
        CommandsQueue.Instance.AddCommand(this);
    }
}
