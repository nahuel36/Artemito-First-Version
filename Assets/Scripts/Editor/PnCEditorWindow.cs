using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public class PnCEditorWindow : EditorWindow
{
    string text;
    UnityEngine.Events.UnityAction action;
    // Start is called before the first frame update
    public static void Init(string text_param, UnityEngine.Events.UnityAction action_param)
    {
        PnCEditorWindow window = (PnCEditorWindow)GetWindow(typeof(PnCEditorWindow));
        window.Show();
        window.text = text_param;
        window.action = action_param;
    }

    // Update is called once per frame
    private void OnGUI()
    {
        GUILayout.Label(text);
        if (GUILayout.Button("Ok"))
        {
            action.Invoke();
            PnCEditorWindow window = (PnCEditorWindow)GetWindow(typeof(PnCEditorWindow));
            window.Close();
        }
        if (GUILayout.Button("Cancel"))
        {
            PnCEditorWindow window = (PnCEditorWindow)GetWindow(typeof(PnCEditorWindow));
            window.Close();
        }    
    }
}
