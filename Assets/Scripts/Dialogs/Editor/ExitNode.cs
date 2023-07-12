using System;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class ExitNode : Node
{

    public ExitNode(ref Dialog dialog, int index, Vector2 position, float width, float height, GUIStyle nodeStyle, GUIStyle selectedStyle, GUIStyle inPointStyle, GUIStyle outPointStyle) : base(ref dialog, index, position, width, height, nodeStyle, selectedStyle, inPointStyle, outPointStyle)
    {

    }

    public void SetOnClick(Action<ConnectionPoint, Node> OnClickInPoint, Action<Node> OnChangeRect)
    {
        base.SetOnClick(OnClickInPoint, null, OnChangeRect);
    }

    public new void Draw()
    {
        base.Draw();
        GUIStyle style = new GUIStyle();
        style.fontSize = 15;
        style.normal.textColor = Color.white;

        EditorGUI.LabelField(new Rect(rect.x + 20, rect.y + 15, rect.width, rect.height), "EXIT",style);
    }

    public new void Drag(Vector2 delta)
    {
        base.Drag(delta);
        dialog.ChangeExitRect(rect);
        EditorUtility.SetDirty(dialog);
    }
       
    
    public new void Zoom(float delta)
    {
        base.Zoom(delta);
        dialog.ChangeExitRect(rect);
    }
}
