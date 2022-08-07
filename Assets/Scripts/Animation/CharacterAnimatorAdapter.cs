using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimatorAdapter : CharacterAnimatorInterface
{
    Animator animator;

    public void Configure(Animator anim)
    {
        animator = anim;
    }


    public void SetAngle(int angle)
    {
        animator.SetInteger("angle 0", angle);
    }

    public void SetTalking(bool talking)
    {
        animator.SetBool("talking", talking);
    }

    public void SetWalking(bool walking)
    {
        animator.SetBool("walking", walking);
    }


}
