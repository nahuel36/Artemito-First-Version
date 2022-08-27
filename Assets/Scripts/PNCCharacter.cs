using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class Interaction
{
    [HideInInspector]public string name;
    public bool isCyclical = false;
    public bool show = true;
    public UnityEvent[] interactions;
}

[System.Serializable]
public class PnCInteractuableVariables
{
    public string name = "new variable";
    public int integer = 0;
    public bool integerDefault = true;
    public bool boolean = false;
    public bool booleanDefault = true;
    public string String = "";
    public bool stringDefault = true;
    public enum types{
        integer = (1 << 0),
        boolean = (1 << 1),
        String = (1 << 2)
    }
    public types type;
}


public class PNCCharacter : MonoBehaviour
{
    IPathFinder pathFinder;
    IMessageTalker messageTalker;
    CommandWalk cancelableWalk;
    CommandTalk skippabletalk;
    CommandTalk backgroundTalk;
    public Interaction[] interactions;
    public PnCInteractuableVariables[] variables;

    private void Awake()
    {

    }

    public void ConfigureTalker()
    {
        messageTalker = new LucasArtText(this.transform, new TextTimeCalculator());
    }

    public void ConfigurePathFinder(float velocity)
    {
        pathFinder = new AStarPathFinder(this.transform, velocity);
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

    public void CancelWalk()
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
