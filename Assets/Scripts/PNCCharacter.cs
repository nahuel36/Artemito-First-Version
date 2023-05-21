using System.Collections;
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
    public enum InteractionType { 
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
    public PNCCharacter variableObject;

    public enum GetVariableAction { 
        Stop,
        Continue,
        GoToSpecificLine
    }
    public GetVariableAction OnCompareResultTrueAction;
    public GetVariableAction OnCompareResultFalseAction;
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
    public bool global_BooleanValue;
    public string global_StringValue;
    public int global_IntegerValue;
    public bool local_BooleanValue;
    public string local_StringValue;
    public int local_IntegerValue;
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
        destiny.LineToGoOnFalseResult = LineToGoOnFalseResult;
        destiny.LineToGoOnTrueResult = LineToGoOnTrueResult;
        destiny.OnCompareResultFalseAction = OnCompareResultFalseAction;
        destiny.OnCompareResultTrueAction = OnCompareResultTrueAction;
    }
}


[System.Serializable]
public class InteractuableLocalVariable
{
    public string name = "new variable";
    public int integer = 0;
    public bool integerDefault = true;
    public bool boolean = false;
    public bool booleanDefault = true;
    public string String = "";
    public bool stringDefault = true;
    [System.Flags]
    public enum types{
        integer = (1 << 0),
        boolean = (1 << 1),
        String = (1 << 2)
    }
    public types type;
    public int globalHashCode = -1;
    public bool expandedInInspector;
}

[System.Serializable]
public class InteractuableGlobalVariable
{
    public string name = "new variable";
    public int integer = 0;
    public bool boolean = false;
    public string String = "";
    public int globalHashCode = -1;
    public GlobalVariableProperty properties;
    public bool expandedInInspector;
}


public class PNCCharacter : MonoBehaviour
{
    IPathFinder pathFinder;
    IMessageTalker messageTalker;
    CommandWalk cancelableWalk;
    CommandTalk skippabletalk;
    CommandTalk backgroundTalk;
    public List<Verb> verbs = new List<Verb>();
    public InteractuableLocalVariable[] local_variables = new InteractuableLocalVariable[0];
    public InteractuableGlobalVariable[] global_variables = new InteractuableGlobalVariable[0];

    private void Awake()
    {
        
    }

    public void ConfigureTalker()
    {
        messageTalker = new LucasArtText(this.transform, new TextTimeCalculator());
    }

    public void ConfigurePathFinder(float velocity)
    {
        Settings settings = Resources.Load<Settings>("Settings/Settings");
        if (settings.pathFindingType == Settings.PathFindingType.AronGranbergAStarPath)
            pathFinder = new AStarPathFinderAdapter(this.transform, velocity);
        else if (settings.pathFindingType == Settings.PathFindingType.UnityNavigationMesh)
            pathFinder = new NavMesh2DPathFinder(this.transform);
    }

    // Start is called before the first frame update
    public void Walk(Vector3 destiny)
    {
        CommandWalk characterWalk = new CommandWalk();
        characterWalk.Queue(pathFinder, destiny);
    }

    public void CancelableWalk(Vector3 destiny)
    {
        cancelableWalk = new CommandWalk();
        cancelableWalk.Queue(pathFinder, destiny, true);
    }

    public void CancelWalkAndTasks()
    {
        CommandsQueue.Instance.ClearAll();
        if(cancelableWalk != null) cancelableWalk.Skip();
    }

    public void Talk(string message)
    {
        skippabletalk = new CommandTalk();
        skippabletalk.Queue(messageTalker, message, true,false);
    }

    public void BackgroundTalk(string message)
    {
        backgroundTalk = new CommandTalk();
        backgroundTalk.Queue(messageTalker, message, true, true);
    }

    public void SkipTalk()
    {
        if(skippabletalk != null)
            skippabletalk.Skip();
    }

    public bool isTalking()
    {
        return (backgroundTalk != null && backgroundTalk.IsTalking()) || (skippabletalk != null && skippabletalk.IsTalking());
    }
}
