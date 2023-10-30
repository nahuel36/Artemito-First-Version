using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Threading.Tasks;
public static class InteractionUtils 
{
    static UnhandledEvents unhandledEvents;
    static InventoryList inventory;
    public static VerbInteractions FindVerb(Verb verb, List<VerbInteractions> verbs)
    {
        for (int i = 0; i < verbs.Count; i++)
        {
            if (verbs[i].verb == verb)
                return verbs[i];
        }
        return null;
    }

    public async static Task RunAttempsInteraction(AttempsContainer attempsContainer, InteractionObjectsType interactionType, Verb verb, InventoryItem[] items, PNCInteractuable[] sceneInteractuables = null, bool runInBackground = false)
    {
        bool runUnhandledEvents = false;
        if (attempsContainer.attemps.Count == 0)
            runUnhandledEvents = true;
        else 
        {
            int index;

            if (attempsContainer.attempsIteration == AttempsContainer.Iteration.inOrderAndRepeatLastOne)
                index = Mathf.Clamp(attempsContainer.executedTimes, 0, attempsContainer.attemps.Count - 1);
            else if(attempsContainer.attempsIteration == AttempsContainer.Iteration.inOrderAndRestartAgain)
                index = attempsContainer.executedTimes % attempsContainer.attemps.Count;
            else 
                index = Random.Range(0,attempsContainer.attemps.Count);

            if (attempsContainer.attemps[index].interactions.Count == 0)
            { 
                runUnhandledEvents = true;
            }
            else 
            {
                int i = 0;
                while (i < attempsContainer.attemps[index].interactions.Count)
                {
                    InitializeInteractionCommand command = new InitializeInteractionCommand();
                    command.Queue(attempsContainer.attemps[index].interactions[i], runInBackground);

                    while (command.action == null)
                    {
                        await Task.Yield();
                    }

                    CustomArgument argument;
                    if (attempsContainer.attemps[index].interactions[i].customActionArguments.Count <= 0)
                    {
                        attempsContainer.attemps[index].interactions[i].customActionArguments = new List<CustomArgument>();
                        argument = new CustomArgument();
                        attempsContainer.attemps[index].interactions[i].customActionArguments.Add(argument);
                    }
                    else
                    {
                        argument = attempsContainer.attemps[index].interactions[i].customActionArguments[0];
                    }
                    argument.interactionType = interactionType;
                    argument.verb = verb;
                    if(items != null && items.Length > 0)
                        argument.itemIndex = items[0].specialIndex;
                    if (sceneInteractuables != null && sceneInteractuables.Length > 0)
                        argument.interactuableID = sceneInteractuables[0].GetInstanceID();

                    attempsContainer.attemps[index].interactions[i].customActionArguments[0] = argument;

                    if (interactionType == InteractionObjectsType.inventoryIninventory)
                    {
                        CustomArgument argument2 = new CustomArgument();
                        argument2.interactionType = interactionType;
                        argument2.verb = verb;
                        argument2.itemIndex = items[1].specialIndex;

                        if (attempsContainer.attemps[index].interactions[i].customActionArguments.Count <= 1)
                        {
                            attempsContainer.attemps[index].interactions[i].customActionArguments.Add(argument2);
                        }
                        else
                        {
                            attempsContainer.attemps[index].interactions[i].customActionArguments[1] = argument2;
                        }
                    }
                    else if (interactionType == InteractionObjectsType.objectInObject)
                    {
                        CustomArgument argument2 = new CustomArgument();
                        argument2.interactionType = interactionType;
                        argument2.verb = verb;
                        argument2.interactuableID = sceneInteractuables[1].GetInstanceID();

                        if (attempsContainer.attemps[index].interactions[i].customActionArguments.Count <= 1)
                        {
                            attempsContainer.attemps[index].interactions[i].customActionArguments.Add(argument2);
                        }
                        else
                        {
                            attempsContainer.attemps[index].interactions[i].customActionArguments[1] = argument2;
                        }
                    }


                    command.action.Invoke(attempsContainer.attemps[index].interactions[i].customActionArguments);

                    i++;

                    if (i < attempsContainer.attemps[index].interactions.Count)
                    {
                        i = CheckConditionals(i, attempsContainer.attemps[index].interactions[i - 1]);
                        if (i == -1)
                        {
                            break;
                        }
                    }
                }
            }
        }

        if (runUnhandledEvents && interactionType != InteractionObjectsType.unhandledEvent)
        {
            RunHunhandledEvents(interactionType, verb, items, sceneInteractuables, runInBackground);
        }
        
        attempsContainer.executedTimes++;
    }

