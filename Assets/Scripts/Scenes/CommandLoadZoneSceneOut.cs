using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class CommandLoadZoneSceneOut : ICommand
{
    string scenePath;
    string entryPoint;
    public async Task Execute()
    {
        await Task.Yield();
        await MultipleScenesManager.Instance.LoadSceneOut(scenePath, entryPoint);
    }

    public void Skip()
    {
    }

    // Start is called before the first frame update
    public void Queue(string scenePath, string entryPoint)
    {
        this.scenePath = scenePath;
        this.entryPoint = entryPoint;
        CommandsQueue.Instance.AddCommand(this);
    }


}
