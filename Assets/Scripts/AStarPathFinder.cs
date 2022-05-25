using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class AStarPathFinder : IPathFinder
{
    GameObject target;
    AIPath aipath;
    bool canceled;
    bool isCancelable;

    public AStarPathFinder(GameObject target, AIPath aipath)
    {
        this.target = target;
        this.aipath = aipath;
    }

    // Start is called before the first frame update
    public void WalkTo(Vector3 destiny, bool isCancelable)
    {
        canceled = false;
        this.isCancelable = isCancelable;
        target.transform.position = destiny;
    }

    // Update is called once per frame
    public bool Reached
    {
        get { return aipath.reachedEndOfPath; }
    }

    public bool Canceled
    {
        get { return isCancelable && canceled; }
    }
    
    public void Cancel()
    {
        if(isCancelable)
            canceled = true;
    }
}
