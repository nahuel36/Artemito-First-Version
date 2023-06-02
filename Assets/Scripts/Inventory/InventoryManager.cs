using System;
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
            for (int j = 0; j < inventory.items[i].inventoryActions.Count; j++)
            {
                InteractionUtils.InitializeInteractions(ref inventory.items[i].inventoryActions[j].attemps);

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

    public void RunInventoryInteraction(InventoryItem item1, InventoryItem item2)
    {
        int index1 = getInventoryActionsIndex(item1, item2.inventoryActions);
        int index2 = getInventoryActionsIndex(item2, item1.inventoryActions);
        int index = -1;
        InventoryItem itemWithAction = null;
        if (index1 != -1)
        {
            itemWithAction = item2;
            index = index1;
        }
        if (index2 != -1)
        {
            itemWithAction = item1;
            index = index2;
        }
        if (index != -1)
        {
           int times = itemWithAction.inventoryActions[index].executedTimes;
           for (int j = 0; j < itemWithAction.inventoryActions[index].attemps[times].interactions.Count; j++)
           {
                itemWithAction.inventoryActions[index].attemps[times].interactions[j].action.Invoke();
           }
           InteractionUtils.increaseExecutedTimes(ref itemWithAction.inventoryActions[index].executedTimes, itemWithAction.inventoryActions[index].attemps.Count, itemWithAction.inventoryActions[index].isCyclical);
        }
    }

    public int getInventoryActionsIndex(InventoryItem item, List<InventoryItemAction> inventoryActions)
    {
        for (int i = 0; i < inventoryActions.Count; i++)
        {
            if(item.specialIndex == inventoryActions[i].specialIndex)
                return i;
        }

        return -1;
    }
}
