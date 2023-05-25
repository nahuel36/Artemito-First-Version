using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;



public interface SayScript
{
    string SayWithScript();
}


public class PNCCharacter : PNCInteractuable
{
    IPathFinder pathFinder;
    IMessageTalker messageTalker;
    CommandWalk cancelableWalk;
    CommandTalk normalTalk;
    CommandTalk backgroundTalk;

    [SerializeField]Sprite SierraTextFace;

    Animator anim;
    CharacterAnimator characterAnimator;

    private void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        ConfigurePathFinder(1);
        ConfigureTalker();
        CharacterAnimatorAdapter characterAnimatorAdapter = new CharacterAnimatorAdapter();
        characterAnimatorAdapter.Configure(anim);
        characterAnimator = GetComponentInChildren<CharacterAnimator>();
        characterAnimator.Configure(characterAnimatorAdapter, this);
    }

    public void ConfigureTalker()
    {
        Settings settings = Resources.Load<Settings>("Settings/Settings");
        if (settings.speechStyle == Settings.SpeechStyle.LucasArts)
            messageTalker = new LucasArtText(this.transform, new TextTimeCalculator());
        else if (settings.speechStyle == Settings.SpeechStyle.Sierra)
            messageTalker = new SierraText(this.transform, new TextTimeCalculator(), SierraTextFace);
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
        normalTalk = new CommandTalk();
        normalTalk.Queue(messageTalker, message, true,false);
    }

    public void UnskippableTalk(string message)
    {
        normalTalk = new CommandTalk();
        normalTalk.Queue(messageTalker, message, false, false);
    }


    public void BackgroundTalk(string message)
    {
        backgroundTalk = new CommandTalk();
        backgroundTalk.Queue(messageTalker, message, true, true);
    }

    public void SkipTalk()
    {
        if(normalTalk != null)
            normalTalk.Skip();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
            SkipTalk();

    }

    public bool isTalking()
    {
        return (backgroundTalk != null && backgroundTalk.IsTalking()) || (normalTalk != null && normalTalk.IsTalking());
    }
}
