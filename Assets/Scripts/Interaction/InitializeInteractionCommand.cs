using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class InitializeInteractionCommand : ICommand
{
    public UnityEngine.Events.UnityEvent<List<CustomArgument>> action;
    private Interaction interaction;
    public async Task Execute()
    {
        await Task.Yield();

        action = InteractionUtils.InitializeInteraction(interaction);
    }

    public void Skip()
    {

    }

    public void Queue(Interaction interaction, bool runInBackground)
    {
        this.interaction = interaction;
        if (!runInBackground)
            CommandsQueue.Instance.AddCommand(this);
        else
            CommandsQueue.BackgroundInstance.AddCommand(this);
    }

}
