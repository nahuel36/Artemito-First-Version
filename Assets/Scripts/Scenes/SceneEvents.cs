using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SceneEventInteraction
{
    public enum SceneEvent
    {
        afterFadeIn,
        beforeFadeIn
    }

    public SceneEvent sceneEvent;

    public AttempsContainer attempsContainer = new AttempsContainer();
}


[System.Serializable]
public class SceneEvents: MonoBehaviour
{
    public List<SceneEventInteraction> events = new List<SceneEventInteraction>();
}
