using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public class PnCMenu : Editor
{
    [MenuItem("PnC/Go to Settings")]
    static void GoToSettings()
    {
        Settings settings = Resources.Load<Settings>("Settings/Settings");
        Selection.activeObject = settings;

    }

    [MenuItem("PnC/PathFinding 2D/Install Aron Granberg - AStar PathFinding")]
    static void InstallAStar()
    {
        PnCEditorWindow.Init("To continue, you have to have installed \n Aron Granberg - AStar PathFinding in this proyect.\n \n Additionally, you have to create layer 'Obstacle' \n \n If you have done this, press OK, \n otherwise press Cancel", InstallAStarDirective);
        
    }

    private static void InstallAStarDirective()
    {
        EditorUtils.AddScriptingDefineSymbol("ASTAR_ARONGRANBERG_PATHFINDING");
        
    }

    private static void InstallNavMeshDirective()
    {
        EditorUtils.AddScriptingDefineSymbol("NAVMESH_PLUS");
    }

    [MenuItem("PnC/PathFinding 2D/Install Nav Mesh Plus")]
    static void InstallNavMeshPlus()
    {
        PnCEditorWindow.Init("To continue, you have to have installed \n NavMesh Plus in this proyect. \n If you have it, press OK, \n otherwise press Cancel", InstallNavMeshDirective);

    }
}
