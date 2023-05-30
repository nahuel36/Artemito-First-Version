using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using UnityEditorInternal;

[CustomEditor(typeof(InventoryList))]
public class InventoryListEditor : Editor
{
    public int size = 100;
    int selectedButton = -1;

    Settings settings;
    Dictionary<string,ReorderableList> localVariablesLists = new Dictionary<string, ReorderableList>();

    Dictionary<string, ReorderableList> invAttempsListDict = new Dictionary<string, ReorderableList>();
    Dictionary<string, ReorderableList> invInteractionsListDict = new Dictionary<string, ReorderableList>();
    Dictionary<string, ReorderableList> invList = new Dictionary<string, ReorderableList>();

    private void OnEnable()
    {

        InventoryList myTarget = (InventoryList)target;
        for (int i = 0; i < myTarget.items.Length; i++)
        {
            PNCEditorUtils.InitializeGlobalVariables(GlobalVariableProperty.object_types.inventory,ref myTarget.items[i].global_variables);
            string key = serializedObject.FindProperty("items").GetArrayElementAtIndex(i).propertyPath;

            localVariablesLists.Add(key,null);
            ReorderableList list = localVariablesLists[key];
            PNCEditorUtils.InitializeLocalVariables(out list, serializedObject.FindProperty("items").GetArrayElementAtIndex(i).serializedObject, serializedObject.FindProperty("items").GetArrayElementAtIndex(i).FindPropertyRelative("local_variables"));
            localVariablesLists[key] = list;
            
            invList.Add(key, null);
            ReorderableList listInv = invList[key]; 
            InitializeInventoryInteractions(out listInv, serializedObject.FindProperty("items").GetArrayElementAtIndex(i).serializedObject, serializedObject.FindProperty("items").GetArrayElementAtIndex(i).FindPropertyRelative("inventoryActions"), myTarget.items[i]);
            invList[key] = listInv;
        }

    }

