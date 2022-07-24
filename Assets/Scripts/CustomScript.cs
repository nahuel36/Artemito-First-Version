using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;


public abstract class CustomScript : MonoBehaviour, IInteraction
{
    public async Task Execute()
    {
        await Task.Yield();
    }

    public virtual void LoadScript() {
        InteractionManager.Instance.AddCommand(this);
    }

    public void Skip()
    {

    }
}
