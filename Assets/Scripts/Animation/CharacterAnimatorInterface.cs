using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface CharacterAnimatorInterface 
{
    // Start is called before the first frame update
    void SetTalking(bool talking);

    // Update is called once per frame
    void SetWalking(bool walking);

    void SetAngle(int angle);
}
