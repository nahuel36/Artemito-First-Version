using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventoryUI : MonoBehaviour
{
    public InventoryItem overInventory;
    List<int> activeItems;
    PNCCursor cursor;
    UnityEngine.UI.GraphicRaycaster raycaster;
    InventoryList inventory;
    EventSystem eventSystem;
    // Start is called before the first frame update
    void Start()
    {
        activeItems = new List<int>();
        inventory = Resources.Load<InventoryList>("Inventory");
        for (int i = 0; i < inventory.items.Length; i++)
        {
            if (inventory.items[i].startWithThisItem)
            {
                GameObject newGO = new GameObject("item " + inventory.items[i].itemName);
                newGO.transform.parent = transform;
                newGO.AddComponent<Image>().sprite = inventory.items[i].normalImage;
                newGO.transform.localScale = Vector3.one;
                activeItems.Add(i);
            }
        }
        raycaster = GetComponentInParent<UnityEngine.UI.GraphicRaycaster>();
        eventSystem = FindObjectOfType<EventSystem>();
        cursor = GameObject.FindObjectOfType<PNCCursor>();

    }

    // Update is called once per frame
    void Update()
    {
        overInventory = null;

        PointerEventData pointerData = new PointerEventData(eventSystem);
        pointerData.position = cursor.GetPosition();

        List<RaycastResult> results = new List<RaycastResult>();
        raycaster.Raycast(pointerData, results);
        foreach (RaycastResult result in results)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                if (result.gameObject.transform == transform.GetChild(i))
                {
                    overInventory = inventory.items[activeItems[i]];
                }
            }
        }
    }
}
