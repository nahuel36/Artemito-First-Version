using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class InteractionTalk : IInteraction
{
    IMessageTalker talker;
    string message;
    bool skippable;

    // Start is called before the first frame update
    public async Task Execute()
    {
        talker.Talk(message, skippable);

        while(talker.Talking)
            await Task.Yield();
    }

    // Update is called once per frame
    public void Queue(IMessageTalker talker, string message, bool skippable)
    {
        this.message = message;
        this.skippable = skippable;
        this.talker = talker;
        InteractionManager.Instance.AddCommand(this);
    }
}
