using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;


public abstract class CustomScript : MonoBehaviour, ICommand
{
    public async Task Execute()
    {
        await Task.Yield();
    }

    public virtual void LoadScript() {
        CommandsQueue.Instance.AddCommand(this);
    }

    public void Skip()
    {

    }
}
