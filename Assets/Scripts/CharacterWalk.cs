using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class CharacterWalk : IInteraction
{
    IPathFinder pathfinder;
    Vector3 destiny;

    // Start is called before the first frame update
    public async Task Execute()
    {
        pathfinder.WalkTo(destiny);

        await Task.Delay(1000);

        while (pathfinder.Reached == false)
            await Task.Yield();
    }

    // Update is called once per frame
    public void Queue(IPathFinder pathfinder, Vector3 destiny)
    {
        this.pathfinder = pathfinder;
        this.destiny = destiny;
        InteractionManager.Instance.AddCommand(this);
    }
}
