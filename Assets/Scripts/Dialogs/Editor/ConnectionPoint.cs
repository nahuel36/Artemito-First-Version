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

    public int inArrayIndex;

    public int outArrayIndex;

    public int optionSpecialIndex;

    public ConnectionPoint(Node node, ConnectionPointType type, GUIStyle style, int indexIn = 0, int indexOut = 0, int optionSpecialIndex = -1)
    {
        this.type = type;
        this.style = style;
        
        rect = new Rect(0, 0, 10f, 20f);

        this.inArrayIndex = indexIn;
        this.outArrayIndex = indexOut;
        this.optionSpecialIndex = optionSpecialIndex;
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
                rect.y += (inArrayIndex) * 30;
                break;

            case ConnectionPointType.Out:
                rect.x = node.rect.x + node.rect.width - 8f;
                if(optionSpecialIndex == -1)
                    rect.y = node.rect.y + (node.rect.height * 0.5f) - rect.height * 0.5f;
                else
                    rect.y = node.rect.y + 50;
                rect.y += (outArrayIndex) * 30;
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