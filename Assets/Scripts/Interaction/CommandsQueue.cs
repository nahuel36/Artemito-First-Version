using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class CommandsQueue  
{
    public static CommandsQueue Instance => _instance ?? (_instance = new CommandsQueue());
    public static CommandsQueue BackgroundInstance => _bginstance ?? (_bginstance = new CommandsQueue());

    private readonly Queue<ICommand> _commandsToExecute;
    private bool _runningCommand;
    private static CommandsQueue _instance;
    private static CommandsQueue _bginstance;
    private List<bool> _conditionals;
    private ICommand actualCommand;

    private CommandsQueue()
    {
        _commandsToExecute = new Queue<ICommand>();
        _conditionals = new List<bool>();
        _runningCommand = false;
    }

    public void AddCommand(ICommand commandToEnqueue)
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
        SkipActualCommand();
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
            {
                actualCommand = commandToExecute;
                await commandToExecute.Execute();
            }
        }

        ClearConditionals();
        _runningCommand = false;
    }

    public void SkipActualCommand()
    {
        if (actualCommand != null)
            actualCommand.Skip();
    }
}
