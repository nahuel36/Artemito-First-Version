using System;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class Node
{
    public Rect rect;
    public string title;
    public bool isDragged;
    public bool isSelected;

    public ConnectionPoint inPoint;
    public ConnectionPoint outPoint;

    public GUIStyle style;
    public GUIStyle defaultNodeStyle;
    public GUIStyle selectedNodeStyle;

    public Action<Node> OnRemoveNode;
    public Action<ConnectionPoint, Node> OnClickIn;
    public Action<ConnectionPoint, Node> OnClickOut;
    public string text;
    public int index;
    public List<DialogOption> options;
    public Node(int index, Vector2 position, float width, float height, GUIStyle nodeStyle, GUIStyle selectedStyle, GUIStyle inPointStyle, GUIStyle outPointStyle) 
    {
        rect = new Rect(position.x, position.y, width, height);
        style = nodeStyle;
        inPoint = new ConnectionPoint(this, ConnectionPointType.In, inPointStyle);
        outPoint = new ConnectionPoint(this, ConnectionPointType.Out, outPointStyle);
        defaultNodeStyle = nodeStyle;
        selectedNodeStyle = selectedStyle;
        text = "new subdialog";
        this.index = index;
    }

    public void SetOnClick(Action<ConnectionPoint, Node> OnClickInPoint, Action<ConnectionPoint, Node> OnClickOutPoint, Action<Node> OnClickRemoveNode)
    {
        OnClickIn = OnClickInPoint;
        OnClickOut = OnClickOutPoint;
        inPoint.SetOnClick(OnClickIn);
        outPoint.SetOnClick(OnClickOut);
        OnRemoveNode = OnClickRemoveNode;
    }


    public void Drag(Vector2 delta)
    {
        rect.position += delta;
    }

    public void Draw()
    {
        inPoint.Draw(this);
        outPoint.Draw(this);
        GUI.Box(rect, title, style);
        text = GUI.TextField(new Rect(rect.x + rect.width * 0.06f, rect.y + rect.width * 0.06f, rect.width *0.9f,EditorGUIUtility.singleLineHeight),text);
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

                if (e.button == 1 && isSelected && rect.Contains(e.mousePosition))
                {
                    ProcessContextMenu();
                    e.Use();
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

    private void ProcessContextMenu()
    {
        GenericMenu genericMenu = new GenericMenu();
        genericMenu.AddItem(new GUIContent("Remove node"), false, OnClickRemoveNode);
        genericMenu.ShowAsContext();
    }

    private void OnClickRemoveNode()
    {
        if (OnRemoveNode != null)
        {
            OnRemoveNode(this);
        }
    }

    public void Zoom(float delta)
    {
        rect = new Rect(rect.x + delta * rect.x * 0.015f, rect.y - delta, rect.width + delta * 4, rect.height + delta );
    }
}
