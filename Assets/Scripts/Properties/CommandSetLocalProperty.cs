using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class CommandSetLocalProperty : CommandSetGenericProperty, ICommand
{

    public void Queue(LocalProperty property,Interaction inter)
    {
        base.Queue((GenericProperty)property, inter);
    }

    

}
