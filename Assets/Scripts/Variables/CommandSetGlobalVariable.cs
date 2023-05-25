using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class CommandSetGlobalVariable : ICommand
{

    InteractuableGlobalVariable variable;
    Interaction interaction;
    public async Task Execute()
    {
        await Task.Yield();
        if (interaction.variablesAction == Interaction.VariablesAction.setGlobalVariable)
        {
            if (interaction.global_changeBooleanValue)
            {
                variable.booleanDefault = false;
                variable.boolean = interaction.global_BooleanValue;
            
            }
            if (interaction.global_changeIntegerValue)
            {
                variable.integerDefault = false;
                variable.integer = interaction.global_IntegerValue;
            }
            if (interaction.global_changeStringValue)
            {
                variable.stringDefault = false;
                variable.String = interaction.global_StringValue;
            }
        }
    }

    public void Queue(InteractuableGlobalVariable variable, Interaction inter)
    {
        this.variable = variable;
        this.interaction = inter;
        CommandsQueue.Instance.AddCommand(this);
    }

    public void Skip()
    {        
    }


}
