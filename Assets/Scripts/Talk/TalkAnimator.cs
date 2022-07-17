using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TalkAnimator : MonoBehaviour
{
    [SerializeField] bool canWalkAndTalkAtSameTime = false;
    [SerializeField] Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (animator.GetBool("walking") && !canWalkAndTalkAtSameTime)
            return;


    }
}
