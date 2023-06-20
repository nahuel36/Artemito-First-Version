using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class CommandUseAsInventory : ICommand
{
    public async Task Execute()
    {
        await Task.Yield();

        await UI_PNC_Manager.Instance.SetInteractuableAsInventory();
    }

    public void Skip()
    {
        
    }

    public void Queue() { 
        CommandsQueue.Instance.AddCommand(this);
    }
   
}
