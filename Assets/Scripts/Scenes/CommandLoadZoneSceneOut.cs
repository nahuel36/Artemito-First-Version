using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class CommandLoadZoneSceneOut : ICommand
{
    public async Task Execute()
    {
        await Task.Yield();
        await MultipleScenesManager.Instance.LoadSceneOut();
    }

    public void Skip()
    {
    }

    // Start is called before the first frame update
    public void Queue()
    {
        CommandsQueue.Instance.AddCommand(this);
    }


}
