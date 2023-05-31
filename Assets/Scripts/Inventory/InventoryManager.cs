using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public delegate void InvManagerFunction(InventoryItem item);
    public static event InvManagerFunction OnAddItem;

    InventoryList inventory;

    private static InventoryManager instance;
    private List<int> activeItems = new List<int>();

    public static InventoryManager Instance
    {
        get 
        { 
            if (instance == null)
            {
                instance = new GameObject("Inventory Manager").AddComponent<InventoryManager>();
            }
            return instance;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        inventory = Resources.Load<InventoryList>("Inventory");
        for (int i = 0; i < inventory.items.Length; i++)
        {
            if (inventory.items[i].startWithThisItem)
            {
                AddItem(inventory.items[i]);
            }
        }
    }

    public void Initialize()
    { 
    
    }

    public void AddItem(InventoryItem item)
    {
        int index = -1;
        for (int i = 0; i < inventory.items.Length; i++)
        {
            if (item.Equals(inventory.items[i]))
            {
                index = i;
            }
        }
        if (index != -1)
        { 
            activeItems.Add(index);
            OnAddItem(item);
        }
    }

    public InventoryItem GetItemAtIndex(int index)
    {
        if (index < activeItems.Count && index >= 0)
        {
            return inventory.items[activeItems[index]];
        }
        return null;
    }
}
