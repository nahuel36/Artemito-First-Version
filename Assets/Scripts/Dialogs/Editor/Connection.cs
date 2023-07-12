using System;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public class Connection
{
    public Action<Connection> OnClickRemoveConnection;
    public Node nodeIn;
    public Node nodeOut;
    public int indexIn;
    public int indexOut;

    public Connection(Node nodeIn, Node nodeOut, int indexIn = 0, int indexOut = 0)
    {
        this.nodeIn = nodeIn;
        this.nodeOut = nodeOut;
        this.indexIn = indexIn;
        this.indexOut = indexOut;
    }

    public void SetOnclick(Action<Connection> OnClickRemoveConnection)
    {
        this.OnClickRemoveConnection = OnClickRemoveConnection;
    }

    public void Draw()
    {
        Handles.DrawBezier(
            nodeIn.inPoint[indexIn].rect.center,
            nodeOut.outPoint[indexOut].rect.center,
            nodeIn.inPoint[indexIn].rect.center + Vector2.left * 50f,
            nodeOut.outPoint[indexOut].rect.center - Vector2.left * 50f,
            new Color(UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f)),
            null,
            2.5f
        );

        if (Handles.Button((nodeIn.inPoint[indexIn].rect.center + nodeOut.outPoint[indexOut].rect.center) * 0.5f, Quaternion.identity, 4, 8, Handles.RectangleHandleCap))
        {
            if (OnClickRemoveConnection != null)
            {
                OnClickRemoveConnection(this);
            }
        }
    }
}