    public static void RunHunhandledEvents(InteractionObjectsType interactionType, Verb verb, InventoryItem[] item, PNCInteractuable[] sceneInteractuable = null, bool runInBackground = false)
    {
        if (!unhandledEvents)
        {
            unhandledEvents = Resources.Load<UnhandledEvents>("UnhandledEvents");
        }
        if (!inventory)
        {
            inventory = Resources.Load<InventoryList>("Inventory");
        }
        if (interactionType == InteractionObjectsType.verbInObject || interactionType == InteractionObjectsType.verbInInventory)
        {
            for (int i = 0; i < unhandledEvents.verbs.Count; i++)
            {
                if (verb.index == unhandledEvents.verbs[i].verb.index)
                {
                    RunAttempsInteraction(unhandledEvents.verbs[i].attempsContainer, InteractionObjectsType.unhandledEvent, verb, item, sceneInteractuable);
                }
            }
        }
        else if (interactionType == InteractionObjectsType.inventoryInObject)
        {
            for (int i = 0; i < unhandledEvents.inventoryActions.Count; i++)
            {
                if (item[0].specialIndex == unhandledEvents.inventoryActions[i].specialIndex && verb.index == unhandledEvents.inventoryActions[i].verb.index)
                {
                    RunAttempsInteraction(unhandledEvents.inventoryActions[i].attempsContainer, InteractionObjectsType.unhandledEvent, verb, item, sceneInteractuable);
                }
            }
        }
        else if (interactionType == InteractionObjectsType.inventoryIninventory)
        {
            for (int i = 0; i < unhandledEvents.inventoryActions.Count; i++)
            {
                if (item[0].specialIndex == unhandledEvents.inventoryActions[i].specialIndex && verb.index == unhandledEvents.inventoryActions[i].verb.index)
                {
                    RunAttempsInteraction(unhandledEvents.inventoryActions[i].attempsContainer, InteractionObjectsType.unhandledEvent, verb, item);
                }
            }
        }
        else if (interactionType == InteractionObjectsType.objectInObject)
        { 
            for (int i = 0; i < unhandledEvents.inventoryActions.Count; i++)
            {
                if (unhandledEvents.inventoryActions[i].sceneObject == sceneInteractuable[0] && verb.index == unhandledEvents.inventoryActions[i].verb.index)
                {
                    RunAttempsInteraction(unhandledEvents.inventoryActions[i].attempsContainer, InteractionObjectsType.unhandledEvent, verb, item, sceneInteractuable);
                }
            }
        }
    }

    private static int CheckConditionals(int actualindex, Interaction interaction)
    {
        if ((interaction.type == Interaction.InteractionType.custom && interaction.customScriptAction == Interaction.CustomScriptAction.customBoolean) ||
            (interaction.type == Interaction.InteractionType.properties_container && (interaction.propertiesAction == Interaction.PropertiesContainerAction.getGlobalProperty || interaction.propertiesAction == Interaction.PropertiesContainerAction.getLocalProperty)))
        {
            bool result = true;
            
            if (interaction.type == Interaction.InteractionType.properties_container && interaction.propertiesAction == Interaction.PropertiesContainerAction.getGlobalProperty)
            {
                var property = interaction.propertyObject.global_properties[interaction.globalPropertySelected];

                if (interaction.global_compareBooleanValue)
                {
                    if (property.booleanDefault && interaction.global_BooleanValue != interaction.global_defaultBooleanValue)
                        result = false;
                    if (!property.booleanDefault && interaction.global_BooleanValue != property.boolean)
                        result = false;
                }
                if (interaction.global_compareIntegerValue)
                {
                    if (property.integerDefault && interaction.global_IntegerValue != interaction.global_defaultIntegerValue)
                        result = false;
                    if (!property.integerDefault && interaction.global_IntegerValue != property.integer)
                        result = false;
                }
                if (interaction.global_compareStringValue)
                {
                    if (property.stringDefault && interaction.global_StringValue != interaction.global_defaultStringValue)
                        result = false;
                    if (!property.stringDefault && interaction.global_StringValue != property.String)
                        result = false;
                }
            }
            else if (interaction.type == Interaction.InteractionType.properties_container && interaction.propertiesAction == Interaction.PropertiesContainerAction.getLocalProperty)
            {
                var property = interaction.propertyObject.local_properties[interaction.localPropertySelected]; 

                if (interaction.local_compareBooleanValue)
                {
                    if (property.booleanDefault && interaction.local_BooleanValue != interaction.local_defaultBooleanValue)
                        result = false;
                    if (!property.booleanDefault && interaction.local_BooleanValue != property.boolean)
                        result = false;
                }
                if (interaction.local_compareIntegerValue)
                {
                    if (property.integerDefault && interaction.local_IntegerValue != interaction.local_defaultIntegerValue)
                        result = false;
                    if (!property.integerDefault && interaction.local_IntegerValue != property.integer)
                        result = false;
                }
                if (interaction.local_compareStringValue)
                {
                    if (property.stringDefault && interaction.local_StringValue != interaction.local_defaultStringValue)
                        result = false;
                    if (!property.stringDefault && interaction.local_StringValue != property.String)
                        result = false;
                }
            }
            else if (interaction.type == Interaction.InteractionType.custom && interaction.customScriptAction == Interaction.CustomScriptAction.customBoolean)
                result = interaction.customActionArguments[0].resultBool;

            if (result == true)
            {
                if (interaction.OnCompareResultTrueAction == Conditional.GetPropertyAction.Stop)
                    return -1;
                else if (interaction.OnCompareResultTrueAction == Conditional.GetPropertyAction.Continue)
                    return actualindex;
                else
                    return interaction.LineToGoOnTrueResult;
            }
            else
            {
                if (interaction.OnCompareResultFalseAction == Conditional.GetPropertyAction.Stop)
                    return -1;
                else if (interaction.OnCompareResultFalseAction == Conditional.GetPropertyAction.Continue)
                    return actualindex;
                else
                    return interaction.LineToGoOnFalseResult;
            }
        }
        return actualindex;
    }

