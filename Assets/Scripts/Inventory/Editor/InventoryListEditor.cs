using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(InventoryList))]
public class InventoryListEditor : Editor
{
    public int size = 100;
    int selectedButton = -1;
    public override void OnInspectorGUI()
    {

        InventoryList myTarget = (InventoryList)target;

        EditorGUILayout.LabelField(EditorGUIUtility.currentViewWidth.ToString());

        //EditorGUILayout.ObjectField(serializedObject.FindProperty("items").GetArrayElementAtIndex(0).FindPropertyRelative("normalImage"), typeof(Sprite), GUILayout.Height(size), GUILayout.Width(EditorGUIUtility.currentViewWidth * size));
        //EditorGUI.ObjectField(new Rect(0, 0, size*2, size*0.75f), serializedObject.FindProperty("items").GetArrayElementAtIndex(0).FindPropertyRelative("normalImage"));
        GUILayout.BeginHorizontal();
        //GUILayout.FlexibleSpace();
        int k = 0;
        List<bool> buttons = new List<bool>();
        for (int i = 0; i < myTarget.items.Length; i++)
        {
            k++;
            
            buttons.Add(GUILayout.Button(myTarget.items[i].normalImage.texture, GUILayout.MaxHeight(size), GUILayout.MaxWidth(size), GUILayout.MinHeight(size), GUILayout.MinWidth(size)));
            if ((k +1.46f)* (size*0.99f) > EditorGUIUtility.currentViewWidth)
            {
                k = 0;
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
            }
        }
        GUILayout.EndHorizontal();
        //GUILayout.Box(myTarget.items[i].normalImage.texture, GUILayout.MaxHeight(size), GUILayout.MaxWidth(size), GUILayout.MinHeight(size), GUILayout.MinWidth(size));
        //EditorGUILayout.ObjectField(serializedObject.FindProperty("items").GetArrayElementAtIndex(0).FindPropertyRelative("normalImage"), typeof(Sprite),GUILayout.Height(size*0.75f), GUILayout.Width(size*2));
        //EditorGUILayout.ObjectField(serializedObject.FindProperty("items").GetArrayElementAtIndex(1).FindPropertyRelative("normalImage"), typeof(Sprite), GUILayout.Height(size * 0.75f), GUILayout.Width(size * 2));
        //GUILayout.FlexibleSpace();

        for (int i = 0; i < buttons.Count; i++)
        {
            if (buttons[i] == true)
            {
                selectedButton = i;
            }
        }

        if (selectedButton != -1)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Box(myTarget.items[selectedButton].normalImage.texture, GUILayout.MaxHeight(size), GUILayout.MaxWidth(size), GUILayout.MinHeight(size), GUILayout.MinWidth(size));
            GUILayout.Box(myTarget.items[selectedButton].selectedImage.texture, GUILayout.MaxHeight(size), GUILayout.MaxWidth(size), GUILayout.MinHeight(size), GUILayout.MinWidth(size));
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("items").GetArrayElementAtIndex(selectedButton).FindPropertyRelative("itemName"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("items").GetArrayElementAtIndex(selectedButton).FindPropertyRelative("normalImage"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("items").GetArrayElementAtIndex(selectedButton).FindPropertyRelative("selectedImage"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("items").GetArrayElementAtIndex(selectedButton).FindPropertyRelative("haveItOnStart"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("items").GetArrayElementAtIndex(selectedButton).FindPropertyRelative("cuantity"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("items").GetArrayElementAtIndex(selectedButton).FindPropertyRelative("localVariables"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("items").GetArrayElementAtIndex(selectedButton).FindPropertyRelative("globalVariables"));
        }

        //base.OnInspectorGUI();

        serializedObject.ApplyModifiedProperties();

        if (GUI.changed)
        {
            EditorUtility.SetDirty(target);
        }



    }


}
