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

    public int optionSpecialIndex;

    public int optionArrayIndex;

    public ConnectionPoint(Node node, ConnectionPointType type, GUIStyle style, int optionSpecialIndex = -1, int index = -1)
    {
        this.type = type;
        this.style = style;
        
        rect = new Rect(0, 0, 10f, 20f);

        this.optionSpecialIndex = optionSpecialIndex;
        this.optionArrayIndex = index;
    }

    public void SetOnClick(Action<ConnectionPoint, Node> OnClickConnectionPoint)
    {
        this.OnClickConnectionPoint = OnClickConnectionPoint;
    }

    public void Draw(Node node)
    {
        switch (type)
        {
            case ConnectionPointType.In:
                rect.x = node.rect.x - rect.width + 8f;
                rect.y = node.rect.y + (node.rect.height * 0.5f) - rect.height * 0.5f;
                break;

            case ConnectionPointType.Out:
                rect.x = node.rect.x + node.rect.width - 8f;
                rect.y = node.rect.y + 50;
                rect.y += (optionArrayIndex) * 30;
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