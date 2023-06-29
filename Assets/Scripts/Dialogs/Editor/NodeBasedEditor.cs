using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class NodeBasedEditor : EditorWindow
{

    private GUIStyle nodeStyle;
    private GUIStyle selectedNodeStyle;
    private GUIStyle inPointStyle;
    private GUIStyle outPointStyle;

    private ConnectionPoint selectedInPoint;
    private Node selectedInPointNode;
    private ConnectionPoint selectedOutPoint;
    private Node selectedOutPointNode;

    private Vector2 offset;
    private Vector2 drag;

    private Dialog dialog;
    public void OpenWindow(Dialog dialogparam)
    {
        NodeBasedEditor window = GetWindow<NodeBasedEditor>();
        window.titleContent = new GUIContent("Node Based Editor");
        dialog = dialogparam;

        if (dialog.nodes != null)
            for (int i = 0; i < dialog.nodes.Count; i++)
            {
                dialog.nodes[i].SetOnClick(OnClickInPoint, OnClickOutPoint, OnClickRemoveNode);
            }
        for (int i = 0; i < dialog.connections.Count; i++)
        {
            for (int j = 0; j < dialog.nodes.Count; j++)
            {
                if (dialog.connections[i].nodeIn.index == dialog.nodes[j].index)
                {
                    dialog.connections[i].nodeIn = dialog.nodes[j];
                }
                if (dialog.connections[i].nodeOut.index == dialog.nodes[j].index)
                {
                    dialog.connections[i].nodeOut = dialog.nodes[j];
                }
            }
            dialog.connections[i].SetOnclick(OnClickRemoveConnection);
        }
    }

    private void OnEnable()
    {
        nodeStyle = new GUIStyle();
        nodeStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node1.png") as Texture2D;
        nodeStyle.border = new RectOffset(12, 12, 12, 12);

        selectedNodeStyle = new GUIStyle();
        selectedNodeStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node1 on.png") as Texture2D;
        selectedNodeStyle.border = new RectOffset(12, 12, 12, 12);

        inPointStyle = new GUIStyle();
        inPointStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn left.png") as Texture2D;
        inPointStyle.active.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn left on.png") as Texture2D;
        inPointStyle.border = new RectOffset(4, 4, 12, 12);

        outPointStyle = new GUIStyle();
        outPointStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn right.png") as Texture2D;
        outPointStyle.active.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn right on.png") as Texture2D;
        outPointStyle.border = new RectOffset(4, 4, 12, 12);


    }

    private void OnGUI()
    {
        DrawGrid(20, 0.2f, Color.gray);
        DrawGrid(100, 0.4f, Color.gray);

        DrawNodes();
        DrawConnections();

        DrawConnectionLine(Event.current);

        ProcessNodeEvents(Event.current);
        ProcessEvents(Event.current);

        if (GUI.changed)
        { 
            EditorUtility.SetDirty(dialog);
            Repaint();
        }
    }

    private void DrawGrid(float gridSpacing, float gridOpacity, Color gridColor)
    {
        int widthDivs = Mathf.CeilToInt(position.width / gridSpacing);
        int heightDivs = Mathf.CeilToInt(position.height / gridSpacing);

        Handles.BeginGUI();
        Handles.color = new Color(gridColor.r, gridColor.g, gridColor.b, gridOpacity);

        offset += drag * 0.5f;
        Vector3 newOffset = new Vector3(offset.x % gridSpacing, offset.y % gridSpacing, 0);

        for (int i = 0; i < widthDivs; i++)
        {
            Handles.DrawLine(new Vector3(gridSpacing * i, -gridSpacing, 0) + newOffset, new Vector3(gridSpacing * i, position.height, 0f) + newOffset);
        }

        for (int j = 0; j < heightDivs; j++)
        {
            Handles.DrawLine(new Vector3(-gridSpacing, gridSpacing * j, 0) + newOffset, new Vector3(position.width, gridSpacing * j, 0f) + newOffset);
        }

        Handles.color = Color.white;
        Handles.EndGUI();
    }

    private void DrawNodes()
    {
        if (dialog.nodes != null)
        {
            for (int i = 0; i < dialog.nodes.Count; i++)
            {
                dialog.nodes[i].Draw();
            }
        }


    }

    private void DrawConnections()
    {
        if (dialog.connections != null)
        {
            for (int i = 0; i < dialog.connections.Count; i++)
            {
                dialog.connections[i].Draw();
            }
        }
    }

    private void ProcessEvents(Event e)
    {
        switch (e.type)
        {
            case EventType.MouseDown:
                if (e.button == 0)
                {
                    ClearConnectionSelection();
                }

                if (e.button == 1)
                {
                    ProcessContextMenu(e.mousePosition);
                }
                break;

            case EventType.MouseDrag:
                if (e.button == 0)
                {
                    OnDrag(e.delta);
                }
                break;

            case EventType.ScrollWheel:
                Zoom(e.delta.y);
                
                break;
        }
    }


    private void Zoom(float delta) 
    {
        if (dialog.nodes != null)
        {
            for (int i = 0; i < dialog.nodes.Count; i++)
            {
                dialog.nodes[i].Zoom(delta);
            }
        }

        GUI.changed = true;

    }

    private void OnDrag(Vector2 delta)
    {
        drag = delta;

        if (dialog.nodes != null)
        {
            for (int i = 0; i < dialog.nodes.Count; i++)
            {
                dialog.nodes[i].Drag(delta);
            }
        }

        GUI.changed = true;
    }


    private void DrawConnectionLine(Event e)
    {
        if (selectedInPoint != null && selectedOutPoint == null)
        {
            Handles.DrawBezier(
                selectedInPoint.rect.center,
                e.mousePosition,
                selectedInPoint.rect.center + Vector2.left * 50f,
                e.mousePosition - Vector2.left * 50f,
                Color.white,
                null,
                2f
            );

            GUI.changed = true;
        }

        if (selectedOutPoint != null && selectedInPoint == null)
        {
            Handles.DrawBezier(
                selectedOutPoint.rect.center,
                e.mousePosition,
                selectedOutPoint.rect.center - Vector2.left * 50f,
                e.mousePosition + Vector2.left * 50f,
                Color.white,
                null,
                2f
            );

            GUI.changed = true;
        }
    }

    private void ProcessNodeEvents(Event e)
    {
        if (dialog.nodes != null)
        {
            for (int i = dialog.nodes.Count - 1; i >= 0; i--)
            {
                bool guiChanged = dialog.nodes[i].ProcessEvents(e);

                if (guiChanged)
                {
                    GUI.changed = true;
                }
            }
        }
    }


    private void ProcessContextMenu(Vector2 mousePosition)
    {
        GenericMenu genericMenu = new GenericMenu();
        genericMenu.AddItem(new GUIContent("Add node"), false, () => OnClickAddNode(mousePosition));
        genericMenu.ShowAsContext();
    }

    private void OnClickAddNode(Vector2 mousePosition)
    {
        if (dialog.nodes == null)
        {
            dialog.nodes = new List<Node>();
        }

        dialog.nodes.Add(new Node(dialog.nodeIndex, mousePosition, 200, 50, nodeStyle, selectedNodeStyle, inPointStyle, outPointStyle));
        dialog.nodes[dialog.nodes.Count - 1].SetOnClick(OnClickInPoint, OnClickOutPoint, OnClickRemoveNode);
        dialog.nodeIndex++;
    }

    private void OnClickInPoint(ConnectionPoint inPoint, Node node)
    {
        selectedInPoint = inPoint;
        selectedInPointNode = node;

        if (selectedOutPoint != null)
        {
            if (selectedOutPointNode != selectedInPointNode)
            {
                CreateConnection();
                ClearConnectionSelection();
            }
            else
            {
                ClearConnectionSelection();
            }
        }
    }


    private void OnClickOutPoint(ConnectionPoint outPoint, Node node)
    {
        selectedOutPoint = outPoint;
        selectedOutPointNode = node;

        if (selectedInPoint != null)
        {
            if (selectedOutPointNode != selectedInPointNode)
            {
                CreateConnection();
                ClearConnectionSelection();
            }
            else
            {
                ClearConnectionSelection();
            }
        }
    }

    private void OnClickRemoveNode(Node node)
    {
        if (dialog.connections != null)
        {
            List<Connection> connectionsToRemove = new List<Connection>();

            for (int i = 0; i < dialog.connections.Count; i++)
            {
                if (dialog.connections[i].nodeIn.inPoint == node.inPoint || dialog.connections[i].nodeOut.outPoint == node.outPoint)
                {
                    connectionsToRemove.Add(dialog.connections[i]);
                }
            }

            for (int i = 0; i < connectionsToRemove.Count; i++)
            {
                dialog.connections.Remove(connectionsToRemove[i]);
            }

            connectionsToRemove = null;
        }

        dialog.nodes.Remove(node);
    }

    private void OnClickRemoveConnection(Connection connection)
    {
        dialog.connections.Remove(connection);
    }

    private void CreateConnection()
    {
        if (dialog.connections == null)
        {
            dialog.connections = new List<Connection>();
        }

        dialog.connections.Add(new Connection(selectedInPointNode, selectedOutPointNode));
        dialog.connections[dialog.connections.Count - 1].SetOnclick(OnClickRemoveConnection);
    }

    private void ClearConnectionSelection()
    {
        selectedInPoint = null;
        selectedOutPointNode = null;
        selectedOutPoint = null;
        selectedInPointNode = null;
    }
}
