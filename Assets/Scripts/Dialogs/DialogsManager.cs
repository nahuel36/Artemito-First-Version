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
                GameObject GO = new GameObject("DialogsManager");
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
                    
                    dialog.subDialogs[i].options[j].current_local_properties = new LocalProperty[dialog.subDialogs[i].options[j].local_properties.Length];
                    for (int z = 0; z < dialog.subDialogs[i].options[j].local_properties.Length; z++)
                    {
                        dialog.subDialogs[i].options[j].current_local_properties[z] = new LocalProperty();
                        dialog.subDialogs[i].options[j].current_local_properties[z].boolean = dialog.subDialogs[i].options[j].local_properties[z].boolean;
                        dialog.subDialogs[i].options[j].current_local_properties[z].booleanDefault = dialog.subDialogs[i].options[j].local_properties[z].booleanDefault;
                        dialog.subDialogs[i].options[j].current_local_properties[z].integer = dialog.subDialogs[i].options[j].local_properties[z].integer;
                        dialog.subDialogs[i].options[j].current_local_properties[z].integerDefault = dialog.subDialogs[i].options[j].local_properties[z].integerDefault;
                        dialog.subDialogs[i].options[j].current_local_properties[z].String = dialog.subDialogs[i].options[j].local_properties[z].String;
                        dialog.subDialogs[i].options[j].current_local_properties[z].stringDefault = dialog.subDialogs[i].options[j].local_properties[z].stringDefault;

                    }

                    dialog.subDialogs[i].options[j].current_global_properties = new GlobalProperty[dialog.subDialogs[i].options[j].global_properties.Length];
                    for (int z = 0; z < dialog.subDialogs[i].options[j].current_global_properties.Length; z++)
                    {
                        dialog.subDialogs[i].options[j].current_global_properties[z] = new GlobalProperty();
                        dialog.subDialogs[i].options[j].current_global_properties[z].boolean = dialog.subDialogs[i].options[j].global_properties[z].boolean;
                        dialog.subDialogs[i].options[j].current_global_properties[z].booleanDefault = dialog.subDialogs[i].options[j].global_properties[z].booleanDefault;
                        dialog.subDialogs[i].options[j].current_global_properties[z].integer = dialog.subDialogs[i].options[j].global_properties[z].integer;
                        dialog.subDialogs[i].options[j].current_global_properties[z].integerDefault = dialog.subDialogs[i].options[j].global_properties[z].integerDefault;
                        dialog.subDialogs[i].options[j].current_global_properties[z].String = dialog.subDialogs[i].options[j].global_properties[z].String;
                        dialog.subDialogs[i].options[j].current_global_properties[z].stringDefault = dialog.subDialogs[i].options[j].global_properties[z].stringDefault;

                    }

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


    public void SetLocalProperty(Dialog dialogSelected, int subDialogIndex, int optionIndex, Interaction interact)
    {
        for (int i = 0; i < dialogSelected.subDialogs.Count; i++)
        {
            if (dialogSelected.subDialogs[i].index == subDialogIndex)
            {
                for (int j = 0; j < dialogSelected.subDialogs[i].options.Count; j++)
                {
                    if (dialogSelected.subDialogs[i].options[j].index == optionIndex)
                    {
                        CommandSetLocalProperty command = new CommandSetLocalProperty();
                        command.Queue(dialogSelected.subDialogs[i].options[j].current_local_properties[interact.localPropertySelected], interact);
                    }
                }
            }
        }
    }

    public void SetGlobalProperty(Dialog dialogSelected, int subDialogIndex, int optionIndex, Interaction interaction)
    {
        for (int i = 0; i < dialogSelected.subDialogs.Count; i++)
        {
            if (dialogSelected.subDialogs[i].index == subDialogIndex)
            {
                for (int j = 0; j < dialogSelected.subDialogs[i].options.Count; j++)
                {
                    if (dialogSelected.subDialogs[i].options[j].index == optionIndex)
                    {
                        CommandSetGlobalProperty command = new CommandSetGlobalProperty();
                        command.Queue(dialogSelected.subDialogs[i].options[j].current_global_properties[interaction.globalPropertySelected], interaction);
                    }
                }
            }
        }

    }

    public GlobalProperty GetGlobalProperty(Dialog dialogSelected, int subDialogIndex, int optionIndex, Interaction interaction)
    {
        for (int i = 0; i < dialogSelected.subDialogs.Count; i++)
        {
            if (dialogSelected.subDialogs[i].index == subDialogIndex)
            {
                for (int j = 0; j < dialogSelected.subDialogs[i].options.Count; j++)
                {
                    if (dialogSelected.subDialogs[i].options[j].index == optionIndex)
                    {
                        return dialogSelected.subDialogs[i].options[j].current_global_properties[interaction.globalPropertySelected];
                    }
                }
            }
        }
        return null;
    }

    public LocalProperty GetLocalProperty(Dialog dialogSelected, int subDialogIndex, int optionIndex, Interaction interaction)
    {
        for (int i = 0; i < dialogSelected.subDialogs.Count; i++)
        {
            if (dialogSelected.subDialogs[i].index == subDialogIndex)
            {
                for (int j = 0; j < dialogSelected.subDialogs[i].options.Count; j++)
                {
                    if (dialogSelected.subDialogs[i].options[j].index == optionIndex)
                    {
                        return dialogSelected.subDialogs[i].options[j].current_local_properties[interaction.localPropertySelected];
                    }
                }
            }
        }
        return null;
    }

    public GenericProperty GetGenericProperty(InteractionUtils.PropertyActionType actionType, Dialog dialogSelected, int subDialogIndex, int optionIndex, Interaction interaction)
    {
        if (actionType == InteractionUtils.PropertyActionType.get_global_property)
            return GetGlobalProperty(dialogSelected, subDialogIndex, optionIndex, interaction);
        else if (actionType == InteractionUtils.PropertyActionType.get_local_property)
            return GetLocalProperty(dialogSelected, subDialogIndex, optionIndex, interaction);
        else
            return null;
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
        currentOptionTask = InteractionUtils.RunAttempsInteraction(actualOption.attempsContainer, InteractionObjectsType.dialogOption, null, null);
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
