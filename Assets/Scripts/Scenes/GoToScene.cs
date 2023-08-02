using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoToScene : MonoBehaviour
{
    public string scenePath;
    public string entryPointName;
    public ScenePoint goToPoint;
    public string cursorName;

    public void Go()
    {
        foreach (PNCCharacter charact in FindObjectsOfType<PNCCharacter>())
        {
            if (charact.isPlayerCharacter)
            {
                charact.Walk(goToPoint.transform.position);
                charact.WalkStraight(goToPoint.transform.position);
            }
        }
        MultipleScenesManager.Instance.LoadZoneScene(scenePath, entryPointName);
    }
}
