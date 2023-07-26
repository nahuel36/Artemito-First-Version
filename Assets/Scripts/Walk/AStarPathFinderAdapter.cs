using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if ASTAR_ARONGRANBERG_PATHFINDING
using Pathfinding;
#endif

public class AStarPathFinderAdapter : IPathFinder
{
    #if ASTAR_ARONGRANBERG_PATHFINDING
    GameObject target;
    AIPath aipath;
    bool canceled;
    bool isCancelable;
    AIDestinationSetter setter;
    #endif

    public AStarPathFinderAdapter(UnityEngine.Transform transform, float velocity)
    {
#if ASTAR_ARONGRANBERG_PATHFINDING
        target = new GameObject("target");
        target.transform.position = transform.position;

        aipath = transform.gameObject.AddComponent<AIPath>();
        aipath.gravity = Vector3.zero;
        aipath.orientation = OrientationMode.YAxisForward;
        aipath.enableRotation = false;
        aipath.maxSpeed = velocity;
        aipath.pickNextWaypointDist = GameObject.FindObjectOfType<WalkableArea2D>().node_size;
        aipath.FindComponents();

        setter = transform.gameObject.AddComponent<AIDestinationSetter>();
        setter.target = target.transform;
#endif
    }


    // Start is called before the first frame update
    public void WalkTo(Vector3 destiny, bool isCancelable)
    {
#if ASTAR_ARONGRANBERG_PATHFINDING

        canceled = false;
        this.isCancelable = isCancelable;
        target.transform.position = destiny;
#endif

    }

    // Update is called once per frame
    public bool Reached
    {
        get {
#if ASTAR_ARONGRANBERG_PATHFINDING
            return aipath.reachedEndOfPath; 
#else
            return true;
#endif
        }
    }

    public bool Canceled
    {
        get {
#if ASTAR_ARONGRANBERG_PATHFINDING
            return isCancelable && canceled; 
#else
            return false;
#endif        
        }
    }
    
    public void Cancel()
    {
#if ASTAR_ARONGRANBERG_PATHFINDING
        if (isCancelable)
        {
            target.transform.position = aipath.transform.position;
            canceled = true;
        }
#endif        
    }
}
