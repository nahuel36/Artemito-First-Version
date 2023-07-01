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
    private SerializedObject dialogSerialized;
    private List<Node> nodes;
    private List<Connection> connections;
    public void OpenWindow(Dialog dialogparam, SerializedObject dialogSerializedParam)
    {
        
        NodeBasedEditor window = GetWindow<NodeBasedEditor>();
        window.titleContent = new GUIContent("Node Based Editor");
        dialog = dialogparam;
        dialogSerialized = dialogSerializedParam;

        InitializeNodes();


        if(connections != null)
            for (int i = 0; i < connections.Count; i++)
            {
                for (int j = 0; j < nodes.Count; j++)
                {
                    if (connections[i].nodeIn.subDialogIndex == nodes[j].subDialogIndex)
                    {
                        connections[i].nodeIn = nodes[j];
                    }
                    if (connections[i].nodeOut.subDialogIndex == nodes[j].subDialogIndex)
                    {
                        connections[i].nodeOut = nodes[j];
                    }
                }
                connections[i].SetOnclick(OnClickRemoveConnection);
            }
    }

    public void InitializeNodes()
    {
        nodes = new List<Node>();

        if (dialogSerialized.FindProperty("subDialogs").arraySize > 0)
        {
            SerializedProperty subDialogs = dialogSerialized.FindProperty("subDialogs");
            for (int i = 0; i < subDialogs.arraySize; i++)
            {
                SerializedProperty subDialog = subDialogs.GetArrayElementAtIndex(i);
                if (subDialog.FindPropertyRelative("nodeRect").FindPropertyRelative("width").floatValue == 0)
                {
                    subDialog.FindPropertyRelative("nodeRect").rectValue = new Rect(20, 20, 200, 50);
                }
                int index = subDialog.FindPropertyRelative("index").intValue;
                Vector2 pos = new Vector2(subDialog.FindPropertyRelative("nodeRect").rectValue.x, subDialog.FindPropertyRelative("nodeRect").rectValue.y);
                float width = subDialog.FindPropertyRelative("nodeRect").rectValue.width;
                float height = subDialog.FindPropertyRelative("nodeRect").rectValue.height;
                nodes.Add(new Node(index, pos, width, height, nodeStyle, selectedNodeStyle, inPointStyle, outPointStyle));
                nodes[nodes.Count - 1].SetOnClick(OnClickInPoint, OnClickOutPoint, OnClickRemoveNode);
                nodes[nodes.Count - 1].dialog = dialog;
                nodes[nodes.Count - 1].text = subDialog.FindPropertyRelative("text").stringValue;
            }
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
            for (int i = 0; i < nodes.Count; i++)
            {
                dialog.ChangeText(nodes[i].subDialogIndex, nodes[i].text);
            }
            EditorUtility.SetDirty(dialog);
            Repaint();
        }
        for (int i = 0; i < nodes.Count; i++)
        {
            nodes[i].text = dialog.GetText(nodes[i].subDialogIndex);
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
        if (nodes != null)
        {
            for (int i = 0; i < nodes.Count; i++)
            {
                nodes[i].Draw();
            }
        }


    }

    private void DrawConnections()
    {
        if (connections != null)
        {
            for (int i = 0; i < connections.Count; i++)
            {
                connections[i].Draw();
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
        if (nodes != null)
        {
            for (int i = 0; i < nodes.Count; i++)
            {
                nodes[i].Zoom(delta);
                dialog.ChangeRect(nodes[i].subDialogIndex, nodes[i].rect);
            }
        }

        

        GUI.changed = true;

    }

    private void OnDrag(Vector2 delta)
    {
        drag = delta;

        if (nodes != null)
        {
            for (int i = 0; i < nodes.Count; i++)
            {
                nodes[i].Drag(delta);
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
        if (nodes != null)
        {
            for (int i = nodes.Count - 1; i >= 0; i--)
            {
                bool guiChanged = nodes[i].ProcessEvents(e);

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
        if (nodes == null)
        {
            nodes = new List<Node>();
        }

        dialog.subDialogIndex++;
        nodes.Add(new Node(dialog.subDialogIndex, mousePosition, 200, 50, nodeStyle, selectedNodeStyle, inPointStyle, outPointStyle));
        nodes[nodes.Count - 1].SetOnClick(OnClickInPoint, OnClickOutPoint, OnClickRemoveNode);
        dialog.subDialogs.Add(new SubDialog() { text = "new subdialog", index = nodes[nodes.Count - 1].subDialogIndex , nodeRect = nodes[nodes.Count - 1].rect});
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
        if (connections != null)
        {
            List<Connection> connectionsToRemove = new List<Connection>();

            for (int i = 0; i < connections.Count; i++)
            {
                if (connections[i].nodeIn.inPoint == node.inPoint || connections[i].nodeOut.outPoint == node.outPoint)
                {
                    connectionsToRemove.Add(connections[i]);
                }
            }

            for (int i = 0; i < connectionsToRemove.Count; i++)
            {
                connections.Remove(connectionsToRemove[i]);
            }

            connectionsToRemove = null;
        }

        nodes.Remove(node);
        dialog.Remove(node.subDialogIndex);
    }

    private void OnClickRemoveConnection(Connection connection)
    {
        connections.Remove(connection);
    }

    private void CreateConnection()
    {
        if (connections == null)
        {
            connections = new List<Connection>();
        }

        connections.Add(new Connection(selectedInPointNode, selectedOutPointNode));
        connections[connections.Count - 1].SetOnclick(OnClickRemoveConnection);
    }

    private void ClearConnectionSelection()
    {
        selectedInPoint = null;
        selectedOutPointNode = null;
        selectedOutPoint = null;
        selectedInPointNode = null;
    }
}
