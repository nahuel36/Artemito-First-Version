using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class CommandSetGlobalProperty : ICommand
{

    GlobalProperty property;
    Interaction interaction;
    public async Task Execute()
    {
        await Task.Yield();
        if (interaction.propertiesAction == Interaction.PropertiesAction.setGlobalProperty)
        {
            if (interaction.global_changeBooleanValue)
            {
                property.booleanDefault = false;
                property.boolean = interaction.global_BooleanValue;
            
            }
            if (interaction.global_changeIntegerValue)
            {
                property.integerDefault = false;
                property.integer = interaction.global_IntegerValue;
            }
            if (interaction.global_changeStringValue)
            {
                property.stringDefault = false;
                property.String = interaction.global_StringValue;
            }
        }
    }

    public void Queue(GlobalProperty property, Interaction inter)
    {
        this.property = property;
        this.interaction = inter;
        CommandsQueue.Instance.AddCommand(this);
    }

    public void Skip()
    {        
    }


}
