using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPathFinder
{
    void WalkTo(Vector3 destiny, bool isCancelable);
    
    bool Reached { get; }

    void Cancel();

    bool Canceled { get; }
}
