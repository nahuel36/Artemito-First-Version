using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

public class Node 
{
    public Rect rect;

    public bool isDragged;
    public bool isSelected;

    public List<ConnectionPoint> inPoint;
    public List<ConnectionPoint> outPoint;

    public Dialog dialog;
    public int subDialogIndex;

    public GUIStyle style;
    public GUIStyle defaultNodeStyle;
    public GUIStyle selectedNodeStyle;

    public Action<ConnectionPoint, Node> OnClickIn;
    public Action<ConnectionPoint, Node> OnClickOut;
    public Action<Node> OnChangeRect;
    public Node(ref Dialog dialog, int index, Vector2 position, float width, float height, GUIStyle nodeStyle, GUIStyle selectedStyle, GUIStyle inPointStyle, GUIStyle outPointStyle, bool isEntry = false)
    {
        this.dialog = dialog;
        outPoint = new List<ConnectionPoint>();
        for (int i = 0; i < dialog.GetOptionsCuantity(index); i++)
        {
            outPoint.Add(new ConnectionPoint(this, ConnectionPointType.Out, outPointStyle, 0, i, dialog.GetOptionSpecialIndex(index, i)));
        }
        rect = new Rect(position.x, position.y, width, height + dialog.GetOptionsCuantity(index) * height * 0.75f);
        style = nodeStyle;
        inPoint = new List<ConnectionPoint>();

        if (!isEntry)
        {
            inPoint.Add(new ConnectionPoint(this, ConnectionPointType.In, inPointStyle));
        }
        else
        {
            outPoint.Add(new ConnectionPoint(this, ConnectionPointType.Out, outPointStyle));

        }
        defaultNodeStyle = nodeStyle;
        selectedNodeStyle = selectedStyle;
        this.subDialogIndex = index;
    }


    public void SetOnClick(Action<ConnectionPoint, Node> OnClickInPoint, Action<ConnectionPoint, Node> OnClickOutPoint, Action<Node> OnChangeRect)
    {
        OnClickIn = OnClickInPoint;
        OnClickOut = OnClickOutPoint;
        for (int i = 0; i < inPoint.Count; i++)
        {
            inPoint[i].SetOnClick(OnClickIn);
        }
        for (int i = 0; i < outPoint.Count; i++)
        {
            outPoint[i].SetOnClick(OnClickOut);
        }
        this.OnChangeRect = OnChangeRect;
    }

    public void Drag(Vector2 delta)
    {
        rect.position += delta;
        OnChangeRect(this);
    }

    public void Draw()
    {
        GUI.Box(rect, "", style);

        for (int i = 0; i < outPoint.Count; i++)
        {
            outPoint[i].Draw(this);
        }

        for (int i = 0; i < inPoint.Count; i++)
        {
            inPoint[i].Draw(this);
        }
    }

    public bool ProcessEvents(Event e)
    {
        switch (e.type)
        {
            case EventType.MouseDown:
                if (e.button == 0)
                {
                    if (rect.Contains(e.mousePosition))
                    {
                        isDragged = true;
                        GUI.changed = true;
                        isSelected = true;
                        style = selectedNodeStyle;
                    }
                    else
                    {
                        GUI.changed = true;
                        isSelected = false;
                        style = defaultNodeStyle;
                    }
                }
                break;

            case EventType.MouseUp:
                isDragged = false;
                break;

            case EventType.MouseDrag:
                if (e.button == 0 && isDragged)
                {
                    Drag(e.delta);
                    e.Use();
                    return true;
                }
                break;
        }

        return false;
    }

    public void Zoom(float delta)
    {
        rect = new Rect(rect.x + delta * rect.x * 0.015f, rect.y - delta, rect.width + delta * 4, rect.height + delta);
    }
}