    public override void OnInspectorGUI()
    {

        InventoryList myTarget = (InventoryList)target;

        //EditorGUILayout.LabelField(EditorGUIUtility.currentViewWidth.ToString());

        //EditorGUILayout.ObjectField(serializedObject.FindProperty("items").GetArrayElementAtIndex(0).FindPropertyRelative("normalImage"), typeof(Sprite), GUILayout.Height(size), GUILayout.Width(EditorGUIUtility.currentViewWidth * size));
        //EditorGUI.ObjectField(new Rect(0, 0, size*2, size*0.75f), serializedObject.FindProperty("items").GetArrayElementAtIndex(0).FindPropertyRelative("normalImage"));
        GUILayout.BeginHorizontal();
        //GUILayout.FlexibleSpace();
        int k = 0;
        List<bool> buttons = new List<bool>();
        for (int i = 0; i < myTarget.items.Length; i++)
        {
            k++;
            GUIContent content = new GUIContent();
            if(myTarget.items[i].normalImage != null)
                content.image = myTarget.items[i].normalImage.texture;
            else
                content.text = myTarget.items[i].itemName;

            content.tooltip = myTarget.items[i].itemName;
            buttons.Add(GUILayout.Button(content, GUILayout.MaxHeight(size), GUILayout.MaxWidth(size), GUILayout.MinHeight(size), GUILayout.MinWidth(size)));
            
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

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("+", GUILayout.MaxHeight(25), GUILayout.MinHeight(25), GUILayout.MaxWidth(25), GUILayout.MinWidth(25)))
        {
            serializedObject.FindProperty("items").arraySize++;
        }
        if(GUILayout.Button("-", GUILayout.MaxHeight(25), GUILayout.MinHeight(25), GUILayout.MaxWidth(25), GUILayout.MinWidth(25)))
        {
            if (selectedButton != -1)
            { 
                serializedObject.FindProperty("items").DeleteArrayElementAtIndex(selectedButton);
                selectedButton = -1;
            
            }
        }
        GUILayout.EndHorizontal();

        for (int i = 0; i < buttons.Count; i++)
        {
            if (buttons[i] == true)
            {
                selectedButton = i;
                GUI.FocusControl(null);
            }
        }

        if (selectedButton != -1)
        {
            EditorGUILayout.BeginHorizontal();
            if(myTarget.items[selectedButton].normalImage)
                GUILayout.Box(myTarget.items[selectedButton].normalImage.texture, GUILayout.MaxHeight(size), GUILayout.MaxWidth(size), GUILayout.MinHeight(size), GUILayout.MinWidth(size));
            if(myTarget.items[selectedButton].selectedImage)
                GUILayout.Box(myTarget.items[selectedButton].selectedImage.texture, GUILayout.MaxHeight(size), GUILayout.MaxWidth(size), GUILayout.MinHeight(size), GUILayout.MinWidth(size));
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("items").GetArrayElementAtIndex(selectedButton).FindPropertyRelative("itemName"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("items").GetArrayElementAtIndex(selectedButton).FindPropertyRelative("normalImage"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("items").GetArrayElementAtIndex(selectedButton).FindPropertyRelative("selectedImage"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("items").GetArrayElementAtIndex(selectedButton).FindPropertyRelative("startWithThisItem"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("items").GetArrayElementAtIndex(selectedButton).FindPropertyRelative("cuantity"));

            SerializedProperty local_variables_serialized = serializedObject.FindProperty("items").GetArrayElementAtIndex(selectedButton).FindPropertyRelative("local_variables");
            SerializedProperty global_variables_serialized = serializedObject.FindProperty("items").GetArrayElementAtIndex(selectedButton).FindPropertyRelative("global_variables");

            localVariablesLists[serializedObject.FindProperty("items").GetArrayElementAtIndex(selectedButton).propertyPath].DoLayoutList();

            PNCEditorUtils.VerificateLocalVariables(ref myTarget.items[selectedButton].local_variables, ref local_variables_serialized);
            
            PNCEditorUtils.ShowGlobalVariables(GlobalVariableProperty.object_types.inventory, ref myTarget.items[selectedButton].global_variables, ref global_variables_serialized);

            invList[serializedObject.FindProperty("items").GetArrayElementAtIndex(selectedButton).propertyPath].DoLayoutList();
        }

        //base.OnInspectorGUI();

        serializedObject.ApplyModifiedProperties();

        if (GUI.changed)
        {
            EditorUtility.SetDirty(target);
        }



    }


    protected void InitializeInventoryInteractions(out ReorderableList inventoryList, SerializedObject serializedInventory, SerializedProperty inventoryProperty, InventoryItem myTarget)
    {
        settings = Resources.Load<Settings>("Settings/Settings");

        InventoryList inventory = Resources.Load<InventoryList>("Inventory");

        inventoryList = new ReorderableList(serializedInventory, inventoryProperty, true, true, true, true)
        {
            drawHeaderCallback = (rect) =>
            {
                EditorGUI.LabelField(rect, "inventory actions");
            },
            elementHeightCallback = (int indexInv) =>
            {
                return PNCEditorUtils.GetAttempsContainerHeight(inventoryProperty, myTarget.inventoryActions[indexInv].attemps, indexInv);
            },
            drawElementCallback = (rect, indexInv, active, focus) =>
            {
                string[] content = new string[inventory.items.Length];
                for (int i = 0; i < inventory.items.Length; i++)
                {
                    content[i] = inventory.items[i].itemName;
                }
                int selected = 0;
                if (myTarget.inventoryActions[indexInv].item != null)
                {
                    for (int i = 0; i < inventory.items.Length; i++)
                    {
                        if (inventory.items[i].Equals(myTarget.inventoryActions[indexInv].item))
                            selected = i;
                    }
                }
                rect.height = EditorGUIUtility.singleLineHeight;

                selected = EditorGUI.Popup(rect, "item", selected, content);

                myTarget.inventoryActions[indexInv].item = inventory.items[selected];


                PNCEditorUtils.DrawElementAttempContainer(inventoryProperty, indexInv, rect, invAttempsListDict, invInteractionsListDict, myTarget.inventoryActions[indexInv].attemps);
            }
        };

    }


}
