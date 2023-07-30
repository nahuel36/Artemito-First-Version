using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "UnhandledEvents", menuName = "Pnc/UnhandledEvents", order = 1)]
public class UnhandledEvents : ScriptableObject
{
    public List<VerbInteractions> verbs;
    public List<InventoryItemAction> inventoryActions;

}
