using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class CommandSetGenericProperty : ICommand
{
    GenericProperty property;
    Interaction interaction;

    public async Task Execute()
    {
        await Task.Yield();
        if (InteractionUtils.CheckArePropertyInteraction(InteractionUtils.PropertyObjectType.any, InteractionUtils.PropertyActionType.set_local_property, interaction))
        {
            if (interaction.local_changeBooleanValue)
            {
                property.booleanDefault = false;
                property.boolean = interaction.local_BooleanValue;
            }
            if (interaction.local_changeIntegerValue)
            {
                property.integerDefault = false;
                if (interaction.changeIntegerOrFloatOperation == Interaction.ChangeIntegerOrFloatOperation.set)
                    property.integer = interaction.local_IntegerValue;
                else if (interaction.changeIntegerOrFloatOperation == Interaction.ChangeIntegerOrFloatOperation.add)
                    property.integer += interaction.local_IntegerValue;
                else if (interaction.changeIntegerOrFloatOperation == Interaction.ChangeIntegerOrFloatOperation.subtract)
                    property.integer -= interaction.local_IntegerValue;
            }
            if (interaction.local_changeStringValue)
            {
                property.stringDefault = false;
                if (interaction.changeStringOperation == Interaction.ChangeStringOperation.change)
                    property.String = interaction.local_StringValue;
                else if (interaction.changeStringOperation == Interaction.ChangeStringOperation.replace)
                    property.String = property.String.Replace(interaction.replaceValueToFind, interaction.local_StringValue);
            }
        }
    }

    public void Queue(GenericProperty property, Interaction inter)
    {
        this.property = property;
        this.interaction = inter;
        CommandsQueue.Instance.AddCommand(this);
    }

    public void Skip()
    {
    }

}
