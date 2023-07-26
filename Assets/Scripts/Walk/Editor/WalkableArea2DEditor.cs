using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
#if NAVMESH_PLUS
using NavMeshPlus;
using UnityEngine.AI;
#endif

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
       
            
        


    }

    

}
