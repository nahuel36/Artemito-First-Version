using System;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public class Connection
{
    public Action<Connection> OnClickRemoveConnection;
    public Node nodeIn;
    public Node nodeOut;
    public int index;
    public Connection(Node nodeIn, Node nodeOut, int index = 0)
    {
        this.nodeIn = nodeIn;
        this.nodeOut = nodeOut;
        this.index = index;
    }

    public void SetOnclick(Action<Connection> OnClickRemoveConnection)
    {
        this.OnClickRemoveConnection = OnClickRemoveConnection;
    }

    public void Draw()
    {
        Handles.DrawBezier(
            nodeIn.inPoint.rect.center,
            nodeOut.outPoint[index].rect.center,
            nodeIn.inPoint.rect.center + Vector2.left * 50f,
            nodeOut.outPoint[index].rect.center - Vector2.left * 50f,
            Color.white,
            null,
            2f
        );

        if (Handles.Button((nodeIn.inPoint.rect.center + nodeOut.outPoint[index].rect.center) * 0.5f, Quaternion.identity, 4, 8, Handles.RectangleHandleCap))
        {
            if (OnClickRemoveConnection != null)
            {
                OnClickRemoveConnection(this);
            }
        }
    }
}

