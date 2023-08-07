using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInitializer : MonoBehaviour
{
    // Start is called before the first frame update
    public UnhandledEvents unhandledEvents;

    void Start()
    {
        DialogsManager.Instance.Initialize();
        InventoryManager.Instance.Initialize();

        unhandledEvents = Resources.Load<UnhandledEvents>("UnhandledEvents");
        for (int i = 0; i < unhandledEvents.verbs.Count; i++)
        {
            unhandledEvents.verbs[i].attempsContainer.executedTimes = 0;
        }
        for (int i = 0; i < unhandledEvents.inventoryActions.Count; i++)
        {
            unhandledEvents.inventoryActions[i].attempsContainer.executedTimes = 0;
        }
    }
}

