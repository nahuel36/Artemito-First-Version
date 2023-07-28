using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class InitializeInteractionCommand : ICommand
{
    public UnityEngine.Events.UnityEvent action;
    private Interaction interaction;
    public async Task Execute()
    {
        await Task.Yield();

        action = InteractionUtils.InitializeInteraction(interaction);
    }

    public void Skip()
    {

    }

    public void Queue(Interaction interaction)
    {
        this.interaction = interaction;
        CommandsQueue.Instance.AddCommand(this);
    }

}
