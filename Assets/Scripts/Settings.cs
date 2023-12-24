using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GlobalPropertyConfig
{
    public string name;
    public int ID = -1;
    public PropertyObjectType objectTypes;
    public VariableType variableTypes;
    
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
    public enum ObjetivePosition 
    { 
        fixedPosition,
        overCursor
    }
    public Verb[] verbs;
    public int verbIndex;
    public GlobalPropertyConfig[] globalPropertiesConfig = new GlobalPropertyConfig[0];
    public int global_propertiesIndex;
    public PathFindingType pathFindingType;
    public SpeechStyle speechStyle;
    public InteractionExecuteMethod interactionExecuteMethod;
    public ObjetivePosition objetivePosition;
    public bool showNumbersInDialogOptions = false;
    public bool alwaysShowAllVerbs = false;
}
