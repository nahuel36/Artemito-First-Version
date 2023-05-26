using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class InventoryItem
{
    public string itemName;
    public Sprite normalImage;
    public Sprite selectedImage;
    public bool haveItOnStart = false;
    public float cuantity = 1;
    public InteractuableLocalVariable[] local_variables;
    public InteractuableGlobalVariable[] global_variables;
}
