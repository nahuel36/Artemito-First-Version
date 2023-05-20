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
    //CUSTOM
    public UnityEvent action;

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
