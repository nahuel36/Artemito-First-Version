using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine.AI;
using UnityEngine;
public class CommandWalkStraight : ICommand
{
    IPathFinder pathFinder;
    PNCCharacter character;
    Vector3 goToPoint;
    float stopDistance = 0.1f;
    int walkDelay = 10;
    float moveDistance = 0.05f;
    public async Task Execute()
    {
        Settings settings = Resources.Load<Settings>("Settings/Settings");

        pathFinder.Disable();

        while (Vector3.Distance(character.transform.position, goToPoint) > stopDistance)
        {
            character.transform.position = Vector3.MoveTowards(character.transform.position, goToPoint, moveDistance);
            await Task.Delay(walkDelay);
        }
    }

    public void Skip()
    {

    }

    public void Queue(PNCCharacter character, Vector3 point, IPathFinder pathFinder)
    {
        this.character = character;
        this.goToPoint = point;
        this.pathFinder = pathFinder;
        CommandsQueue.Instance.AddCommand(this);
    }
}
