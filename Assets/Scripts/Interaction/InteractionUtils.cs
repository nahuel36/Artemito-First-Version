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

    public async static Task RunAttempsInteraction(AttempsContainer attempsContainer, InteractionObjectsType interactionType, string prefixNameAndPostfix, int verbIndex, int itemIndex)
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
                    command.Queue(attempsContainer.attemps[index].interactions[i]);

                    while (command.action == null)
                    {
                        await Task.Yield();
                    }

                    if (attempsContainer.attemps[index].interactions[i].type == Interaction.InteractionType.custom)
                    {
                        attempsContainer.attemps[index].interactions[i].customActionObject.interaction = attempsContainer.attemps[index].interactions[i];
                    }

                    command.action.Invoke();

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
            RunHunhandledEvents(interactionType, prefixNameAndPostfix, verbIndex, itemIndex);
        }
        
        attempsContainer.executedTimes++;
    }

    public static void RunHunhandledEvents(InteractionObjectsType interactionType, string prefixNameAndPostfix, int verbIndex, int itemIndex)
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
                if (verbIndex == unhandledEvents.verbs[i].verb.index)
                {
                    RunAttempsInteraction(unhandledEvents.verbs[i].attempsContainer, InteractionObjectsType.unhandledEvent, prefixNameAndPostfix, verbIndex, itemIndex);
                }
            }
        }
        else if (interactionType == InteractionObjectsType.inventoryInObject)
        {
            for (int i = 0; i < unhandledEvents.inventoryActions.Count; i++)
            {
                if (itemIndex == unhandledEvents.inventoryActions[i].specialIndex && verbIndex == unhandledEvents.inventoryActions[i].verb.index)
                {
                    RunAttempsInteraction(unhandledEvents.inventoryActions[i].attempsContainer, InteractionObjectsType.unhandledEvent, prefixNameAndPostfix, verbIndex, itemIndex);
                }
            }
        }
    }

    private static int CheckConditionals(int actualindex, Interaction interaction)
    {
        if ((interaction.type == Interaction.InteractionType.custom && interaction.customScriptAction == Interaction.CustomScriptAction.customBoolean) ||
            interaction.type == Interaction.InteractionType.variables && (interaction.variablesAction == Interaction.VariablesAction.getGlobalVariable || interaction.variablesAction == Interaction.VariablesAction.getLocalVariable))
        {
            bool result = true;
            var variable = interaction.variableObject.global_variables[interaction.globalVariableSelected];

            if (interaction.type == Interaction.InteractionType.variables && interaction.variablesAction == Interaction.VariablesAction.getGlobalVariable)
            { 
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
            }
            else if (interaction.type == Interaction.InteractionType.variables && interaction.variablesAction == Interaction.VariablesAction.getLocalVariable)
            {
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
            }
            else if (interaction.type == Interaction.InteractionType.custom && interaction.customScriptAction == Interaction.CustomScriptAction.customBoolean)
                result = interaction.customScriptBool;

            if (result == true)
            {
                if (interaction.OnCompareResultTrueAction == Conditional.GetVariableAction.Stop)
                    return -1;
                else if (interaction.OnCompareResultTrueAction == Conditional.GetVariableAction.Continue)
                    return actualindex;
                else
                    return interaction.LineToGoOnTrueResult;
            }
            else
            {
                if (interaction.OnCompareResultFalseAction == Conditional.GetVariableAction.Stop)
                    return -1;
                else if (interaction.OnCompareResultFalseAction == Conditional.GetVariableAction.Continue)
                    return actualindex;
                else
                    return interaction.LineToGoOnFalseResult;
            }
        }
        return actualindex;
    }

    public static UnityEvent InitializeInteraction(Interaction interaction)
    {
        UnityEvent action = new UnityEvent();
        if (interaction.type == Interaction.InteractionType.custom)
            action = interaction.action;
        else if (interaction.type == Interaction.InteractionType.character)
        {
            PNCCharacter charact = interaction.character;
            if (interaction.characterAction == Interaction.CharacterAction.say)
            {
                string whattosay = interaction.WhatToSay;
                if (interaction.CanSkip)
                    action.AddListener(() => charact.Talk(whattosay));
                else
                    action.AddListener(() => charact.UnskippableTalk(whattosay));
            }
            else if (interaction.characterAction == Interaction.CharacterAction.sayWithScript)
            {
                if (interaction.CanSkip)
                    action.AddListener(() => charact.Talk(((SayScript)interaction.SayScript).SayWithScript()));
                else
                    action.AddListener(() => charact.UnskippableTalk(((SayScript)interaction.SayScript).SayWithScript()));
            }
            else if (interaction.characterAction == Interaction.CharacterAction.walk)
            {
                action.AddListener(() => charact.Walk(interaction.WhereToWalk.position));
            }
        }
        else if (interaction.type == Interaction.InteractionType.dialog)
        {
            if (interaction.dialogAction == Interaction.DialogAction.startDialog)
            {
                action.AddListener(() => DialogsManager.Instance.StartDialog(interaction.dialogSelected, interaction.dialogSelected.current_entryDialogIndex));
            }
            else if (interaction.dialogAction == Interaction.DialogAction.changeEntry)
            {
                action.AddListener(() => DialogsManager.Instance.ChangeEntry(interaction.dialogSelected, interaction.newDialogEntry));
            }
            else if (interaction.dialogAction == Interaction.DialogAction.changeOptionState)
            {
                action.AddListener(() => DialogsManager.Instance.ChangeOptionState(interaction.dialogSelected, interaction.subDialogIndex, interaction.optionIndex, interaction.newOptionState));
            }
            else if (interaction.dialogAction == Interaction.DialogAction.changeOptionText)
            {
                action.AddListener(() => DialogsManager.Instance.ChangeOptionText(interaction.dialogSelected, interaction.subDialogIndex, interaction.optionIndex, interaction.newOptionText));
            }
        }
        else if (interaction.type == Interaction.InteractionType.variables)
        {
            PNCVariablesContainer varContainer = interaction.variableObject;
            if (interaction.variablesAction == Interaction.VariablesAction.setLocalVariable)
            {
                action.AddListener(() =>
                varContainer.SetLocalVariable(interaction,
                                                interaction.variableObject.local_variables[interaction.localVariableSelected]));
            }
            else if (interaction.variablesAction == Interaction.VariablesAction.setGlobalVariable)
            {
                action.AddListener(() =>
                varContainer.SetGlobalVariable(interaction,
                                                interaction.variableObject.global_variables[interaction.globalVariableSelected]));
            }
        }
        else if (interaction.type == Interaction.InteractionType.inventory)
        {
            if (interaction.inventoryAction == Interaction.InventoryAction.useAsInventory)
            {
                action.AddListener(() =>
                {
                    CommandUseAsInventory command = new CommandUseAsInventory();
                    command.Queue();
                });
            }
        }
        return action;
    }
}

