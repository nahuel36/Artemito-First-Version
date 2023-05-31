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
    public InventoryItemAction[] inventoryActions;
    public bool expandedInInspector = false;
    public int specialIndex = -1;
}
