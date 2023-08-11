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


    Dictionary<string, ReorderableList> verbAttempsListDict = new Dictionary<string, ReorderableList>();
    Dictionary<string, ReorderableList> verbInteractionsListDict = new Dictionary<string, ReorderableList>();
    Dictionary<string, ReorderableList> verbsList = new Dictionary<string, ReorderableList>();

    Dictionary<string, ReorderableList> customScriptInteractionDict = new Dictionary<string, ReorderableList>();
    private void OnEnable()
    {

        InventoryList myTarget = (InventoryList)target;
        for (int i = 0; i < myTarget.items.Length; i++)
        {
            PNCEditorUtils.InitializeGlobalVariables(GlobalVariableProperty.object_types.inventory,ref myTarget.items[i].global_variables);
            string key = serializedObject.FindProperty("items").GetArrayElementAtIndex(i).FindPropertyRelative("specialIndex").intValue.ToString();

            localVariablesLists.Add(key,null);
            ReorderableList list = localVariablesLists[key];
            PNCEditorUtils.InitializeLocalVariables(out list, serializedObject.FindProperty("items").GetArrayElementAtIndex(i).serializedObject, serializedObject.FindProperty("items").GetArrayElementAtIndex(i).FindPropertyRelative("local_variables"));
            localVariablesLists[key] = list;
            
            invList.Add(key, null);
            ReorderableList listInv = invList[key];
            InitializeInventoryInteractions(out listInv, serializedObject.FindProperty("items").GetArrayElementAtIndex(i).serializedObject, serializedObject.FindProperty("items").GetArrayElementAtIndex(i).FindPropertyRelative("inventoryActions"), myTarget.items[i]);
            invList[key] = listInv;

            verbsList.Add(key, null);
            ReorderableList listVerbs = verbsList[key];
            InitializeVerbs(out listVerbs, serializedObject.FindProperty("items").GetArrayElementAtIndex(i).serializedObject, serializedObject.FindProperty("items").GetArrayElementAtIndex(i).FindPropertyRelative("verbs"), myTarget.items[i]);
            verbsList[key] = listVerbs;
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

            serializedObject.ApplyModifiedProperties();

            serializedObject.FindProperty("specialIndex").intValue++;
            int index = serializedObject.FindProperty("items").arraySize - 1;
            serializedObject.FindProperty("items").GetArrayElementAtIndex(index).FindPropertyRelative("specialIndex").intValue = serializedObject.FindProperty("specialIndex").intValue;
            serializedObject.FindProperty("items").GetArrayElementAtIndex(index).FindPropertyRelative("itemName").stringValue = "new item " + serializedObject.FindProperty("specialIndex").intValue;
                                    
            string key = serializedObject.FindProperty("items").GetArrayElementAtIndex(index).FindPropertyRelative("specialIndex").intValue.ToString();

            if (!localVariablesLists.ContainsKey(key))
            { 
                localVariablesLists.Add(key, null);
                ReorderableList list = localVariablesLists[key];
                PNCEditorUtils.InitializeLocalVariables(out list, serializedObject.FindProperty("items").GetArrayElementAtIndex(index).serializedObject, serializedObject.FindProperty("items").GetArrayElementAtIndex(index).FindPropertyRelative("local_variables"));
                localVariablesLists[key] = list;
            }

            if (!invList.ContainsKey(key))
            {
                invList.Add(key, null);
                ReorderableList listInv = invList[key];
                InitializeInventoryInteractions(out listInv, serializedObject.FindProperty("items").GetArrayElementAtIndex(index).serializedObject, serializedObject.FindProperty("items").GetArrayElementAtIndex(index).FindPropertyRelative("inventoryActions"), myTarget.items[index]);
                invList[key] = listInv;
            }

            if (!verbsList.ContainsKey(key))
            {
                verbsList.Add(key, null);
                ReorderableList listVerbs = verbsList[key];
                InitializeVerbs(out listVerbs, serializedObject.FindProperty("items").GetArrayElementAtIndex(index).serializedObject, serializedObject.FindProperty("items").GetArrayElementAtIndex(index).FindPropertyRelative("verbs"), myTarget.items[index]);
                verbsList[key] = listVerbs;
            }
            serializedObject.ApplyModifiedProperties();

            selectedButton = buttons.Count;

            serializedObject.Update();
        }
        if (GUILayout.Button("-", GUILayout.MaxHeight(25), GUILayout.MinHeight(25), GUILayout.MaxWidth(25), GUILayout.MinWidth(25)))
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
            EditorGUILayout.PropertyField(serializedObject.FindProperty("items").GetArrayElementAtIndex(selectedButton).FindPropertyRelative("priority"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("items").GetArrayElementAtIndex(selectedButton).FindPropertyRelative("cuantity"));

            SerializedProperty local_variables_serialized = serializedObject.FindProperty("items").GetArrayElementAtIndex(selectedButton).FindPropertyRelative("local_variables");
            SerializedProperty global_variables_serialized = serializedObject.FindProperty("items").GetArrayElementAtIndex(selectedButton).FindPropertyRelative("global_variables");

            string key = serializedObject.FindProperty("items").GetArrayElementAtIndex(selectedButton).FindPropertyRelative("specialIndex").intValue.ToString();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUIStyle tittleStyle = new GUIStyle();
            tittleStyle.normal.textColor = Color.white;
            tittleStyle.fontSize = 14;
            GUILayout.Label("<b>Inventory Interactions</b>", tittleStyle);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            invList[key].DoLayoutList();

            PNCEditorUtils.ShowLocalVariables(localVariablesLists[key], ref myTarget.items[selectedButton].local_variables, ref local_variables_serialized);

            PNCEditorUtils.ShowGlobalVariables(GlobalVariableProperty.object_types.inventory, ref myTarget.items[selectedButton].global_variables, ref global_variables_serialized);

            ShowInteractionVerbs(key);
        }

        //base.OnInspectorGUI();

        serializedObject.ApplyModifiedProperties();

        if (GUI.changed)
        {
            EditorUtility.SetDirty(target);
        }



    }



    protected void ShowInteractionVerbs(string key)
    {
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUIStyle tittleStyle = new GUIStyle();
        tittleStyle.normal.textColor = Color.white;
        tittleStyle.fontSize = 14;
        GUILayout.Label("<b>Interactions</b>", tittleStyle);
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        if (GUILayout.Button("Edit verbs"))
        {
            Selection.objects = new UnityEngine.Object[] { settings };
            EditorGUIUtility.PingObject(settings);
        }
        verbsList[key].DoLayoutList();
    }


    protected void InitializeVerbs(out ReorderableList verbsList, SerializedObject serializedInventory, SerializedProperty inventoryProperty, InventoryItem myTarget)
    {
        settings = Resources.Load<Settings>("Settings/Settings");

        verbsList = new ReorderableList(serializedInventory, inventoryProperty, true, true, true, true)
        {
            drawHeaderCallback = (rect) =>
            {
                EditorGUI.LabelField(rect, "verbs");
            },
            elementHeightCallback = (int indexV) =>
            {
                return PNCEditorUtils.GetAttempsContainerHeight(inventoryProperty, indexV);
            },
            drawElementCallback = (rect, indexV, active, focus) =>
            {
                PNCEditorUtils.DrawArrayWithAttempContainer(inventoryProperty, indexV, rect, verbAttempsListDict, verbInteractionsListDict, customScriptInteractionDict, myTarget.verbs[indexV].attempsContainer.attemps, false);
            },
            onCanAddCallback = (list) =>
            {
                return myTarget.verbs.Count < settings.verbs.Length;
            },
            onAddDropdownCallback = (rect, list) =>
            {
                PNCEditorUtils.OnAddVerbDropdown(list, myTarget.verbs, serializedObject);
            }
        };


        PNCEditorUtils.CheckVerbs(ref myTarget.verbs);
    }


