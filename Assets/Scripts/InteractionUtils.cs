using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Threading.Tasks;
public static class InteractionUtils 
{
    public static VerbInteractions FindVerb(Verb verb, List<VerbInteractions> verbs)
    {
        for (int i = 0; i < verbs.Count; i++)
        {
            if (verbs[i].verb == verb)
                return verbs[i];
        }
        return null;
    }

    public async static void RunAttempsInteraction(AttempsContainer attempsContainer)
    {
        if(attempsContainer.attemps.Count > 0)
        {
            int index;

            if (!attempsContainer.isRandom && !attempsContainer.isCyclical)
                index = Mathf.Clamp(attempsContainer.executedTimes, 0, attempsContainer.attemps.Count - 1);
            else if(!attempsContainer.isCyclical && !attempsContainer.isRandom)
                index = attempsContainer.executedTimes % attempsContainer.attemps.Count;
            else 
                index = Random.Range(0,attempsContainer.attemps.Count);

            int i = 0;
            while (i < attempsContainer.attemps[index].interactions.Count)
            {
                InitializeInteractionCommand command = new InitializeInteractionCommand();
                command.Queue(attempsContainer.attemps[index].interactions[i]);

                while (command.action == null)
                    await Task.Yield();
                
                command.action.Invoke();
                //InitializeInteraction(attempsContainer.attemps[index].interactions[i]).Invoke();

                i++;
            }

            attempsContainer.executedTimes++;
        }
        else
        {
            //unhandled event
        }
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
            else if (interaction.variablesAction == Interaction.VariablesAction.getLocalVariable)
            {
                action.AddListener(() =>
                varContainer.GetLocalVariable(interaction,
                                                interaction.variableObject.local_variables[interaction.localVariableSelected]));
            }
            else if (interaction.variablesAction == Interaction.VariablesAction.getGlobalVariable)
            {
                action.AddListener(() =>
                varContainer.GetGlobalVariable(interaction,
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

