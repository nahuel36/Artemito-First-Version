using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.Events;

public class ConditionalCommand : ICommand
{
    Conditional conditional;

    // Start is called before the first frame update
    public async Task Execute()
    {
        CommandsQueue.Instance.AddConditional(conditional);
        await Task.Yield();
    }

    public void QueueConditional(Conditional conditional)
    {
        this.conditional = conditional;
        CommandsQueue.Instance.AddCommand(this);
    }

    public void Skip()
    {

    }
}
