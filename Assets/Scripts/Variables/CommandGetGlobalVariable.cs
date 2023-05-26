using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class CommandGetGlobalVariable : ICommand
{

    InteractuableGlobalVariable variable;
    Interaction interaction;
    Conditional conditional;
    public async Task Execute()
    {
        await Task.Yield();
        if (interaction.variablesAction == Interaction.VariablesAction.getGlobalVariable)
        {
            bool result = true;
            if (interaction.global_compareBooleanValue)
            {
                if (variable.booleanDefault && interaction.global_BooleanValue != interaction.global_defaultBooleanValue)
                    result = false;
                if (!variable.booleanDefault && interaction.global_BooleanValue != variable.boolean)
                    result = false;
            }
            if (interaction.global_compareIntegerValue)
            {
                if (variable.integerDefault && interaction.global_IntegerValue != interaction.global_defaultIntegerValue)
                    result = false;
                if (!variable.integerDefault && interaction.global_IntegerValue != variable.integer)
                    result = false;
            }
            if (interaction.global_compareStringValue)
            {
                if (variable.stringDefault && interaction.global_StringValue != interaction.global_defaultStringValue)
                    result = false;
                if (!variable.stringDefault && interaction.global_StringValue != variable.String)
                    result = false;
            }
            conditional.condition = result;
        }
        if(interaction.global_compareBooleanValue || interaction.global_compareIntegerValue || interaction.global_compareStringValue)
            CommandsQueue.Instance.AddConditional(conditional);
    }

    public void Queue(InteractuableGlobalVariable variable, Interaction inter)
    {
        this.variable = variable;
        this.interaction = inter;
        
        conditional = new Conditional();
        conditional.lineToGoIfFalse = interaction.LineToGoOnFalseResult;
        conditional.lineToGoIfTrue = interaction.LineToGoOnTrueResult;
        conditional.actionIfFalse = interaction.OnCompareResultFalseAction;
        conditional.actionIfTrue = interaction.OnCompareResultTrueAction;

        CommandsQueue.Instance.AddCommand(this);
    }

    public void Skip()
    {        
    }


}
