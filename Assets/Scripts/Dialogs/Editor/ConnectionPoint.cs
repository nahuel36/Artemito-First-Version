using System;
using UnityEngine;

public enum ConnectionPointType { In, Out }

[System.Serializable]
public class ConnectionPoint
{
    public Rect rect;

    public ConnectionPointType type;

    public GUIStyle style;

    public Action<ConnectionPoint, Node> OnClickConnectionPoint;

    public ConnectionPoint(Node node, ConnectionPointType type, GUIStyle style)
    {
        this.type = type;
        this.style = style;
        
        rect = new Rect(0, 0, 10f, 20f);
    }

    public void SetOnClick(Action<ConnectionPoint, Node> OnClickConnectionPoint)
    {
        this.OnClickConnectionPoint = OnClickConnectionPoint;
    }

    public void Draw(Node node)
    {
        rect.y = node.rect.y + (node.rect.height * 0.5f) - rect.height * 0.5f;

        switch (type)
        {
            case ConnectionPointType.In:
                rect.x = node.rect.x - rect.width + 8f;
                break;

            case ConnectionPointType.Out:
                rect.x = node.rect.x + node.rect.width - 8f;
                break;
        }

        if (GUI.Button(rect, "", style))
        {
            if (OnClickConnectionPoint != null)
            {
                OnClickConnectionPoint(this, node);
            }
        }
    }
}