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

    public void RunVerbInteraction(Verb verbToRunString)
    {
        VerbInteractions verbToRun = InteractionUtils.FindVerb(verbToRunString, verbs);

        if (verbToRun != null)
            InteractionUtils.RunAttempsInteraction(verbToRun.attempsContainer);
    }

    public Verb[] GetActiveVerbs()
    {
        List<Verb> activeVerbs = new List<Verb>();
        for (int i = 0; i < verbs.Count; i++)
        {
            if (verbs[i].use)
                activeVerbs.Add(verbs[i].verb);
        }
        return activeVerbs.ToArray();
    }

}
