using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneTransitionAnimation : MonoBehaviour
{
    // Start is called before the first frame update
    Animator animator;
    void Start()
    {
        animator.GetComponent<Animator>();
    }

    public void In() 
    {
        animator.Play("in");
    }

    public void Out() 
    {
        animator.Play("out");
    }


}
