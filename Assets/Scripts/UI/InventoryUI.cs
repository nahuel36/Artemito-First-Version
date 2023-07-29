using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventoryUI : MonoBehaviour
{
    public InventoryItem overInventory;
     PNCCursor cursor;
    UnityEngine.UI.GraphicRaycaster raycaster;
     EventSystem eventSystem;
    // Start is called before the first frame update
    void Start()
    {
        InventoryManager.OnAddItem += OnAddItem;

        raycaster = GetComponentInParent<UnityEngine.UI.GraphicRaycaster>();
        eventSystem = FindObjectOfType<EventSystem>();
        cursor = GameObject.FindObjectOfType<PNCCursor>();

    }

    void OnAddItem(InventoryItem item)
    {
         GameObject newGO = new GameObject("item " + item.itemName);
         newGO.transform.parent = transform;
         newGO.AddComponent<Image>().sprite = item.normalImage;
         newGO.transform.localScale = Vector3.one;
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
                    overInventory = InventoryManager.Instance.GetItemAtIndex(i);
                }
            }
        }
    }
}
