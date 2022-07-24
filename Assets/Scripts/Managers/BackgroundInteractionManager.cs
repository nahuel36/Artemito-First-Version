using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class BackgroundInteractionManager
{
    public static BackgroundInteractionManager Instance => _instance ?? (_instance = new BackgroundInteractionManager());

    private readonly Queue<IBackgroundInteraction> _commandsToExecute;
    private bool _runningCommand;
    private static BackgroundInteractionManager _instance;
    private List<bool> _conditionals;

    private BackgroundInteractionManager()
    {
        _commandsToExecute = new Queue<IBackgroundInteraction>();
        _conditionals = new List<bool>();
        _runningCommand = false;
    }

    public void AddCommand(IBackgroundInteraction commandToEnqueue)
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

    public void ClearAll()
    {
        _conditionals.Clear();
        _commandsToExecute.Clear();
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
                await commandToExecute.ExecuteInBackground();
        }

        ClearConditionals();
        _runningCommand = false;
    }
}
