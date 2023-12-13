using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LocalProperty
{
    public string name = "new property";
    public bool hasInteger = false;
    public int integer = 0;
    public bool integerDefault = true;
    public bool hasBoolean = false;
    public bool boolean = false;
    public bool booleanDefault = true;
    public bool hasString = false;
    public string String = "";
    public bool stringDefault = true;
    public bool expandedInInspector;

}

[System.Serializable]
public class GlobalProperty
{
    public string name = "new property";
    public int integer = 0;
    public bool boolean = false;
    public string String = "";
    public int globalID = -1;
    public GlobalPropertyConfig config;
    public bool expandedInInspector;
    public bool integerDefault = true;
    public bool booleanDefault = true;
    public bool stringDefault = true;
}

public class PNCPropertiesContainer : MonoBehaviour, PNCPropertyInterface
{
    public LocalProperty[] local_properties = new LocalProperty[0];
    public GlobalProperty[] global_properties = new GlobalProperty[0];

    public LocalProperty[] LocalProperties { get { return local_properties; } set { } }
    public GlobalProperty[] GlobalProperties { get { return global_properties; } set { } }

    public void SetLocalProperty(Interaction interact, LocalProperty property)
    {
        CommandSetLocalProperty command = new CommandSetLocalProperty();
        command.Queue(property, interact);
    }

    internal void SetGlobalProperty(Interaction interaction, GlobalProperty property)
    {
        CommandSetGlobalProperty command = new CommandSetGlobalProperty();
        command.Queue(property, interaction);
    }

}
