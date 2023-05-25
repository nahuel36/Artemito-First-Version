using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System;
using UnityEngine.Events;

public class Timer : MonoBehaviour//, IInteraction
{
    float seconds;
    float counter;
    bool started;

    public bool Started { set { started = value; } }

    bool ended;
    [SerializeField] UnityEvent onEnd;

    public void StartTimer()
    {
        StartTimer start = new StartTimer();
        start.Queue(this);
    }

    public void Update()
    {
        if(started)
        {
            if (!ended)
            { 
                counter += Time.deltaTime;
                if(counter > seconds)
                {
                    ended = true;
                    EndTimer end = new EndTimer();
                    end.QueueOnEnd(onEnd);
                }
            }
        }
    }

    public bool ifEnded(Conditional cond)
    {
        BoolConditional onend = new BoolConditional();
        onend.QueueConditional(cond);
        return ended;
    }
    
    public void Configure(float seconds, Action endAction)
    {
        this.seconds = seconds;
        this.onEnd = new UnityEvent();
        this.onEnd.AddListener( () => endAction.Invoke());
    }

    public void WaitForSeconds(float secondsP)
    {
        TimerEfimero efimero = new TimerEfimero();
        efimero.WaitForSeconds(secondsP);
    }

}
