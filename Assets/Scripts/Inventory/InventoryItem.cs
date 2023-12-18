using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class InventoryItem:PNCPropertyInterface
{
    public string itemName;
    public Sprite normalImage;
    public Sprite selectedImage;
    public bool startWithThisItem = false;
    public float cuantity = 1;
    public LocalProperty[] local_properties = new LocalProperty[0];
    public GlobalProperty[] global_properties;
    public List<InventoryItemAction> inventoryActions;
    public List<VerbInteractions> verbs;
    public bool expandedInInspector = false;
    public int specialIndex = -1;
    public int priority = 0;
    private Settings settings;

    public LocalProperty[] LocalProperties { get { return local_properties; } set { } }
    public GlobalProperty[] GlobalProperties { get { return global_properties; } set { } }


    public LocalProperty[] current_local_properties = new LocalProperty[0];
    public GlobalProperty[] current_global_properties;

    public GenericProperty[] GenericCurrentProperties(InteractionUtils.PropertyActionType action)
    {
        if (action == InteractionUtils.PropertyActionType.get_local_property)
            return current_local_properties;
        else if (action == InteractionUtils.PropertyActionType.get_global_property)
            return current_global_properties;
        else
            return null;
    }

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

    public void RunVerbInteraction(Verb verbToRunString)
    {
        VerbInteractions verbToRun = InteractionUtils.FindVerb(verbToRunString, verbs);

        if (verbToRun != null)
            InteractionUtils.RunAttempsInteraction(verbToRun.attempsContainer, InteractionObjectsType.verbInInventory, verbToRunString, new InventoryItem[] { this });
        else
            InteractionUtils.RunUnhandledEvents(InteractionObjectsType.verbInInventory, verbToRunString, new InventoryItem[] { this });
    }

    public Verb[] GetActiveVerbs()
    {
        if (settings == null)
            settings = Resources.Load<Settings>("Settings/Settings");

        List<Verb> activeVerbs = new List<Verb>();
        for (int i = 0; i < settings.verbs.Length; i++)
        {
            bool founded = false;
            for (int j = 0; j < verbs.Count; j++)
            {
                if (settings.verbs[i].index == verbs[j].verb.index)
                {
                    if (verbs[j].use || settings.alwaysShowAllVerbs)
                    {
                        activeVerbs.Add(verbs[i].verb);
                        founded = true;
                    }
                }
            }
            if (!founded && settings.alwaysShowAllVerbs)
                activeVerbs.Add(settings.verbs[i]);
        }
        return activeVerbs.ToArray();
    }

}
