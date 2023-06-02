using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public static class InteractionUtils 
{
    public static void InitializeInteractions(ref List<InteractionsAttemp> attemps)
    {
        for (int j = 0; j < attemps.Count; j++)
        {
            for (int k = 0; k < attemps[j].interactions.Count; k++)
            {
                Interaction interaction = attemps[j].interactions[k];
                if (interaction.type != Interaction.InteractionType.custom)
                    attemps[j].interactions[k].action = new UnityEvent();
                if (interaction.type == Interaction.InteractionType.character)
                {
                    PNCCharacter charact = interaction.character;
                    if (interaction.characterAction == Interaction.CharacterAction.say)
                    {
                        string whattosay = interaction.WhatToSay;
                        if (interaction.CanSkip)
                            attemps[j].interactions[k].action.AddListener(() => charact.Talk(whattosay));
                        else
                            attemps[j].interactions[k].action.AddListener(() => charact.UnskippableTalk(whattosay));
                    }
                    else if (interaction.characterAction == Interaction.CharacterAction.sayWithScript)
                    {
                        if (interaction.CanSkip)
                            attemps[j].interactions[k].action.AddListener(() => charact.Talk(((SayScript)interaction.SayScript).SayWithScript()));
                        else
                            attemps[j].interactions[k].action.AddListener(() => charact.UnskippableTalk(((SayScript)interaction.SayScript).SayWithScript()));
                    }
                    else if (interaction.characterAction == Interaction.CharacterAction.walk)
                    {
                        attemps[j].interactions[k].action.AddListener(() => charact.Walk(interaction.WhereToWalk.position));
                    }
                }
                else if (interaction.type == Interaction.InteractionType.variables)
                {
                    PNCVariablesContainer varContainer = interaction.variableObject;
                    if (interaction.variablesAction == Interaction.VariablesAction.setLocalVariable)
                    {
                        attemps[j].interactions[k].action.AddListener(() =>
                        varContainer.SetLocalVariable(interaction,
                                                        interaction.variableObject.local_variables[interaction.localVariableSelected]));
                    }
                    else if (interaction.variablesAction == Interaction.VariablesAction.setGlobalVariable)
                    {
                        attemps[j].interactions[k].action.AddListener(() =>
                        varContainer.SetGlobalVariable(interaction,
                                                        interaction.variableObject.global_variables[interaction.globalVariableSelected]));
                    }
                    else if (interaction.variablesAction == Interaction.VariablesAction.getLocalVariable)
                    {
                        attemps[j].interactions[k].action.AddListener(() =>
                        varContainer.GetLocalVariable(interaction,
                                                        interaction.variableObject.local_variables[interaction.localVariableSelected]));
                    }
                    else if (interaction.variablesAction == Interaction.VariablesAction.getGlobalVariable)
                    {
                        attemps[j].interactions[k].action.AddListener(() =>
                        varContainer.GetGlobalVariable(interaction,
                                                        interaction.variableObject.global_variables[interaction.globalVariableSelected]));
                    }
                }

            }
        }
    }

    public static void increaseExecutedTimes(ref int executedTimes, int count, bool isCyclical)
    {
        if (executedTimes + 1 == count)
        {
            if (isCyclical) executedTimes = 0;
            else executedTimes = count - 1;
        }
        else
            executedTimes++;
    }
}
