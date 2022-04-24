using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class InteractionManager  
{
    public static InteractionManager Instance => _instance ?? (_instance = new InteractionManager());

    private readonly Queue<IInteraction> _commandsToExecute;
    private bool _runningCommand;
    private static InteractionManager _instance;
    private List<bool> _conditionals;

    private InteractionManager()
    {
        _commandsToExecute = new Queue<IInteraction>();
        _conditionals = new List<bool>();
        _runningCommand = false;
    }

    public void AddCommand(IInteraction commandToEnqueue)
    {
        _commandsToExecute.Enqueue(commandToEnqueue);
        RunNextCommand();
    }

    public bool Executing()
    {
        return _runningCommand;
    }

    public void AddConditional(bool cond)
    {
        _conditionals.Add(cond);
    }

    public void ClearConditionals()
    {
        _conditionals.Clear();
    }

    public void DebugCount()
    {
        Debug.Log(_conditionals.Count);
    }

    private async Task RunNextCommand()
    {
        if (_runningCommand)
        {
            return;
        }

        while (_commandsToExecute.Count > 0)
        {
            _runningCommand = true;
            var commandToExecute = _commandsToExecute.Dequeue();
            bool execute = true;

            if (commandToExecute is EndTimer)
                ClearConditionals();
            

            for (int i = 0; i < _conditionals.Count; i++)
            {
                if(_conditionals[i] == false)
                {
                    execute = false;
                }
            }

            if (execute)
                await commandToExecute.Execute();
        }

        ClearConditionals();
        _runningCommand = false;
    }
}
