using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using NavMeshPlus;
using UnityEngine.AI;
[CustomEditor(typeof(WalkableArea2D))]
public class WalkableArea2DEditor : Editor
{
    Settings settings = null;
    public override void OnInspectorGUI()
    {
        WalkableArea2D myTarget = (WalkableArea2D)target;

        if(settings == null)
             settings = Resources.Load<Settings>("Settings/Settings");

        if (settings.pathFindingType == Settings.PathFindingType.AronGranbergAStarPath)
        { 
            EditorGUILayout.LabelField("Using AStarPathfinding");
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Node Size");
            ((WalkableArea2D)target).node_size = EditorGUILayout.FloatField(myTarget.node_size);
            EditorGUILayout.EndHorizontal();
        }
        if (settings.pathFindingType == Settings.PathFindingType.UnityNavigationMesh)
            EditorGUILayout.LabelField("Using Unity Navigation Mesh");
        if (GUILayout.Button("Change PathFinder"))
        {
            Selection.activeObject = settings;
        }
        if (GUILayout.Button("Create PathFinder"))
        {
            for (int i = 0; i < myTarget.transform.childCount; i++)
                DestroyImmediate(myTarget.transform.GetChild(i).gameObject);


            Collider2D collider = myTarget.GetComponent<Collider2D>();

            Settings settings = Resources.Load<Settings>("Settings/Settings");

            for (int i = myTarget.GetComponents<NavMeshPlus.Components.NavMeshModifier>().Length-1; i >= 0 ; i--)
            {
                DestroyImmediate(myTarget.GetComponents<NavMeshPlus.Components.NavMeshModifier>()[i]);
            }
            if(AstarPath.active)
                DestroyImmediate(AstarPath.active.gameObject);
            if (FindObjectOfType<NavMeshPlus.Components.NavMeshSurface>())
                DestroyImmediate(FindObjectOfType<NavMeshPlus.Components.NavMeshSurface>().gameObject);

            if (settings.pathFindingType == Settings.PathFindingType.AronGranbergAStarPath)
            {
                GameObject pathGO = Instantiate(Resources.Load<GameObject>("Prefabs/AStarPathFinderPrefab"), myTarget.transform);
                AstarPath.FindAstarPath();
                ((Pathfinding.GridGraph)AstarPath.active.data.FindGraphOfType(typeof(Pathfinding.GridGraph)))
                    .SetDimensions(
                        Mathf.RoundToInt(collider.bounds.size.x / myTarget.node_size),
                        Mathf.RoundToInt(collider.bounds.size.y / myTarget.node_size),
                        myTarget.node_size);
                ((Pathfinding.GridGraph)AstarPath.active.data.FindGraphOfType(typeof(Pathfinding.GridGraph))).center = collider.offset + (Vector2)myTarget.transform.position;
                ((Pathfinding.GridGraph)AstarPath.active.data.FindGraphOfType(typeof(Pathfinding.GridGraph))).Scan();

                Selection.activeGameObject = pathGO;
            }
            if (settings.pathFindingType == Settings.PathFindingType.UnityNavigationMesh)
            {
                myTarget.gameObject.AddComponent<NavMeshPlus.Components.NavMeshModifier>();

                GameObject GO = new GameObject("NavMeshManager");
                GO.transform.rotation = Quaternion.Euler(-90, 0, 0);
                NavMeshPlus.Components.NavMeshSurface surface = GO.AddComponent<NavMeshPlus.Components.NavMeshSurface>();
                GO.AddComponent<NavMeshPlus.Extensions.CollectSources2d>();

                surface.useGeometry = NavMeshCollectGeometry.PhysicsColliders;

                surface.AddData();
                surface.BuildNavMesh();
                surface.UpdateNavMesh(surface.navMeshData);

                GO.transform.parent = myTarget.transform;

                
            }
        }


    }

    

}
