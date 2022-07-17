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
    // Start is called before the first frame update
    void Start()
    {
        lastPos.x = transform.position.x;
        lastPos.y = transform.position.y;
        lastPos.z = transform.position.z;
        animator = GetComponent<Animator>();
    }
    
    // Update is called once per frame
    void FixedUpdate()
    {
        int angle = Mathf.RoundToInt(Vector2.SignedAngle(transform.position - lastPos, Vector2.right));
        animator.SetInteger("angle 0", angle);
        if(transform.position != lastPos)
        {
            animator.SetBool("walking", true);
        }
        else
            animator.SetBool("walking", false);

        if (counter < delay)
            counter++;
        else
        { 
            lastPos = transform.position;
            counter = 0;
        }
    }
}
