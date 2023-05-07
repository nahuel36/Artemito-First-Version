using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if NAVMESH_PLUS
using UnityEngine.AI;
#endif

public class NavMesh2DPathFinder : IPathFinder
{
#if NAVMESH_PLUS
    NavMeshAgent walker;
    bool canceled;
#endif
    public bool Reached { 
        
        get {
#if NAVMESH_PLUS
            return walker.remainingDistance == 0; 
#else
            return true;
#endif

        }
    } 

    public bool Canceled { 
        get {
#if NAVMESH_PLUS
            return canceled; 
#else
            return false;
#endif 
        } }

    public NavMesh2DPathFinder(Transform walkerTransform) {
#if NAVMESH_PLUS
        walker = walkerTransform.gameObject.AddComponent<NavMeshAgent>();
        walkerTransform.gameObject.AddComponent<NavMeshPlus.Extensions.AgentOverride2d>();
#endif
    }

    public void Cancel()
    {
#if NAVMESH_PLUS

        walker.SetDestination(walker.transform.position);
        canceled = true;
#endif
    }

    public void WalkTo(Vector3 destiny, bool isCancelable)
    {
#if NAVMESH_PLUS

        walker.SetDestination(new Vector3(destiny.x,destiny.y, 0));
#endif
    }

}
