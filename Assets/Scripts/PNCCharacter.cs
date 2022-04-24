using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PNCCharacter : MonoBehaviour
{
    IPathFinder pathFinder;
    [SerializeField] GameObject target;

    private void Awake()
    {
        pathFinder = new AStarPathFinder(target, GetComponent<Pathfinding.AIPath>());
    }

    // Start is called before the first frame update
    public void Walk(Vector3 destiny)
    {
        pathFinder.WalkTo(destiny);
    }

}
