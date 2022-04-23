using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPathFinder
{
    void WalkTo(Vector3 destiny);

    bool Reached { get; }

}
