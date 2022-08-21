using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class CommandTalk : ICommand
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
            this.isBackground = false;
            CommandsQueue.BackgroundInstance.AddCommand(this);
        }
    }

    // Update is called once per frame
    public void Queue(IMessageTalker talker, string message, bool skippable, bool isBackground)
    {
        this.message = message;
        this.isBackground = isBackground;
        this.skippable = skippable;
        this.talker = talker;
        CommandsQueue.Instance.AddCommand(this);
        
    }

    public void Skip()
    {
        talker.Skip();
    }

    public bool IsTalking()
    {
        return talker.Talking;
    }
}
