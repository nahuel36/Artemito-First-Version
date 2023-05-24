using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointAndWalk : MonoBehaviour
{
    [SerializeField] PNCCharacter pNCCharacter;


    public void WalkCancelable(){
        Vector3 point = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        pNCCharacter.BackgroundTalk("Estoy caminando");
        pNCCharacter.BackgroundTalk("Sigo caminando");
        pNCCharacter.Walk(point.x * Vector3.right + point.y * Vector3.up);

    }

    public void WalkUncancelable()
    {
        Vector3 point = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        pNCCharacter.CancelWalkAndTasks();
        pNCCharacter.SkipTalk();
        pNCCharacter.CancelableWalk(point.x * Vector3.right + point.y * Vector3.up);
        pNCCharacter.Talk("LLegué");
    }


}
