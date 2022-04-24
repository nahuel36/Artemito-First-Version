using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;


public class StartTimer : IInteraction
{
    Timer timer;
    float seconds;

    public void Queue(Timer timer)
    {
        this.timer = timer;
        InteractionManager.Instance.AddCommand(this);
    }


    public async Task Execute()
    {
        timer.Started = true;
        await Task.Yield();
    }
}
