using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;


public class StartTimer : ICommand
{
    Timer timer;
    float seconds;

    public void Queue(Timer timer)
    {
        this.timer = timer;
        CommandsQueue.Instance.AddCommand(this);
    }


    public async Task Execute()
    {
        timer.Started = true;
        await Task.Yield();
    }

    public void Skip()
    {

    }
}
