using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Inventory", menuName = "Pnc/InventoryContainer", order = 1)]
public class InventoryList : ScriptableObject
{
    public InventoryItem[] items;

}
