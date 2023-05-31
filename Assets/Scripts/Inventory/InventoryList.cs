using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Inventory", menuName = "Pnc/InventoryList", order = 1)]
public class InventoryList : ScriptableObject
{
    public InventoryItem[] items = new InventoryItem[0];
    public int specialIndex = 0;
}
