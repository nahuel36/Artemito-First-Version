using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogsManager : MonoBehaviour
{

    private DialogsUI dialogsUI;
    
    private static DialogsManager instance;

    public static DialogsManager Instance
    {
        get {

            if (instance == null)
            {
                GameObject GO = new GameObject();
                DontDestroyOnLoad(GO);
                instance = GO.AddComponent<DialogsManager>();
            }
            return instance;
        }
    }

    public void Initialize()
    {
        dialogsUI = FindObjectOfType<DialogsUI>();
    }

    public void StartDialog(Dialog dialog, int subDialogIndex)
    {
        StartDialogCommand command = new StartDialogCommand();
        command.Queue(dialogsUI, dialog, subDialogIndex);
    }
}
