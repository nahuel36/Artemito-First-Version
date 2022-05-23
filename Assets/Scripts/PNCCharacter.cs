using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PNCCharacter : MonoBehaviour
{
    IPathFinder pathFinder;
    [SerializeField] GameObject target;
    CharacterWalk cancelableWalk;

    private void Awake()
    {
        pathFinder = new AStarPathFinder(target, GetComponent<Pathfinding.AIPath>());
    }

    // Start is called before the first frame update
    public void Walk(Vector3 destiny)
    {
        CharacterWalk characterWalk = new CharacterWalk();
        characterWalk.Queue(pathFinder, destiny);
    }

    public void CancelableWalk(Vector3 destiny)
    {
        cancelableWalk = new CharacterWalk();
        cancelableWalk.Queue(pathFinder, destiny, true);
    }

    public void CancelWalk()
    {
        InteractionManager.Instance.ClearAll();
        if(cancelableWalk != null) cancelableWalk.Cancel();
    }
}
