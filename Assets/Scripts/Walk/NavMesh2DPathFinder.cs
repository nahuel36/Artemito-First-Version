using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavMesh2DPathFinder : IPathFinder
{
    NavMeshAgent walker;
    bool canceled;
    public bool Reached => walker.remainingDistance == 0;

    public bool Canceled => canceled;

    public NavMesh2DPathFinder(Transform walkerTransform) {
        walker = walkerTransform.gameObject.AddComponent<NavMeshAgent>();
        walkerTransform.gameObject.AddComponent<NavMeshPlus.Extensions.AgentOverride2d>();
    }

    public void Cancel()
    {
        walker.SetDestination(walker.transform.position);
        canceled = true;
    }

    public void WalkTo(Vector3 destiny, bool isCancelable)
    {
        walker.SetDestination(new Vector3(destiny.x,destiny.y, 0));

    }

}
