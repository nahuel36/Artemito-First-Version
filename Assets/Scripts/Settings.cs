using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GlobalVariableProperty
{
    public string name;
    public int ID;
    [System.Flags]
    public enum types
    {
        characters = (1 << 0),
        objects = (1 << 1),
        inventory = (1 << 2)
    }
    public types type;
}


[CreateAssetMenu(fileName = "Settings", menuName = "Pnc/SettingsFile", order = 1)]
public class Settings : ScriptableObject
{
    public string[] modes;
    public GlobalVariableProperty[] global_variables;
}
