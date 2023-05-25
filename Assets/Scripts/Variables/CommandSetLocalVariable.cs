using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class CommandSetLocalVariable : ICommand
{
    InteractuableLocalVariable variable;
    Interaction interaction;

    public async Task Execute()
    {
        await Task.Yield();
        if (interaction.variablesAction == Interaction.VariablesAction.setLocalVariable)
        {
            if (interaction.local_changeBooleanValue)
            {
                variable.booleanDefault = false;
                variable.boolean = interaction.local_BooleanValue;
            }
            if (interaction.local_changeIntegerValue)
            {
                variable.integerDefault = false;
                variable.integer = interaction.local_IntegerValue;
            }
            if (interaction.local_changeStringValue)
            {
                variable.stringDefault = false;
                variable.String = interaction.local_StringValue;
            }
        }
    }

    public void Queue(InteractuableLocalVariable variable,Interaction inter)
    {
        this.variable = variable;
        this.interaction = inter;
        CommandsQueue.Instance.AddCommand(this);
    }

    public void Skip()
    {
    }

}
