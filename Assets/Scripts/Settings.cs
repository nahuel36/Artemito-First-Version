using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GlobalVariableProperty
{
    public string name;
    public int ID = -1;
    [System.Flags]
    public enum object_types
    {
        characters = (1 << 0),
        objects = (1 << 1),
        inventory = (1 << 2)
    }
    public object_types object_type;

    [System.Flags]
    public enum variable_types
    {
        integer = (1 << 0),
        boolean = (1 << 1),
        String = (1 << 2)
    }
    public variable_types variable_type;
}


[CreateAssetMenu(fileName = "Settings", menuName = "Pnc/SettingsFile", order = 1)]
public class Settings : ScriptableObject
{
    public enum SpeechStyle
    {
        LucasArts, 
        Sierra,
        Custom
    }

    public enum InteractionExecuteMethod
    {
        FirstActionThenObject,
        FirstObjectThenAction
    }
    public enum PathFindingType
    {
        UnityNavigationMesh,
        AronGranbergAStarPath,
        Custom
    }
    public string[] verbs;
    public GlobalVariableProperty[] global_variables;
    public PathFindingType pathFindingType;
    public SpeechStyle speechStyle;
    public InteractionExecuteMethod interactionExecuteMethod;
}
