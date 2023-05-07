using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointAndWalk : MonoBehaviour
{
    [SerializeField]CharacterAnimator characterAnimator;
    [SerializeField] PNCCharacter pNCCharacter;
    [SerializeField] Animator anim;

    private void Awake()
    {
        pNCCharacter.ConfigurePathFinder(1);
        pNCCharacter.ConfigureTalker();
        CharacterAnimatorAdapter characterAnimatorAdapter = new CharacterAnimatorAdapter();
        characterAnimatorAdapter.Configure(anim);
        characterAnimator.Configure(characterAnimatorAdapter, pNCCharacter);

    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
       if(Input.GetMouseButtonDown(0))
        {
            Vector3 point = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            pNCCharacter.CancelWalkAndTasks();
            pNCCharacter.SkipTalk();
            pNCCharacter.CancelableWalk(point.x * Vector3.right + point.y * Vector3.up);
            pNCCharacter.Talk("LLegué");

        }
       else if(Input.GetMouseButtonDown(1))
        {
            Vector3 point = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            pNCCharacter.BackgroundTalk("Estoy caminando");
            pNCCharacter.BackgroundTalk("Sigo caminando");
            pNCCharacter.Walk(point.x * Vector3.right + point.y * Vector3.up);
        }
    }
}
