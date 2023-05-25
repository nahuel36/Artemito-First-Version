using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class Conditional {
    public enum GetVariableAction
    {
        Stop,
        Continue,
        GoToSpecificLine
    }
    public GetVariableAction actionIfTrue;
    public GetVariableAction actionIfFalse;
    public int lineToGoIfTrue;
    public int lineToGoIfFalse;
    public bool condition;
}

public class CommandsQueue
{
    public static CommandsQueue Instance => _instance ?? (_instance = new CommandsQueue());
    public static CommandsQueue BackgroundInstance => _bginstance ?? (_bginstance = new CommandsQueue());

    private readonly List<ICommand> _commandsToExecute;
    private bool _runningCommand;
    private static CommandsQueue _instance;
    private static CommandsQueue _bginstance;
    private Conditional _conditional;
    private ICommand actualCommand;
    private int actualCommandIndex = 0;
    private bool execute = true;
    private CommandsQueue()
    {
        _commandsToExecute = new List<ICommand>();
        _runningCommand = false;
        actualCommandIndex = 0;
    }

    public void AddCommand(ICommand commandToEnqueue)
    {
        _commandsToExecute.Add(commandToEnqueue);
        RunNextCommand();
    }

    public bool Executing()
    {
        return _runningCommand;
    }

    public void AddConditional(Conditional cond)
    {
        _conditional = cond;

         if (_conditional != null && _conditional.condition == true)
            {
                if (_conditional.actionIfTrue == Conditional.GetVariableAction.Stop)
                {
                    execute = false;
                }
                if (_conditional.actionIfTrue == Conditional.GetVariableAction.Continue)
                {
                    execute = true;
                }
                if (_conditional.actionIfTrue == Conditional.GetVariableAction.GoToSpecificLine)
                {
                    actualCommandIndex = _conditional.lineToGoIfTrue;
                    execute = true;
                }
            }
            if (_conditional != null && _conditional.condition == false)
            {
                if (_conditional.actionIfFalse == Conditional.GetVariableAction.Stop)
                {
                    execute = false;
                }
                if (_conditional.actionIfFalse == Conditional.GetVariableAction.Continue)
                {
                    execute = true;
                }
                if (_conditional.actionIfFalse == Conditional.GetVariableAction.GoToSpecificLine)
                {
                    actualCommandIndex = _conditional.lineToGoIfFalse;
                    execute = true;
                }
            }

    }

    public void ClearConditionals()
    {
        _conditional = null;
    }

    public void SkipActualCommand()
    {
        if (actualCommand != null)
            actualCommand.Skip();
    }

    public void ClearAll()
    {
        _conditional = null;
        _commandsToExecute.Clear();
        SkipActualCommand();
    }


    private async Task RunNextCommand()
    {
        if (_runningCommand)
        {
            return;
        }

        while (actualCommandIndex < _commandsToExecute.Count)
        {
            _runningCommand = true;
            var commandToExecute = _commandsToExecute[actualCommandIndex];
            actualCommandIndex++;

            if (commandToExecute is EndTimer)
                execute = true;

            if (execute)
            {
                actualCommand = commandToExecute;
                await commandToExecute.Execute();
            }
        }

        execute = true;
        _commandsToExecute.Clear();
        actualCommandIndex = 0;
        _runningCommand = false;
    }
}



