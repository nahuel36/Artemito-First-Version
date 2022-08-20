using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class Interaction
{
    [HideInInspector]public string name;
    public UnityEvent[] interactions;
    public bool isCyclical = false;
    public bool show = true;
}


public class PNCCharacter : MonoBehaviour
{
    IPathFinder pathFinder;
    IMessageTalker messageTalker;
    InteractionWalk cancelableWalk;
    InteractionTalk skippabletalk;
    InteractionTalk backgroundTalk;
    public Interaction[] interactions;

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
        InteractionWalk characterWalk = new InteractionWalk();
        characterWalk.Queue(pathFinder, destiny);
    }

    public void CancelableWalk(Vector3 destiny)
    {
        cancelableWalk = new InteractionWalk();
        cancelableWalk.Queue(pathFinder, destiny, true);
    }

    public void CancelWalk()
    {
        InteractionManager.Instance.ClearAll();
        if(cancelableWalk != null) cancelableWalk.Skip();
    }

    public void Talk(string message)
    {
        skippabletalk = new InteractionTalk();
        skippabletalk.Queue(messageTalker, message, true,false);
    }

    public void BackgroundTalk(string message)
    {
        backgroundTalk = new InteractionTalk();
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
