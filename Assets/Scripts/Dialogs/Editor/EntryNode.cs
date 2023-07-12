using System;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class EntryNode : Node
{
    
    public EntryNode(ref Dialog dialog, int index, Vector2 position, float width, float height, GUIStyle nodeStyle, GUIStyle selectedStyle, GUIStyle inPointStyle,GUIStyle outPointStyle) : base (ref dialog, index, position, width, height, nodeStyle, selectedStyle, inPointStyle, outPointStyle, true)
    {
        
    }

    public void SetOnClick(Action<ConnectionPoint, Node> OnClickOutPoint, Action<Node> OnChangeRect)
    {
        base.SetOnClick(null, OnClickOutPoint, OnChangeRect);
    }

    public new void Draw()
    {
        base.Draw();
        GUIStyle style = new GUIStyle();
        style.fontSize = 15;
        style.normal.textColor = Color.white;

        EditorGUI.LabelField(new Rect(rect.x + 20, rect.y + 15, rect.width, rect.height), "ENTER", style);
    }

    public new void Drag(Vector2 delta)
    {
        base.Drag(delta);
        dialog.ChangeEntryRect(rect);
        EditorUtility.SetDirty(dialog);
    }
    
    public new void Zoom(float delta)
    {
        base.Zoom(delta);
        dialog.ChangeEntryRect(rect);
    }
}
