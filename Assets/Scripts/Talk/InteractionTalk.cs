using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class InteractionTalk : IInteraction, IBackgroundInteraction
{
    IMessageTalker talker;
    string message;
    bool skippable;
    bool isBackground;

    // Start is called before the first frame update
    public async Task Execute()
    {
        if(!isBackground)
        { 
            talker.Talk(message, skippable);

            while(talker.Talking)
                await Task.Yield();
        }
        else
        {
            BackgroundInteractionManager.Instance.AddCommand(this);
        }
    }

    public async Task ExecuteInBackground()
    {
        talker.Talk(message, skippable);

        while (talker.Talking)
            await Task.Yield();
    }

    // Update is called once per frame
    public void Queue(IMessageTalker talker, string message, bool skippable, bool isBackground)
    {
        this.message = message;
        this.isBackground = isBackground;
        this.skippable = skippable;
        this.talker = talker;
        InteractionManager.Instance.AddCommand(this);
        
    }

    public void Skip()
    {
        talker.Skip();
    }
}
