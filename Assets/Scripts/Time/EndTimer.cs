using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.Events;

public class EndTimer : ICommand
{
    UnityEvent onEnd;
    // Start is called before the first frame update
    public async Task Execute()
    {
        onEnd.Invoke();
        await Task.Yield();
    }

    public void QueueOnEnd(UnityEvent action)
    {
        this.onEnd = action;
        CommandsQueue.Instance.AddCommand(this);
    }

    public void Skip()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