    public static UnityEvent<List<CustomArgument>> InitializeInteraction(Interaction interaction)
    {
        UnityEvent<List<CustomArgument>> action = new UnityEvent<List<CustomArgument>>();
        if (interaction.type == Interaction.InteractionType.custom)
            action = interaction.action;
        else if (interaction.type == Interaction.InteractionType.character)
        {
            PNCCharacter charact = interaction.character;
            if (interaction.characterAction == Interaction.CharacterAction.say)
            {
                string whattosay = interaction.WhatToSay;
                if (interaction.CanSkip)
                    action.AddListener((arguments) => charact.Talk(whattosay));
                else
                    action.AddListener((arguments) => charact.UnskippableTalk(whattosay));
            }
            else if (interaction.characterAction == Interaction.CharacterAction.sayWithScript)
            {
                if (interaction.CanSkip)
                    action.AddListener((arguments) => charact.Talk(((SayScript)interaction.SayScript).SayWithScript(arguments)));
                else
                    action.AddListener((arguments) => charact.UnskippableTalk(((SayScript)interaction.SayScript).SayWithScript(arguments)));
            }
            else if (interaction.characterAction == Interaction.CharacterAction.walk)
            {
                action.AddListener((arguments) => charact.Walk(interaction.WhereToWalk.position));
            }
            else if (interaction.characterAction == Interaction.CharacterAction.walkStraight)
            {
                action.AddListener((arguments) => charact.WalkStraight(interaction.WhereToWalk.position));
            }
        }
        else if (interaction.type == Interaction.InteractionType.dialog)
        {
            if (interaction.dialogAction == Interaction.DialogAction.startDialog)
            {
                action.AddListener((arguments) => DialogsManager.Instance.StartDialog(interaction.dialogSelected, interaction.dialogSelected.current_entryDialogIndex));
            }
            else if (interaction.dialogAction == Interaction.DialogAction.changeEntry)
            {
                action.AddListener((arguments) => DialogsManager.Instance.ChangeEntry(interaction.dialogSelected, interaction.newDialogEntry));
            }
            else if (interaction.dialogAction == Interaction.DialogAction.changeOptionState)
            {
                action.AddListener((arguments) => DialogsManager.Instance.ChangeOptionState(interaction.dialogSelected, interaction.subDialogIndex, interaction.optionIndex, interaction.newOptionState));
            }
            else if (interaction.dialogAction == Interaction.DialogAction.changeOptionText)
            {
                action.AddListener((arguments) => DialogsManager.Instance.ChangeOptionText(interaction.dialogSelected, interaction.subDialogIndex, interaction.optionIndex, interaction.newOptionText));
            }
        }
        else if (interaction.type == Interaction.InteractionType.properties_container)
        {
            PNCPropertiesContainer varContainer = interaction.propertyObject;
            if (interaction.propertiesAction == Interaction.PropertiesContainerAction.setLocalProperty)
            {
                action.AddListener((arguments) =>
                varContainer.SetLocalProperty(interaction,
                                                interaction.propertyObject.local_properties[interaction.localPropertySelected]));
            }
            else if (interaction.propertiesAction == Interaction.PropertiesContainerAction.setGlobalProperty)
            {
                action.AddListener((arguments) =>
                varContainer.SetGlobalProperty(interaction,
                                                interaction.propertyObject.global_properties[interaction.globalPropertySelected]));
            }
        }
        else if (interaction.type == Interaction.InteractionType.inventory)
        {
            if (interaction.inventoryAction == Interaction.InventoryAction.useAsInventory)
            {
                action.AddListener((arguments) =>
                {
                    CommandUseAsInventory command = new CommandUseAsInventory();
                    command.Queue();
                });
            }
        }
        return action;
    }
}

