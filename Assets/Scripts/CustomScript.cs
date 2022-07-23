using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;


public abstract class CustomScript : MonoBehaviour
{
    public async Task Execute()
    {
        await Task.Yield();
        LoadScript();
    }
    
    public abstract void LoadScript();

    
}
