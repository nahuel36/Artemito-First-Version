using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class AStarPathFinder : MonoBehaviour, IPathFinder
{
    // Start is called before the first frame update
    public void WalkTo(Vector3 destiny)
    {
        
    }

    // Update is called once per frame
    public bool Reached
    {
        get { return true; }
    }
}
