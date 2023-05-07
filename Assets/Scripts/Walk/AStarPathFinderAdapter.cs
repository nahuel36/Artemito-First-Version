using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class AStarPathFinderAdapter : IPathFinder
{
    GameObject target;
    AIPath aipath;
    bool canceled;
    bool isCancelable;
    AIDestinationSetter setter;

    public AStarPathFinderAdapter(UnityEngine.Transform transform, float velocity)
    {
        target = new GameObject("target");
        target.transform.position = transform.position;

        aipath = transform.gameObject.AddComponent<AIPath>();
        aipath.gravity = Vector3.zero;
        aipath.orientation = OrientationMode.YAxisForward;
        aipath.enableRotation = false;
        aipath.maxSpeed = velocity;
        aipath.FindComponents();

        setter = transform.gameObject.AddComponent<AIDestinationSetter>();
        setter.target = target.transform;
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
        if (isCancelable)
        {
            target.transform.position = aipath.transform.position;
            canceled = true;
        }

    }
}
