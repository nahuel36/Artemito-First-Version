using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class NodeBasedEditor : EditorWindow
{
    private GUIStyle entryNodeStyle;
    private GUIStyle exitNodeStyle;
    private GUIStyle nodeStyle;
    private GUIStyle selectedNodeStyle;
    private GUIStyle selectedEntryNodeStyle;
    private GUIStyle selectedExitNodeStyle;
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
    private List<SubDialogNode> subDialogNodes;
    private List<Connection> connections;

    private EntryNode entryNode;
    private ExitNode exitNode;
    public void OpenWindow(Dialog dialogparam, SerializedObject dialogSerializedParam)
    {

        NodeBasedEditor window = GetWindow<NodeBasedEditor>();
        window.titleContent = new GUIContent("Node Based Editor");
        dialog = dialogparam;
        dialogSerialized = dialogSerializedParam;

        InitializeNodes();

        InitializeConnections();


    }

    public void InitializeConnections() {
        connections = new List<Connection>();

        for (int i = 0; i < dialogSerialized.FindProperty("subDialogs").arraySize; i++)
        {
            for (int j = 0; j < dialogSerialized.FindProperty("subDialogs").GetArrayElementAtIndex(i).FindPropertyRelative("options").arraySize; j++)
            {
                int destiny = dialogSerialized.FindProperty("subDialogs").GetArrayElementAtIndex(i).FindPropertyRelative("options").GetArrayElementAtIndex(j).FindPropertyRelative("subDialogDestinyIndex").intValue;
                if (destiny > 0)
                {
                    connections.Add(new Connection(FindNodeBySubdialogIndex(destiny), FindNodeBySubdialogIndex(dialog.subDialogs[i].index), 0, j));
                    connections[connections.Count - 1].SetOnclick(OnClickRemoveConnection);
                }
                if (destiny == -2)
                {
                    connections.Add(new Connection(exitNode, FindNodeBySubdialogIndex(dialog.subDialogs[i].index), 0, j));
                    connections[connections.Count - 1].SetOnclick(OnClickRemoveConnection);
                }
            }
        }

        if (dialog.entryDialogIndex > 0)
        { 
            connections.Add(new Connection(FindNodeBySubdialogIndex(dialog.entryDialogIndex), entryNode));
            connections[connections.Count - 1].SetOnclick(OnClickRemoveConnection);
        }
    }


    public SubDialogNode FindNodeBySubdialogIndex(int index)
    {
        for (int i = 0; i < subDialogNodes.Count; i++)
        {
            if (subDialogNodes[i].subDialogIndex == index)
                return subDialogNodes[i];
        }
        return null;
    }

    public void InitializeNodes()
    {
        if (dialogSerialized.FindProperty("enterNodeRect").rectValue.width == 0)
        {
            dialogSerialized.FindProperty("enterNodeRect").rectValue = new Rect(120, 120, 200, 50);
        }
        Vector2 entryPos = new Vector2(dialogSerialized.FindProperty("enterNodeRect").rectValue.x, dialogSerialized.FindProperty("enterNodeRect").rectValue.y);
        float entryWidth = dialogSerialized.FindProperty("enterNodeRect").rectValue.width;
        float entryHeight = dialogSerialized.FindProperty("enterNodeRect").rectValue.height;
        entryNode = new EntryNode(ref dialog, 0, entryPos, entryWidth, entryHeight, entryNodeStyle, selectedEntryNodeStyle, inPointStyle, outPointStyle);
        entryNode.SetOnClick(OnClickOutPoint, OnChangeRect);

        if (dialogSerialized.FindProperty("exitNodeRect").rectValue.width == 0)
        {
            dialogSerialized.FindProperty("exitNodeRect").rectValue = new Rect(120, 160, 200, 50);
        }
        Vector2 exitPos = new Vector2(dialogSerialized.FindProperty("exitNodeRect").rectValue.x, dialogSerialized.FindProperty("exitNodeRect").rectValue.y);
        float exitWidth = dialogSerialized.FindProperty("exitNodeRect").rectValue.width;
        float exitHeight = dialogSerialized.FindProperty("exitNodeRect").rectValue.height;
        exitNode = new ExitNode(ref dialog, 0, exitPos, exitWidth, exitHeight, exitNodeStyle, selectedExitNodeStyle, inPointStyle, outPointStyle);
        exitNode.SetOnClick(OnClickInPoint, OnChangeRect);

        subDialogNodes = new List<SubDialogNode>();

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
                float height = 50;
                subDialogNodes.Add(new SubDialogNode(ref dialog, index, pos, width, height, nodeStyle, selectedNodeStyle, inPointStyle, outPointStyle));
                subDialogNodes[subDialogNodes.Count - 1].SetOnClick(OnClickInPoint, OnClickOutPoint, OnClickRemoveNode, OnChangeRect);
            }
        }
    }

    private void OnEnable()
    {
        entryNodeStyle = new GUIStyle();
        entryNodeStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node3.png") as Texture2D;
        entryNodeStyle.border = new RectOffset(12, 12, 12, 12);

        nodeStyle = new GUIStyle();
        nodeStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node1.png") as Texture2D;
        nodeStyle.border = new RectOffset(12, 12, 12, 12);

        exitNodeStyle = new GUIStyle();
        exitNodeStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node6.png") as Texture2D;
        exitNodeStyle.border = new RectOffset(12, 12, 12, 12);

        selectedEntryNodeStyle = new GUIStyle();
        selectedEntryNodeStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node3 on.png") as Texture2D;
        selectedEntryNodeStyle.border = new RectOffset(12, 12, 12, 12);

        selectedNodeStyle = new GUIStyle();
        selectedNodeStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node1 on.png") as Texture2D;
        selectedNodeStyle.border = new RectOffset(12, 12, 12, 12);

        selectedExitNodeStyle = new GUIStyle();
        selectedExitNodeStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node6 on.png") as Texture2D;
        selectedExitNodeStyle.border = new RectOffset(12, 12, 12, 12);

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
        if (subDialogNodes != null)
        {
            for (int i = 0; i < subDialogNodes.Count; i++)
            {
                subDialogNodes[i].Draw();
            }
        }

        entryNode.Draw();
        exitNode.Draw();
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
        if (subDialogNodes != null)
        {
            for (int i = 0; i < subDialogNodes.Count; i++)
            {
                subDialogNodes[i].Zoom(delta);
            }
        }

        entryNode.Zoom(delta);
        exitNode.Zoom(delta);

        GUI.changed = true;

    }

    private void OnDrag(Vector2 delta)
    {
        drag = delta;

        if (subDialogNodes != null)
        {
            for (int i = 0; i < subDialogNodes.Count; i++)
            {
                subDialogNodes[i].Drag(delta);
            }
        }

        entryNode.Drag(delta);
        dialog.ChangeEntryRect(entryNode.rect);
        exitNode.Drag(delta);
        dialog.ChangeExitRect(exitNode.rect);

        EditorUtility.SetDirty(dialog);

        GUI.changed = true;
    }

    private void OnChangeRect(Node node)
    {
        if (node is SubDialogNode)
        {
            dialog.ChangeSubDialogRect(node.subDialogIndex, node.rect);
        }

        if (node is EntryNode)
        {
            dialog.ChangeEntryRect(node.rect);
        }

        if (node is ExitNode)
        {
            dialog.ChangeExitRect(node.rect);
        }
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
        if (subDialogNodes != null)
        {
            for (int i = subDialogNodes.Count - 1; i >= 0; i--)
            {
                bool guiChanged = subDialogNodes[i].ProcessEvents(e) || entryNode.ProcessEvents(e) || exitNode.ProcessEvents(e);

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
        if (subDialogNodes == null)
        {
            subDialogNodes = new List<SubDialogNode>();
        }
        if (dialog.subDialogs == null)
        {
            dialog.subDialogs = new List<SubDialog>();
        }

        dialog.subDialogIndex++;
        subDialogNodes.Add(new SubDialogNode(ref dialog, dialog.subDialogIndex, mousePosition, 200, 50, nodeStyle, selectedNodeStyle, inPointStyle, outPointStyle));
        subDialogNodes[subDialogNodes.Count - 1].SetOnClick(OnClickInPoint, OnClickOutPoint, OnClickRemoveNode, OnChangeRect);
        dialog.subDialogs.Add(new SubDialog() { text = "new subdialog", index = subDialogNodes[subDialogNodes.Count - 1].subDialogIndex , nodeRect = subDialogNodes[subDialogNodes.Count - 1].rect});
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

    private void OnClickRemoveNode(SubDialogNode node)
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
                if (connectionsToRemove[i].nodeOut is EntryNode)
                    dialog.ChangeEntry(0);
                else if (connectionsToRemove[i].nodeIn is SubDialogNode && connectionsToRemove[i].nodeOut is SubDialogNode)
                    dialog.ChangeDestiny(connectionsToRemove[i].nodeOut.subDialogIndex, 0, connectionsToRemove[i].nodeOut.outPoint[connectionsToRemove[i].indexOut].optionSpecialIndex);
            }

            connectionsToRemove = null;
        }

        subDialogNodes.Remove(node);
        dialog.Remove(node.subDialogIndex);
    }

    private void OnClickRemoveConnection(Connection connection)
    {
        connections.Remove(connection);
        if (connection.nodeOut is EntryNode)
            dialog.ChangeEntry(0);
        else
            dialog.ChangeDestiny(connection.nodeOut.subDialogIndex, 0, connection.nodeOut.outPoint[connection.indexOut].optionSpecialIndex);
    }

    private void CreateConnection()
    {
        List<Connection> connectionsToRemove = new List<Connection>();

        for (int i = 0; i < connections.Count; i++)
        {
            if (selectedOutPointNode == connections[i].nodeOut)
            { 
                for (int j = 0; j < connections[i].nodeOut.outPoint.Count; j++)
                {
                    if (connections[i].indexOut == connections[i].nodeOut.outPoint[j].outArrayIndex &&
                        selectedOutPoint == connections[i].nodeOut.outPoint[j]) 
                    {
                        connectionsToRemove.Add(connections[i]);
                    }
                }
            }
        }
        for (int i = 0; i < connectionsToRemove.Count; i++)
        {
            connections.Remove(connectionsToRemove[i]);
        }


        if (selectedInPointNode is ExitNode)
        {
            dialog.ChangeDestiny(selectedOutPointNode.subDialogIndex,-2, selectedOutPoint.optionSpecialIndex);
        }
        else if (selectedOutPointNode is EntryNode)
        {
            dialog.ChangeEntry(selectedInPointNode.subDialogIndex);
        }
        else
        {
            dialog.ChangeDestiny(selectedOutPointNode.subDialogIndex, selectedInPointNode.subDialogIndex, selectedOutPoint.optionSpecialIndex);
        }

        if (connections == null)
        {
            connections = new List<Connection>();
        }

        connections.Add(new Connection(selectedInPointNode, selectedOutPointNode, 0, selectedOutPoint.outArrayIndex));
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
