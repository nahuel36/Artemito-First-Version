using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class WalkableArea2D : MonoBehaviour
{

    public float node_size = 1;

    private IEnumerator Start()
    {
        yield return new WaitUntil(() => MultipleScenesManager.Instance != null && MultipleScenesManager.Instance.allZoneScenesInitialized);

        for (int i = transform.childCount - 1; i >= 0; i--)
            DestroyImmediate(transform.GetChild(i).gameObject);


        Collider2D collider = GetComponent<Collider2D>();

        Settings settings = Resources.Load<Settings>("Settings/Settings");
#if NAVMESH_PLUS
        for (int i = GetComponents<NavMeshPlus.Components.NavMeshModifier>().Length - 1; i >= 0; i--)
        {
            DestroyImmediate(GetComponents<NavMeshPlus.Components.NavMeshModifier>()[i]);
        }
        if (FindObjectOfType<NavMeshPlus.Components.NavMeshSurface>())
            DestroyImmediate(FindObjectOfType<NavMeshPlus.Components.NavMeshSurface>().gameObject);
#endif
#if ASTAR_ARONGRANBERG_PATHFINDING
        if (AstarPath.active)
            DestroyImmediate(AstarPath.active.gameObject);
#endif
        WalkObstacle[] obstacles = FindObjectsOfType<WalkObstacle>();
        for (int i = 0; i < obstacles.Length; i++)
        {
            obstacles[i].Init();
        }

        if (settings.pathFindingType == Settings.PathFindingType.AronGranbergAStarPath)
        {
#if ASTAR_ARONGRANBERG_PATHFINDING
            GameObject pathGO = Instantiate(Resources.Load<GameObject>("Prefabs/AStarPathFinderPrefab"), transform);
            AstarPath.FindAstarPath();
            ((Pathfinding.GridGraph)AstarPath.active.data.FindGraphOfType(typeof(Pathfinding.GridGraph)))
                .SetDimensions(
                    Mathf.RoundToInt(collider.bounds.size.x / node_size),
                    Mathf.RoundToInt(collider.bounds.size.y / node_size),
                    node_size);
            ((Pathfinding.GridGraph)AstarPath.active.data.FindGraphOfType(typeof(Pathfinding.GridGraph))).center = collider.offset + (Vector2)transform.position;
            ((Pathfinding.GridGraph)AstarPath.active.data.FindGraphOfType(typeof(Pathfinding.GridGraph))).Scan();
#endif
        }
        if (settings.pathFindingType == Settings.PathFindingType.UnityNavigationMesh)
        {
#if NAVMESH_PLUS
            gameObject.AddComponent<NavMeshPlus.Components.NavMeshModifier>();

            GameObject GO = new GameObject("NavMeshManager");
            GO.transform.rotation = Quaternion.Euler(-90, 0, 0);
            NavMeshPlus.Components.NavMeshSurface surface = GO.AddComponent<NavMeshPlus.Components.NavMeshSurface>();
            GO.AddComponent<NavMeshPlus.Extensions.CollectSources2d>();

            surface.useGeometry = UnityEngine.AI.NavMeshCollectGeometry.PhysicsColliders;

            surface.AddData();
            surface.BuildNavMesh();
            surface.UpdateNavMesh(surface.navMeshData);

            GO.transform.parent = transform;

#endif
        }

    }

}
