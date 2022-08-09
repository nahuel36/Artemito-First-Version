using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PNCCharacter : MonoBehaviour
{
    IPathFinder pathFinder;
    IMessageTalker messageTalker;
    [SerializeField] GameObject target;
    InteractionWalk cancelableWalk;
    InteractionTalk skippabletalk;
    InteractionTalk backgroundTalk;

    private void Awake()
    {
        pathFinder = new AStarPathFinder(target, this.transform);
        messageTalker = new LucasArtText(this.transform, new TextTimeCalculator());

    }

    public void Configure(GameObject target)
    {
        this.target = target;
        pathFinder = new AStarPathFinder(target, this.transform);
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
