using System;
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
        foreach (Dialog dialog in Resources.LoadAll<Dialog>("Dialogs/"))
        {
            dialog.current_entryDialogIndex = dialog.initial_entryDialogIndex;
            for (int i = 0; i < dialog.subDialogs.Count; i++)
            {
                for (int j = 0; j < dialog.subDialogs[i].options.Count; j++)
                {
                    dialog.subDialogs[i].options[j].currentState = (DialogOption.current_state)dialog.subDialogs[i].options[j].initialState;
                }
            }
        }
    }

    public void StartDialog(Dialog dialog, int subDialogIndex)
    {
        StartDialogCommand command = new StartDialogCommand();
        command.Queue(dialogsUI, dialog, subDialogIndex);
    }

    public void EndDialog()
    {
        EndDialogCommand command = new EndDialogCommand();
        command.Queue(dialogsUI);
        
    }

    public void ChangeEntry(Dialog dialogSelected, int newEntry)
    {
        ChangeEntryDialogCommand command = new ChangeEntryDialogCommand();
        command.Queue(dialogSelected, newEntry);
    }
}
