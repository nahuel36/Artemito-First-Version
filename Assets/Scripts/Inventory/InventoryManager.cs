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
        }

        
    }

    public void Initialize()
    {
        
    }

    public bool HaveInteraction(InventoryItem item, Verb verb)
    {
        for (int i = 0; i < item.verbs.Count; i++)
        {
            if (item.verbs[i].verb == verb)
                return true;
        }
        return false;
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

    
    public void RunInventoryInteraction(InventoryItem item1, InventoryItem item2, Verb verb)
    {
        int index1 = getInventoryActionsIndex(item1, item2.inventoryActions, verb);
        int index2 = getInventoryActionsIndex(item2, item1.inventoryActions, verb);
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
            InteractionUtils.RunAttempsInteraction(itemWithAction.inventoryActions[index].attempsContainer, InteractionObjectsType.inventoryIninventory, item1.namePrefix + item1.itemName + item1.namePostfix, verb.index, item2.specialIndex);
        }
        else
        {
            InteractionUtils.RunHunhandledEvents(InteractionObjectsType.inventoryIninventory, item1.namePrefix + item1.itemName + item1.namePostfix, verb.index, item2.specialIndex);
        }
    }

    public int getInventoryActionsIndex(InventoryItem item, List<InventoryItemAction> inventoryActions, Verb verb)
    {
        for (int i = 0; i < inventoryActions.Count; i++)
        {
            if(item.specialIndex == inventoryActions[i].specialIndex && inventoryActions[i].verb.index == verb.index)
                return i;
        }

        return -1;
    }

    public int getInventoryActionsIndex(PNCInteractuable item, List<InventoryItemAction> inventoryActions, Verb verb)
    {
        for (int i = 0; i < inventoryActions.Count; i++)
        {
            if (item == inventoryActions[i].sceneObject && inventoryActions[i].verb.index == verb.index)
                return i;
        }

        return -1;
    }

}
