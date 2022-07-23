using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System;

public class TimerEfimero : IInteraction
{
    float seconds;

    // Start is called before the first frame update
    public async Task Execute()
    {
        await Task.Delay(TimeSpan.FromSeconds(seconds));
    }

    // Update is called once per frame
    public void WaitForSeconds(float secondsP)
    {
        seconds = secondsP;
        InteractionManager.Instance.AddCommand(this);
    }

    public void ConfigureWithoutQueue(float secondsP)
    {
        seconds = secondsP;
    }
}
