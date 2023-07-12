using System;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class SubDialogNode : Node
{

    public Action<SubDialogNode> OnRemoveNode;

    public SubDialogNode(ref Dialog dialog, int index, Vector2 position, float width, float height, GUIStyle nodeStyle, GUIStyle selectedStyle, GUIStyle inPointStyle, GUIStyle outPointStyle) : base(ref dialog, index, position, width, height, nodeStyle, selectedStyle, inPointStyle, outPointStyle)
    {

    }

    public void SetOnClick(Action<ConnectionPoint, Node> OnClickInPoint, Action<ConnectionPoint, Node> OnClickOutPoint, Action<SubDialogNode> OnClickRemoveNode, Action<Node> OnChangeRect)
    {
        base.SetOnClick(OnClickInPoint, OnClickOutPoint, OnChangeRect);
        OnRemoveNode = OnClickRemoveNode;
    }


    public new void Drag(Vector2 delta)
    {
        base.Drag(delta);
        dialog.ChangeSubDialogRect(subDialogIndex, rect);
    }

    public new void Draw()
    {
        base.Draw();

        for (int i = 0; i < outPoint.Count; i++)
        {
            dialog.GetSubDialogByIndex(subDialogIndex).options[i].text = GUI.TextField(new Rect(rect.x + rect.width * 0.06f, rect.y + (i) * 30 + 50, rect.width * 0.9f, EditorGUIUtility.singleLineHeight), dialog.GetSubDialogByIndex(subDialogIndex).options[i].text);
        }

        dialog.GetSubDialogByIndex(subDialogIndex).text = GUI.TextField(new Rect(rect.x + rect.width * 0.06f, rect.y + rect.width * 0.06f, rect.width *0.9f,EditorGUIUtility.singleLineHeight), dialog.GetSubDialogByIndex(subDialogIndex).text);
    }

    public new bool ProcessEvents(Event e)
    {
        base.ProcessEvents(e);

        switch (e.type)
        {
            case EventType.MouseDown:
                if (e.button == 1 && isSelected && rect.Contains(e.mousePosition))
                {
                    ProcessContextMenu();
                    e.Use();
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

    public new void Zoom(float delta) 
    {
        base.Zoom(delta);
        dialog.ChangeSubDialogRect(subDialogIndex, rect);
    }
}
