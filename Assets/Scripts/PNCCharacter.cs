using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class Mode
{
    public string name;
    public bool isCyclical = false;
    public bool show = true;
    public InteractionList[] interactionsLists;
}

[System.Serializable]
public class InteractionList
{
    [HideInInspector]public string name;
    public Interaction[] interactions;
}

[System.Serializable]
public class Interaction
{
    public enum InteractionType { 
        character,
        custom
    }

    public InteractionType type;
    
    public UnityEvent action;
    public Character character;
}

public interface Character
{ 

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
}


public class PNCCharacter : MonoBehaviour, Character
{
    IPathFinder pathFinder;
    IMessageTalker messageTalker;
    CommandWalk cancelableWalk;
    CommandTalk skippabletalk;
    CommandTalk backgroundTalk;
    public Mode[] interactions = new Mode[0];
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
