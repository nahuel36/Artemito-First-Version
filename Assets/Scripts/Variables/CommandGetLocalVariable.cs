using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class CommandGetLocalVariable : ICommand
{
    InteractuableLocalVariable variable;
    Interaction interaction;
    Conditional conditional;
    public async Task Execute()
    {
        await Task.Yield();
        if (interaction.variablesAction == Interaction.VariablesAction.getLocalVariable)
        {
            bool result = false;
            if (interaction.local_compareBooleanValue)
            {
                if (variable.booleanDefault && interaction.local_BooleanValue == interaction.local_defaultBooleanValue)
                    result = true;
                if (!variable.booleanDefault && interaction.local_BooleanValue == variable.boolean)
                    result = true;
            }
            if (interaction.local_compareIntegerValue)
            {
                if (variable.integerDefault && interaction.local_IntegerValue == interaction.local_defaultIntegerValue)
                    result = true;
                if (!variable.integerDefault && interaction.local_IntegerValue == variable.integer)
                    result = true;
            }
            if (interaction.local_compareStringValue)
            {
                if (variable.stringDefault && interaction.local_StringValue == interaction.local_defaultStringValue)
                    result = true;
                if (!variable.stringDefault && interaction.local_StringValue == variable.String)
                    result = true;
            }
            conditional.condition = result;
        }
        CommandsQueue.Instance.AddConditional(conditional);

    }

    public void Queue(InteractuableLocalVariable variable,Interaction inter)
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
