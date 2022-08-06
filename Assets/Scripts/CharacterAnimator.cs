using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CharacterAnimator : MonoBehaviour
{

    Animator animator;
    Vector3 lastPos;
    float counter;
    [SerializeField] float delay;
    [SerializeField] PNCCharacter character;
    int angle;
    // Start is called before the first frame update
    void Start()
    {
        angle = 90;
        lastPos.x = transform.position.x;
        lastPos.y = transform.position.y;
        lastPos.z = transform.position.z;
        animator = GetComponent<Animator>();
    }
    
    // Update is called once per frame
    void FixedUpdate()
    {
        
        if(transform.position != lastPos)
        {
            angle = Mathf.RoundToInt(Vector2.SignedAngle(transform.position - lastPos, Vector2.right));
            animator.SetBool("walking", true);
        }
        else
            animator.SetBool("walking", false);
        animator.SetInteger("angle 0", angle);

        animator.SetBool("talking", character.isTalking());

        if (counter < delay)
            counter++;
        else
        { 
            lastPos = transform.position;
            counter = 0;
        }
    }
}
