using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class CommandSetGlobalProperty : CommandSetGenericProperty, ICommand
{

    public void Queue(GlobalProperty property, Interaction inter)
    {
        base.Queue((GenericProperty)property, inter);
    }

    


}
