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
            bool result = false;
            if (interaction.global_compareBooleanValue)
            {
                if (variable.booleanDefault && interaction.global_BooleanValue == interaction.global_defaultBooleanValue)
                    result = true;
                if (!variable.booleanDefault && interaction.global_BooleanValue == variable.boolean)
                    result = true;
            }
            if (interaction.global_compareIntegerValue)
            {
                if (variable.integerDefault && interaction.global_IntegerValue == interaction.global_defaultIntegerValue)
                    result = true;
                if (!variable.integerDefault && interaction.global_IntegerValue == variable.integer)
                    result = true;
            }
            if (interaction.global_compareStringValue)
            {
                if (variable.stringDefault && interaction.global_StringValue == interaction.global_defaultStringValue)
                    result = true;
                if (!variable.stringDefault && interaction.global_StringValue == variable.String)
                    result = true;
            }
            conditional.condition = result;
        }
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
