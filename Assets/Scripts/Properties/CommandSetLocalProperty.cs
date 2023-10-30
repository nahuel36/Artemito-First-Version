using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class CommandSetLocalProperty : ICommand
{
    LocalProperty property;
    Interaction interaction;

    public async Task Execute()
    {
        await Task.Yield();
        if (interaction.propertiesAction == Interaction.PropertiesContainerAction.setLocalProperty)
        {
            if (interaction.local_changeBooleanValue)
            {
                property.booleanDefault = false;
                property.boolean = interaction.local_BooleanValue;
            }
            if (interaction.local_changeIntegerValue)
            {
                property.integerDefault = false;
                property.integer = interaction.local_IntegerValue;
            }
            if (interaction.local_changeStringValue)
            {
                property.stringDefault = false;
                property.String = interaction.local_StringValue;
            }
        }
    }

    public void Queue(LocalProperty property,Interaction inter)
    {
        this.property = property;
        this.interaction = inter;
        CommandsQueue.Instance.AddCommand(this);
    }

    public void Skip()
    {
    }

}
