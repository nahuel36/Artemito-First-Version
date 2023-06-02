using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class Verb
{
    public string name;
    public bool isCyclical = false;
    public bool use = true;
    public List<InteractionsAttemp> attemps = new List<InteractionsAttemp>();
    public bool expandedInInspector;
    public int executedTimes = 0;
}

[System.Serializable]
public class InventoryItemAction {
    public int specialIndex = -1;
    public string name;
    public bool isCyclical = false;
    public List<InteractionsAttemp> attemps = new List<InteractionsAttemp>();
    public bool expandedInInspector;
    public int executedTimes = 0;
}


[System.Serializable]
public class InteractionsAttemp
{
    public List<Interaction> interactions = new List<Interaction>();
    public bool expandedInInspector;
}

[System.Serializable]
public class Interaction
{
    public enum InteractionType
    {
        character,
        variables,
        custom
    }
    public InteractionType type;

    public bool expandedInInspector;

    //CHARACTER
    public enum CharacterAction
    {
        say,
        sayWithScript,
        walk
    }
    public PNCCharacter character;
    public CharacterAction characterAction;
    public string WhatToSay;
    public Transform WhereToWalk;
    //VARIABLES
    public enum VariablesAction
    {
        setLocalVariable,
        getLocalVariable,
        setGlobalVariable,
        getGlobalVariable
    }
    public int globalVariableSelected;
    public int localVariableSelected;
    public VariablesAction variablesAction;
    public PNCVariablesContainer variableObject;


    public Conditional.GetVariableAction OnCompareResultTrueAction;
    public Conditional.GetVariableAction OnCompareResultFalseAction;
    public int LineToGoOnTrueResult;
    public int LineToGoOnFalseResult;
    //CUSTOM
    public UnityEvent action;
    public bool global_changeBooleanValue;
    public bool global_changeStringValue;
    public bool global_changeIntegerValue;
    public bool local_changeBooleanValue;
    public bool local_changeStringValue;
    public bool local_changeIntegerValue;
    public bool global_compareBooleanValue;
    public bool global_compareStringValue;
    public bool global_compareIntegerValue;
    public bool local_compareBooleanValue;
    public bool local_compareStringValue;
    public bool local_compareIntegerValue;
    public bool local_defaultBooleanValue;
    public int local_defaultIntegerValue;
    public string local_defaultStringValue;
    public bool global_defaultBooleanValue;
    public int global_defaultIntegerValue;
    public string global_defaultStringValue;
    public bool global_BooleanValue;
    public string global_StringValue;
    public int global_IntegerValue;
    public bool local_BooleanValue;
    public string local_StringValue;
    public int local_IntegerValue;
    public MonoBehaviour SayScript;
    public bool CanSkip;
    //averiguar sobre deep copy / clone
    public void Copy(Interaction destiny)
    {
        destiny.action = action;
        destiny.character = character;
        destiny.characterAction = characterAction;
        destiny.expandedInInspector = false;
        destiny.globalVariableSelected = globalVariableSelected;
        destiny.localVariableSelected = localVariableSelected;
        destiny.type = type;
        destiny.variableObject = variableObject;
        destiny.variablesAction = variablesAction;
        destiny.WhatToSay = WhatToSay;
        destiny.WhereToWalk = WhereToWalk;
        destiny.local_changeBooleanValue = local_changeBooleanValue;
        destiny.local_changeIntegerValue = local_changeIntegerValue;
        destiny.local_changeStringValue = local_changeStringValue;
        destiny.global_changeBooleanValue = global_changeBooleanValue;
        destiny.global_changeIntegerValue = global_changeIntegerValue;
        destiny.global_changeStringValue = global_changeStringValue;
        destiny.local_compareBooleanValue = local_compareBooleanValue;
        destiny.local_compareIntegerValue = local_compareIntegerValue;
        destiny.local_compareStringValue = local_compareStringValue;
        destiny.global_compareBooleanValue = global_compareBooleanValue;
        destiny.global_compareIntegerValue = global_compareIntegerValue;
        destiny.global_compareStringValue = global_compareStringValue;
        destiny.local_BooleanValue = local_BooleanValue;
        destiny.local_IntegerValue = local_IntegerValue;
        destiny.local_StringValue = local_StringValue;
        destiny.global_BooleanValue = global_BooleanValue;
        destiny.global_IntegerValue = global_IntegerValue;
        destiny.global_StringValue = global_StringValue;
        destiny.local_defaultBooleanValue = local_defaultBooleanValue;
        destiny.local_defaultIntegerValue = local_defaultIntegerValue;
        destiny.local_defaultStringValue = local_defaultStringValue;
        destiny.global_defaultBooleanValue = global_defaultBooleanValue;
        destiny.global_defaultIntegerValue = global_defaultIntegerValue;
        destiny.global_defaultStringValue = global_defaultStringValue;
        destiny.LineToGoOnFalseResult = LineToGoOnFalseResult;
        destiny.LineToGoOnTrueResult = LineToGoOnTrueResult;
        destiny.OnCompareResultFalseAction = OnCompareResultFalseAction;
        destiny.OnCompareResultTrueAction = OnCompareResultTrueAction;
        destiny.SayScript = SayScript;
        destiny.CanSkip = CanSkip;
    }
}

public class PNCInteractuable : PNCVariablesContainer
{

    public string name;

    public List<Verb> verbs = new List<Verb>();
    public List<InventoryItemAction> inventoryActions = new List<InventoryItemAction>();

    


    private void Start()
    {
        for (int i = 0; i < verbs.Count; i++)
        {
            InteractionUtils.InitializeInteractions(ref verbs[i].attemps);
        }
        for (int i = 0; i < inventoryActions.Count; i++)
        {
            InteractionUtils.InitializeInteractions(ref inventoryActions[i].attemps);
        }
    }


    public string[] getActiveVerbs() 
    {
        List<string> activeVerbs = new List<string>();
        for (int i = 0; i < verbs.Count; i++)
        {
            if (verbs[i].use)
                activeVerbs.Add(verbs[i].name);
        }
        return activeVerbs.ToArray();
    }

    public void RunInteraction(string verbToRunString) 
    {
        
        Verb verbToRun = FindVerb(verbToRunString);

        for (int i = 0; i < verbToRun.attemps[verbToRun.executedTimes].interactions.Count; i++)
        {
            verbToRun.attemps[verbToRun.executedTimes].interactions[i].action.Invoke();
        }
        InteractionUtils.increaseExecutedTimes(ref verbToRun.executedTimes, verbToRun.attemps.Count, verbToRun.isCyclical);

    }



    public void RunInventoryInteraction(InventoryItem item)
    {
        int index = InventoryManager.Instance.getInventoryActionsIndex(item, inventoryActions);
        if (index != -1)
        {
            int times = inventoryActions[index].executedTimes;
            for (int j = 0; j < inventoryActions[index].attemps[times].interactions.Count; j++)
            {
                inventoryActions[index].attemps[times].interactions[j].action.Invoke();
            }
            InteractionUtils.increaseExecutedTimes(ref inventoryActions[index].executedTimes, inventoryActions[index].attemps.Count, inventoryActions[index].isCyclical);
        }
    }

    public Verb FindVerb(string verb) {
        for (int i = 0; i < verbs.Count; i++)
        {
            if (verbs[i].name == verb)
                return verbs[i];
        }
        return null;
    }
}
