using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class DialogsManager : MonoBehaviour
{

    private DialogsUI dialogsUI;
    
    private static DialogsManager instance;

    public Dialog activeDialog;
    public int activeSubDialog;

    private Task currentOptionTask;
    public bool waitingForTask;
    public DialogOption waitingOption;

    public bool InActiveDialog { get { return activeDialog != null; } }
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
        waitingForTask = false;
        activeDialog = null;
        activeSubDialog = 0;
        dialogsUI = FindObjectOfType<DialogsUI>();
        foreach (Dialog dialog in Resources.LoadAll<Dialog>("Dialogs/"))
        {
            dialog.current_entryDialogIndex = dialog.initial_entryDialogIndex;
            for (int i = 0; i < dialog.subDialogs.Count; i++)
            {
                for (int j = 0; j < dialog.subDialogs[i].options.Count; j++)
                {
                    dialog.subDialogs[i].options[j].currentState = (DialogOption.current_state)dialog.subDialogs[i].options[j].initialState;
                    dialog.subDialogs[i].options[j].currentText = dialog.subDialogs[i].options[j].initialText;
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

    public void ChangeOptionState(Dialog dialogSelected, int subDialogIndex, int optionIndex, DialogOption.current_state newOptionState)
    {
        ChangeOptionStateCommand command = new ChangeOptionStateCommand();
        command.Queue(dialogSelected, subDialogIndex, optionIndex, newOptionState);
    }

    public void ChangeOptionText(Dialog dialogSelected, int subDialogIndex, int optionIndex, string newOptionText)
    {
        ChangeOptionTextCommand command = new ChangeOptionTextCommand();
        command.Queue(dialogSelected, subDialogIndex, optionIndex, newOptionText);
    }

    public void OnClickOnOption(DialogOption actualOption)
    {
        dialogsUI.HideDialog();
        if (actualOption.say)
        {
            foreach (PNCCharacter character in FindObjectsOfType<PNCCharacter>())
            {
                if (character.isPlayerCharacter)
                {
                    character.Talk(actualOption.currentText);
                }
            }
        }
        currentOptionTask = InteractionUtils.RunAttempsInteraction(actualOption.attempsContainer, InteractionObjectsType.dialogOption, "", -1, -1);
        waitingForTask = true;
        waitingOption = actualOption;
    }

    private void Update()
    {
        if (waitingForTask && currentOptionTask.IsCompleted)
        {
            waitingForTask = false;
            int destiny = waitingOption.subDialogDestinyIndex;
            if (destiny > 0)
                StartDialog(activeDialog, destiny);//queue
            else if (destiny == -2)
            {
                EndDialog();
            }
            else
                StartDialog(activeDialog, activeSubDialog);
        }
    }
}
