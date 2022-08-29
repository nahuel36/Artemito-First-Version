using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Settings", menuName = "Pnc/SettingsFile", order = 1)]

public class Settings : ScriptableObject
{
    public string[] modes;
    public GlobalVariableProperty[] global_variables;
}
