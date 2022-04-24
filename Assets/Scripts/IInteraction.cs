using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public interface IInteraction  
{
    // Start is called before the first frame update
    Task Execute();
}
