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
                attemps[j].interactions[k].action = new UnityEvent();
                Interaction interaction = attemps[j].interactions[k];
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
                    if (interaction.characterAction == Interaction.CharacterAction.sayWithScript)
                    {
                        if (interaction.CanSkip)
                            attemps[j].interactions[k].action.AddListener(() => charact.Talk(((SayScript)interaction.SayScript).SayWithScript()));
                        else
                            attemps[j].interactions[k].action.AddListener(() => charact.UnskippableTalk(((SayScript)interaction.SayScript).SayWithScript()));
                    }
                    if (interaction.characterAction == Interaction.CharacterAction.walk)
                    {
                        attemps[j].interactions[k].action.AddListener(() => charact.Walk(interaction.WhereToWalk.position));
                    }
                }
                if (interaction.type == Interaction.InteractionType.variables)
                {
                    PNCVariablesContainer varContainer = interaction.variableObject;
                    if (interaction.variablesAction == Interaction.VariablesAction.setLocalVariable)
                    {
                        attemps[j].interactions[k].action.AddListener(() =>
                        varContainer.SetLocalVariable(interaction,
                                                        interaction.variableObject.local_variables[interaction.localVariableSelected]));
                    }
                    if (interaction.variablesAction == Interaction.VariablesAction.setGlobalVariable)
                    {
                        attemps[j].interactions[k].action.AddListener(() =>
                        varContainer.SetGlobalVariable(interaction,
                                                        interaction.variableObject.global_variables[interaction.globalVariableSelected]));
                    }
                    if (interaction.variablesAction == Interaction.VariablesAction.getLocalVariable)
                    {
                        attemps[j].interactions[k].action.AddListener(() =>
                        varContainer.GetLocalVariable(interaction,
                                                        interaction.variableObject.local_variables[interaction.localVariableSelected]));
                    }
                    if (interaction.variablesAction == Interaction.VariablesAction.getGlobalVariable)
                    {
                        attemps[j].interactions[k].action.AddListener(() =>
                        varContainer.GetGlobalVariable(interaction,
                                                        interaction.variableObject.global_variables[interaction.globalVariableSelected]));
                    }
                }

            }
        }
    }
}
