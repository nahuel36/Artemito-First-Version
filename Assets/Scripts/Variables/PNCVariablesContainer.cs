using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class InteractuableLocalVariable
{
    public string name = "new variable";
    public int integer = 0;
    public bool integerDefault = true;
    public bool boolean = false;
    public bool booleanDefault = true;
    public string String = "";
    public bool stringDefault = true;
    [System.Flags]
    public enum types
    {
        integer = (1 << 0),
        boolean = (1 << 1),
        String = (1 << 2)
    }
    public types type;
    public int globalHashCode = -1;
    public bool expandedInInspector;

}

[System.Serializable]
public class InteractuableGlobalVariable
{
    public string name = "new variable";
    public int integer = 0;
    public bool boolean = false;
    public string String = "";
    public int globalHashCode = -1;
    public GlobalVariableProperty properties;
    public bool expandedInInspector;
    public bool integerDefault = true;
    public bool booleanDefault = true;
    public bool stringDefault = true;
}

public class PNCVariablesContainer : MonoBehaviour
{
    public InteractuableLocalVariable[] local_variables = new InteractuableLocalVariable[0];
    public InteractuableGlobalVariable[] global_variables = new InteractuableGlobalVariable[0];

    public void SetLocalVariable(Interaction interact, InteractuableLocalVariable variable)
    {
        CommandSetLocalVariable command = new CommandSetLocalVariable();
        command.Queue(variable, interact);
    }

    internal void SetGlobalVariable(Interaction interaction, InteractuableGlobalVariable interactuableGlobalVariable)
    {
        CommandSetGlobalVariable command = new CommandSetGlobalVariable();
        command.Queue(interactuableGlobalVariable, interaction);
    }

}
