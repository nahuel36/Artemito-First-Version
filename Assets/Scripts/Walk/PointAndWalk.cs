using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointAndWalk : MonoBehaviour
{
    [SerializeField]CharacterAnimator characterAnimator;
    [SerializeField] PNCCharacter PNCCharacter;
    [SerializeField] Animator anim;

    private void Awake()
    {
        CharacterAnimatorAdapter characterAnimatorAdapter = new CharacterAnimatorAdapter();
        characterAnimatorAdapter.Configure(anim);
        characterAnimator.Configure(characterAnimatorAdapter, PNCCharacter);
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
            FindObjectOfType<PNCCharacter>().CancelWalk();
            FindObjectOfType<PNCCharacter>().SkipTalk();
            FindObjectOfType<PNCCharacter>().CancelableWalk(point.x * Vector3.right + point.y * Vector3.up);
            FindObjectOfType<PNCCharacter>().Talk("LLegué");

        }
       else if(Input.GetMouseButtonDown(1))
        {
            Vector3 point = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            FindObjectOfType<PNCCharacter>().BackgroundTalk("Estoy caminando");
            FindObjectOfType<PNCCharacter>().BackgroundTalk("Sigo caminando");
            FindObjectOfType<PNCCharacter>().Walk(point.x * Vector3.right + point.y * Vector3.up);
        }
    }
}
