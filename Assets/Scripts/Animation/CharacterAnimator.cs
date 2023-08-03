using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CharacterAnimator : MonoBehaviour
{

    CharacterAnimatorInterface animator;
    Vector3 lastPos;
    float counter;
    [SerializeField] float delay;
    PNCCharacter character;
    int angle;
    bool configured;
    // Start is called before the first frame update
    void Awake()
    {
        angle = 90;
        lastPos.x = transform.position.x;
        lastPos.y = transform.position.y;
        lastPos.z = transform.position.z;
    }

    public void Configure(CharacterAnimatorInterface anim, PNCCharacter charact)
    {
        angle = 90;
        lastPos.x = transform.position.x;
        lastPos.y = transform.position.y;
        lastPos.z = transform.position.z;

        delay = 0;
        animator = anim;
        character = charact;
        configured = true;
    }
    
    // Update is called once per frame
    void FixedUpdate()
    {
        if (!configured) return;

        if (transform.position != lastPos)
        {
            angle = Mathf.RoundToInt(Vector2.SignedAngle(transform.position - lastPos, Vector2.right));
            animator.SetWalking(true);
        }
        else
            animator.SetWalking(false);

        animator.SetAngle(angle);

        animator.SetTalking(character.isTalking());

        if (counter < delay)
            counter++;
        else
        { 
            lastPos = transform.position;
            counter = 0;
        }
    }
}
