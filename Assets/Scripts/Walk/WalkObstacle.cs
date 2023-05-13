using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Collider2D))]
public class WalkObstacle : MonoBehaviour
{
    // Start is called before the first frame update
    public void Init()
    {
#if NAVMESH_PLUS
        for (int i = GetComponents<NavMeshPlus.Components.NavMeshModifier>().Length - 1; i >= 0; i--)
        {
            DestroyImmediate(GetComponents<NavMeshPlus.Components.NavMeshModifier>()[i]);
        }
#endif

        Settings settings = Resources.Load<Settings>("Settings/Settings");
        if (settings.pathFindingType == Settings.PathFindingType.UnityNavigationMesh)
        {
#if NAVMESH_PLUS

            NavMeshPlus.Components.NavMeshModifier modifier = this.gameObject.AddComponent<NavMeshPlus.Components.NavMeshModifier>();
            modifier.overrideArea = true;
            modifier.area = 1;//not walkable
#endif

        }
        else if (settings.pathFindingType == Settings.PathFindingType.AronGranbergAStarPath)
        {
#if ASTAR_ARONGRANBERG_PATHFINDING
            this.gameObject.layer = LayerMask.NameToLayer("Obstacle");
#endif
        }

    }

}