protected void InitializeInventoryInteractions(out ReorderableList inventoryList, SerializedObject serializedInventory, SerializedProperty inventoryProperty, InventoryItem myTarget)
    {
        settings = Resources.Load<Settings>("Settings/Settings");
       

        inventoryList = new ReorderableList(serializedInventory, inventoryProperty, true, true, true, true)
        {
            drawHeaderCallback = (rect) =>
            {
                EditorGUI.LabelField(rect, "inventory actions");
            },
            elementHeightCallback = (int indexInv) =>
            {
                return PNCEditorUtils.GetAttempsContainerHeight(inventoryProperty, indexInv);
            },
            drawElementCallback = (rect, indexInv, active, focus) =>
            {
                List<string> content = new List<string>();
                List<int> posibleItems = new List<int>();
                for (int i = 0; i < serializedObject.FindProperty("items").arraySize; i++)
                {
                    if (serializedObject.FindProperty("items").GetArrayElementAtIndex(i).FindPropertyRelative("specialIndex").intValue != myTarget.specialIndex)
                    { 
                        content.Add(serializedObject.FindProperty("items").GetArrayElementAtIndex(i).FindPropertyRelative("itemName").stringValue);
                        posibleItems.Add(serializedObject.FindProperty("items").GetArrayElementAtIndex(i).FindPropertyRelative("specialIndex").intValue);
                    }
                }
                int selected = 0;
                if (inventoryProperty.GetArrayElementAtIndex(indexInv).FindPropertyRelative("specialIndex").intValue != -1 && inventoryProperty.GetArrayElementAtIndex(indexInv).FindPropertyRelative("specialIndex").intValue != 0)
                {
                    for (int i = 0; i < posibleItems.Count; i++)
                    {
                        if (posibleItems[i] == inventoryProperty.GetArrayElementAtIndex(indexInv).FindPropertyRelative("specialIndex").intValue)
                            selected = i;
                    }
                }
                rect.height = EditorGUIUtility.singleLineHeight;

                selected = EditorGUI.Popup(new Rect(rect.x + rect.width / 2.25f, rect.y, rect.width / 2, rect.height), "",selected, content.ToArray());

                inventoryProperty.GetArrayElementAtIndex(indexInv).FindPropertyRelative("specialIndex").intValue = posibleItems[selected];



                List<string> verbsContent = new List<string>();
                for (int i = 0; i < settings.verbs.Length; i++)
                {
                    if (settings.verbs[i].isLikeGive || settings.verbs[i].isLikeUse)
                        verbsContent.Add(settings.verbs[i].name);
                }
                int verbSelected = 0;
                if (myTarget.inventoryActions[indexInv].verb.index >= 0)
                {
                    for (int i = 0; i < settings.verbs.Length; i++)
                    {
                        if (settings.verbs[i].index == myTarget.inventoryActions[indexInv].verb.index)
                            verbSelected = i;
                    }
                }
                verbSelected = EditorGUI.Popup(new Rect(rect.x + 7, rect.y, rect.width / 2.5f, EditorGUIUtility.singleLineHeight), "", verbSelected, verbsContent.ToArray());

                inventoryProperty.GetArrayElementAtIndex(indexInv).FindPropertyRelative("verb").FindPropertyRelative("index").intValue = settings.verbs[verbSelected].index;

                //EditorGUI.PropertyField(new Rect(rect.x + 7, rect.y, rect.width / 2.5f, EditorGUIUtility.singleLineHeight), inventoryProperty.GetArrayElementAtIndex(indexInv).FindPropertyRelative("verb").FindPropertyRelative("name"), GUIContent.none);

                PNCEditorUtils.DrawArrayWithAttempContainer(inventoryProperty, indexInv, rect, invAttempsListDict, invInteractionsListDict, customScriptInteractionDict, myTarget.inventoryActions[indexInv].attempsContainer.attemps, true);
            }
        };

    }


}
