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
            bool result = true;
            if (interaction.local_compareBooleanValue)
            {
                if (variable.booleanDefault && interaction.local_BooleanValue != interaction.local_defaultBooleanValue)
                    result = false;
                if (!variable.booleanDefault && interaction.local_BooleanValue != variable.boolean)
                    result = false;
            }
            if (interaction.local_compareIntegerValue)
            {
                if (variable.integerDefault && interaction.local_IntegerValue != interaction.local_defaultIntegerValue)
                    result = false;
                if (!variable.integerDefault && interaction.local_IntegerValue != variable.integer)
                    result = false;
            }
            if (interaction.local_compareStringValue)
            {
                if (variable.stringDefault && interaction.local_StringValue != interaction.local_defaultStringValue)
                    result = false;
                if (!variable.stringDefault && interaction.local_StringValue != variable.String)
                    result = false;
            }
            conditional.condition = result;
        }
        if(interaction.local_compareBooleanValue || interaction.local_compareIntegerValue || interaction.local_compareStringValue)
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
