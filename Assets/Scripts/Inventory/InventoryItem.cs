using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class InventoryItem
{
    public string itemName;
    public Sprite normalImage;
    public Sprite selectedImage;
    public bool startWithThisItem = false;
    public float cuantity = 1;
    public InteractuableLocalVariable[] local_variables = new InteractuableLocalVariable[0];
    public InteractuableGlobalVariable[] global_variables;
    public List<InventoryItemAction> inventoryActions;
    public List<VerbInteractions> verbs;
    public bool expandedInInspector = false;
    public int specialIndex = -1;
    private Settings settings;
    public void RunVerbInteraction(Verb verbToRunString)
    {
        VerbInteractions verbToRun = InteractionUtils.FindVerb(verbToRunString, verbs);

        if (verbToRun != null)
            InteractionUtils.RunAttempsInteraction(verbToRun.attempsContainer, InteractionObjectsType.verbInInventory, verbToRunString, new InventoryItem[] { this });
        else
            InteractionUtils.RunHunhandledEvents(InteractionObjectsType.verbInInventory, verbToRunString, new InventoryItem[] { this });
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
