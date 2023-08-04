using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
public class SceneTransitionAnimation : MonoBehaviour
{
    // Start is called before the first frame update
    Animator animator;
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public async Task In() 
    {
        animator.Play("in");
        await Task.Delay(1000);
        while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.99f)
        {
            await Task.Delay(Mathf.RoundToInt(Time.deltaTime * 1000));
        }
    }

    public async Task Out() 
    {
        animator.Play("out");
        await Task.Delay(1000);
        while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.99f)
        {
            await Task.Delay(Mathf.RoundToInt(Time.deltaTime * 1000));
        }
    }


}
