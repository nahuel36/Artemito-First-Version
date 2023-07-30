using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointAndWalk : MonoBehaviour
{
    [SerializeField] PNCCharacter pNCCharacter;

    public void Start()
    {
        PNCCharacter[] characters = FindObjectsOfType<PNCCharacter>();
        foreach (PNCCharacter character in characters)
        {
            StartCoroutine(character.Initialize());
        }
        
    }

    public void WalkUnCancelable(){
        Vector3 point = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        pNCCharacter.Walk(point.x * Vector3.right + point.y * Vector3.up);

    }

    public void WalkCancelable()
    {
        Vector3 point = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        pNCCharacter.SkipWalk();
        pNCCharacter.CancelableWalk(point.x * Vector3.right + point.y * Vector3.up);
    }


}
