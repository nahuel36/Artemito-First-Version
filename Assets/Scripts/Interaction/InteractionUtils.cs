using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Threading.Tasks;
public static class InteractionUtils 
{
    static UnhandledEvents unhandledEvents;
    static InventoryList inventory;
    public static VerbInteractions FindVerb(Verb verb, List<VerbInteractions> verbs)
    {
        for (int i = 0; i < verbs.Count; i++)
        {
            if (verbs[i].verb == verb)
                return verbs[i];
        }
        return null;
    }

    public async static Task RunAttempsInteraction(AttempsContainer attempsContainer, InteractionObjectsType interactionType, Verb verb, InventoryItem[] items, PNCInteractuable[] sceneInteractuables = null, bool runInBackground = false)
    {
        bool runUnhandledEvents = false;
        if (attempsContainer.attemps.Count == 0)
            runUnhandledEvents = true;
        else 
        {
            int index;

            if (attempsContainer.attempsIteration == AttempsContainer.Iteration.inOrderAndRepeatLastOne)
                index = Mathf.Clamp(attempsContainer.executedTimes, 0, attempsContainer.attemps.Count - 1);
            else if(attempsContainer.attempsIteration == AttempsContainer.Iteration.inOrderAndRestartAgain)
                index = attempsContainer.executedTimes % attempsContainer.attemps.Count;
            else 
                index = Random.Range(0,attempsContainer.attemps.Count);

            if (attempsContainer.attemps[index].interactions.Count == 0)
            { 
                runUnhandledEvents = true;
            }
            else 
            {
                int i = 0;
                while (i < attempsContainer.attemps[index].interactions.Count)
                {
                    InitializeInteractionCommand command = new InitializeInteractionCommand();
                    command.Queue(attempsContainer.attemps[index].interactions[i], runInBackground);

                    while (command.action == null)
                    {
                        await Task.Yield();
                    }

                    CustomArgument argument;
                    if (attempsContainer.attemps[index].interactions[i].customActionArguments.Count <= 0)
                    {
                        attempsContainer.attemps[index].interactions[i].customActionArguments = new List<CustomArgument>();
                        argument = new CustomArgument();
                        attempsContainer.attemps[index].interactions[i].customActionArguments.Add(argument);
                    }
                    else
                    {
                        argument = attempsContainer.attemps[index].interactions[i].customActionArguments[0];
                    }
                    argument.interactionType = interactionType;
                    argument.verb = verb;
                    if(items != null && items.Length > 0)
                        argument.itemIndex = items[0].specialIndex;
                    if (sceneInteractuables != null && sceneInteractuables.Length > 0)
                        argument.interactuableID = sceneInteractuables[0].GetInstanceID();

                    attempsContainer.attemps[index].interactions[i].customActionArguments[0] = argument;

                    if (interactionType == InteractionObjectsType.inventoryIninventory)
                    {
                        CustomArgument argument2 = new CustomArgument();
                        argument2.interactionType = interactionType;
                        argument2.verb = verb;
                        argument2.itemIndex = items[1].specialIndex;

                        if (attempsContainer.attemps[index].interactions[i].customActionArguments.Count <= 1)
                        {
                            attempsContainer.attemps[index].interactions[i].customActionArguments.Add(argument2);
                        }
                        else
                        {
                            attempsContainer.attemps[index].interactions[i].customActionArguments[1] = argument2;
                        }
                    }
                    else if (interactionType == InteractionObjectsType.objectInObject)
                    {
                        CustomArgument argument2 = new CustomArgument();
                        argument2.interactionType = interactionType;
                        argument2.verb = verb;
                        argument2.interactuableID = sceneInteractuables[1].GetInstanceID();

                        if (attempsContainer.attemps[index].interactions[i].customActionArguments.Count <= 1)
                        {
                            attempsContainer.attemps[index].interactions[i].customActionArguments.Add(argument2);
                        }
                        else
                        {
                            attempsContainer.attemps[index].interactions[i].customActionArguments[1] = argument2;
                        }
                    }

                    command.action.Invoke(attempsContainer.attemps[index].interactions[i].customActionArguments);

                    i++;

                    if (i < attempsContainer.attemps[index].interactions.Count)
                    {
                        i = CheckConditionals(i, attempsContainer.attemps[index].interactions[i - 1]);
                        if (i == -1)
                        {
                            break;
                        }
                    }
                }
            }
        }

        if (runUnhandledEvents && interactionType != InteractionObjectsType.unhandledEvent)
        {
            RunUnhandledEvents(interactionType, verb, items, sceneInteractuables, runInBackground);
        }
        
        attempsContainer.executedTimes++;
    }

    public static void RunUnhandledEvents(InteractionObjectsType interactionType, Verb verb, InventoryItem[] item, PNCInteractuable[] sceneInteractuable = null, bool runInBackground = false)
    {
        if (!unhandledEvents)
        {
            unhandledEvents = Resources.Load<UnhandledEvents>("UnhandledEvents");
        }
        if (!inventory)
        {
            inventory = Resources.Load<InventoryList>("Inventory");
        }
        if (interactionType == InteractionObjectsType.verbInObject || interactionType == InteractionObjectsType.verbInInventory)
        {
            for (int i = 0; i < unhandledEvents.verbs.Count; i++)
            {
                if (verb.index == unhandledEvents.verbs[i].verb.index)
                {
                    RunAttempsInteraction(unhandledEvents.verbs[i].attempsContainer, InteractionObjectsType.unhandledEvent, verb, item, sceneInteractuable);
                }
            }
        }
        else if (interactionType == InteractionObjectsType.inventoryInObject)
        {
            for (int i = 0; i < unhandledEvents.inventoryActions.Count; i++)
            {
                if (item[0].specialIndex == unhandledEvents.inventoryActions[i].specialIndex && verb.index == unhandledEvents.inventoryActions[i].verb.index)
                {
                    RunAttempsInteraction(unhandledEvents.inventoryActions[i].attempsContainer, InteractionObjectsType.unhandledEvent, verb, item, sceneInteractuable);
                }
            }
        }
        else if (interactionType == InteractionObjectsType.inventoryIninventory)
        {
            for (int i = 0; i < unhandledEvents.inventoryActions.Count; i++)
            {
                if (item[0].specialIndex == unhandledEvents.inventoryActions[i].specialIndex && verb.index == unhandledEvents.inventoryActions[i].verb.index)
                {
                    RunAttempsInteraction(unhandledEvents.inventoryActions[i].attempsContainer, InteractionObjectsType.unhandledEvent, verb, item);
                }
            }
        }
        else if (interactionType == InteractionObjectsType.objectInObject)
        { 
            for (int i = 0; i < unhandledEvents.inventoryActions.Count; i++)
            {
                if (unhandledEvents.inventoryActions[i].sceneObject == sceneInteractuable[0] && verb.index == unhandledEvents.inventoryActions[i].verb.index)
                {
                    RunAttempsInteraction(unhandledEvents.inventoryActions[i].attempsContainer, InteractionObjectsType.unhandledEvent, verb, item, sceneInteractuable);
                }
            }
        }
    }

    private static int CheckConditionals(int actualindex, Interaction interaction)
    {
        bool result = true;
        if (CheckArePropertyInteraction(PropertyObjectType.any, PropertyActionType.any_get, interaction))
        {
            int index;
            PropertyActionType actiontype;
            if (CheckArePropertyInteraction(PropertyObjectType.any, PropertyActionType.get_global_property, interaction))
            {
                actiontype = PropertyActionType.get_global_property;
                index = interaction.globalPropertySelected;
            }
            else
            { 
                actiontype = PropertyActionType.get_local_property;
                index = interaction.localPropertySelected;
            }




            GenericProperty property = null;
            if (interaction.type == Interaction.InteractionType.properties_container)
                property = interaction.propertyObject.GenericProperties(actiontype)[index];
            else if (interaction.type == Interaction.InteractionType.character)
                property = interaction.character.GenericProperties(actiontype)[index];
            else if (interaction.type == Interaction.InteractionType.inventory)
            {
                if (inventory == null)
                    inventory = Resources.Load<InventoryList>("Inventory");

                for (int i = 0; i < inventory.items.Length; i++)
                {
                    if (inventory.items[i].specialIndex == interaction.inventorySelected)
                    {
                        property = inventory.items[i].GenericCurrentProperties(actiontype)[index];
                    }
                }
            }
            else if (interaction.type == Interaction.InteractionType.dialog)
            {
                property = DialogsManager.Instance.GetGenericProperty(actiontype, interaction.dialogSelected, interaction.subDialogIndex, interaction.optionIndex, interaction);
            }


            bool compareBoolean, compareInteger, compareString, compareFloat;
            bool booleanValue, defaultBooleanValue;
            int integerValue, defaultIntegerValue;
            string stringValue, defaultStringValue;
            if (CheckArePropertyInteraction(PropertyObjectType.any, PropertyActionType.get_global_property, interaction))
            {
                compareBoolean = interaction.global_compareBooleanValue;
                compareInteger = interaction.global_compareIntegerValue;
                compareString = interaction.global_compareStringValue;
                booleanValue = interaction.global_BooleanValue;
                integerValue = interaction.global_IntegerValue;
                stringValue = interaction.global_StringValue;
                defaultBooleanValue = interaction.global_defaultBooleanValue;
                defaultIntegerValue = interaction.global_defaultIntegerValue;
                defaultStringValue = interaction.global_defaultStringValue;
            }
            else
            {
                compareBoolean = interaction.local_compareBooleanValue;
                compareInteger = interaction.local_compareIntegerValue;
                compareString = interaction.local_compareStringValue;
                booleanValue = interaction.local_BooleanValue;
                integerValue = interaction.local_IntegerValue;
                stringValue = interaction.local_StringValue;
                defaultBooleanValue = interaction.local_defaultBooleanValue;
                defaultIntegerValue = interaction.local_defaultIntegerValue;
                defaultStringValue = interaction.local_defaultStringValue;
            }


            if (compareBoolean)
            {
                if (property.booleanDefault && booleanValue != defaultBooleanValue)
                    result = false;
                if (!property.booleanDefault && booleanValue != property.boolean)
                    result = false;
            }
            if (compareInteger)
            {
                int valueToCompare = property.integerDefault ? defaultIntegerValue : property.integer;

                if ((interaction.compareIntegerOrFloatOperation == Interaction.CompareIntegerOrFloatOperation.areEqual
                    && integerValue != valueToCompare)
                 || (interaction.compareIntegerOrFloatOperation == Interaction.CompareIntegerOrFloatOperation.isGreaterThan
                    && integerValue >= valueToCompare)
                 || (interaction.compareIntegerOrFloatOperation == Interaction.CompareIntegerOrFloatOperation.isLessThan
                    && integerValue <= valueToCompare))
                    result = false;
            }
            if (compareString)
            {
                string valueToCompare = property.stringDefault ? stringValue : property.String;

                if ((interaction.compareStringOperation == Interaction.CompareStringOperation.areEqualCaseSensitive
                    && stringValue != valueToCompare)
                || (interaction.compareStringOperation == Interaction.CompareStringOperation.areEqualCaseInsensitive
                    && stringValue.ToLower() != valueToCompare.ToLower())
                || (interaction.compareStringOperation == Interaction.CompareStringOperation.containsCaseSensitive
                    && !valueToCompare.Contains(stringValue))
                || (interaction.compareStringOperation == Interaction.CompareStringOperation.containsCaseInsensitive
                    && !valueToCompare.ToLower().Contains(stringValue.ToLower())))
                    result = false;
            }
        }

        else if (interaction.type == Interaction.InteractionType.custom && interaction.customScriptAction == Interaction.CustomScriptAction.customBoolean)
        { 
            result = interaction.customActionArguments[0].resultBool;
        }
        else
            return actualindex;

        if (result == true)
        {
            if (interaction.OnCompareResultTrueAction == Conditional.GetPropertyAction.Stop)
                return -1;
            else if (interaction.OnCompareResultTrueAction == Conditional.GetPropertyAction.Continue)
                return actualindex;
            else
                return interaction.LineToGoOnTrueResult;
        }
        else
        {
            if (interaction.OnCompareResultFalseAction == Conditional.GetPropertyAction.Stop)
                return -1;
            else if (interaction.OnCompareResultFalseAction == Conditional.GetPropertyAction.Continue)
                return actualindex;
            else
                return interaction.LineToGoOnFalseResult;
        }
            
    }

    public static UnityEvent<List<CustomArgument>> InitializeInteraction(Interaction interaction)
    {
        UnityEvent<List<CustomArgument>> action = new UnityEvent<List<CustomArgument>>();
        if (interaction.type == Interaction.InteractionType.custom)
            action = interaction.action;
        else if (interaction.type == Interaction.InteractionType.character)
        {
            PNCCharacter charact = interaction.character;
            if (interaction.characterAction == Interaction.CharacterAction.say)
            {
                string whattosay = interaction.WhatToSay;
                if (interaction.CanSkip)
                    action.AddListener((arguments) => charact.Talk(whattosay));
                else
                    action.AddListener((arguments) => charact.UnskippableTalk(whattosay));
            }
            else if (interaction.characterAction == Interaction.CharacterAction.sayWithScript)
            {
                if (interaction.CanSkip)
                    action.AddListener((arguments) => charact.Talk(((SayScript)interaction.SayScript).SayWithScript(arguments)));
                else
                    action.AddListener((arguments) => charact.UnskippableTalk(((SayScript)interaction.SayScript).SayWithScript(arguments)));
            }
            else if (interaction.characterAction == Interaction.CharacterAction.walk)
            {
                action.AddListener((arguments) => charact.Walk(interaction.WhereToWalk.position));
            }
            else if (interaction.characterAction == Interaction.CharacterAction.walkStraight)
            {
                action.AddListener((arguments) => charact.WalkStraight(interaction.WhereToWalk.position));
            }
            else if (interaction.characterAction == Interaction.CharacterAction.setLocalProperty || interaction.characterAction == Interaction.CharacterAction.setGlobalProperty)
            {
                PNCCharacter varContainer = (PNCCharacter)interaction.propertyObject;
                if (interaction.characterAction == Interaction.CharacterAction.setLocalProperty)
                {
                    action.AddListener((arguments) =>
                    varContainer.SetLocalProperty(interaction,
                                                    interaction.propertyObject.LocalProperties[interaction.localPropertySelected]));
                }
                else if (interaction.characterAction== Interaction.CharacterAction.setGlobalProperty)
                {
                    action.AddListener((arguments) =>
                    varContainer.SetGlobalProperty(interaction,
                                                    interaction.propertyObject.GlobalProperties[interaction.globalPropertySelected]));
                }
            }
        }
        else if (interaction.type == Interaction.InteractionType.dialog)
        {
            if (interaction.dialogAction == Interaction.DialogAction.startDialog)
            {
                action.AddListener((arguments) => DialogsManager.Instance.StartDialog(interaction.dialogSelected, interaction.dialogSelected.current_entryDialogIndex));
            }
            else if (interaction.dialogAction == Interaction.DialogAction.changeEntry)
            {
                action.AddListener((arguments) => DialogsManager.Instance.ChangeEntry(interaction.dialogSelected, interaction.newDialogEntry));
            }
            else if (interaction.dialogAction == Interaction.DialogAction.changeOptionState)
            {
                action.AddListener((arguments) => DialogsManager.Instance.ChangeOptionState(interaction.dialogSelected, interaction.subDialogIndex, interaction.optionIndex, interaction.newOptionState));
            }
            else if (interaction.dialogAction == Interaction.DialogAction.changeOptionText)
            {
                action.AddListener((arguments) => DialogsManager.Instance.ChangeOptionText(interaction.dialogSelected, interaction.subDialogIndex, interaction.optionIndex, interaction.newOptionText));
            }
            else if (interaction.dialogAction == Interaction.DialogAction.setGlobalProperty || interaction.dialogAction == Interaction.DialogAction.setLocalProperty)
            {
                if (interaction.dialogAction == Interaction.DialogAction.setLocalProperty)
                {
                    action.AddListener((arguments) =>
                    DialogsManager.Instance.SetLocalProperty(interaction.dialogSelected, interaction.subDialogIndex, interaction.optionIndex, interaction)
                    );
                }
                else if (interaction.dialogAction == Interaction.DialogAction.setGlobalProperty)
                {
                    action.AddListener((arguments) =>
                    DialogsManager.Instance.SetGlobalProperty(interaction.dialogSelected, interaction.subDialogIndex, interaction.optionIndex,interaction));
                }
            }
        }
        else if (interaction.type == Interaction.InteractionType.properties_container)
        {
            PNCPropertiesContainer varContainer = (PNCPropertiesContainer)interaction.propertyObject;
            if (interaction.propertiesAction == Interaction.PropertiesContainerAction.setLocalProperty)
            {
                action.AddListener((arguments) =>
                varContainer.SetLocalProperty(interaction,
                                                varContainer.LocalProperties[interaction.localPropertySelected]));
            }
            else if (interaction.propertiesAction == Interaction.PropertiesContainerAction.setGlobalProperty)
            {
                action.AddListener((arguments) =>
                varContainer.SetGlobalProperty(interaction,
                                                varContainer.GlobalProperties[interaction.globalPropertySelected]));
            }
        }
        else if (interaction.type == Interaction.InteractionType.inventory)
        {
            if (interaction.inventoryAction == Interaction.InventoryAction.useAsInventory)
            {
                action.AddListener((arguments) =>
                {
                    CommandUseAsInventory command = new CommandUseAsInventory();
                    command.Queue();
                });
            }
            else if (interaction.inventoryAction == Interaction.InventoryAction.setLocalProperty || interaction.inventoryAction == Interaction.InventoryAction.setGlobalProperty)
            {
                if (interaction.inventorySelected > 0)
                {
                    
                    if(inventory == null)
                        inventory = Resources.Load<InventoryList>("Inventory");

                    int specialIndex = interaction.inventorySelected;
                    for (int i = 0; i < inventory.items.Length; i++)
                    {
                        if (inventory.items[i].specialIndex == specialIndex)
                        {
                            InventoryItem varContainer = (InventoryItem)inventory.items[i];

                            if (interaction.inventoryAction == Interaction.InventoryAction.setLocalProperty)
                                action.AddListener((arguments) =>
                                {
                                    varContainer.SetLocalProperty(interaction, varContainer.current_local_properties[interaction.localPropertySelected]);
                                });

                            else
                                action.AddListener((arguments) =>
                                {
                                    varContainer.SetGlobalProperty(interaction, varContainer.current_global_properties[interaction.globalPropertySelected]);
                                });
                        }    
                        
                    }
                }

            }
            
            
        }
        return action;
    }

    public enum PropertyObjectType
    {
        room_object,
        character,
        dialog_option,
        inventory,
        properties_container,
        any
    }

    public enum PropertyActionType
    {
        set_global_property,
        get_global_property,
        set_local_property,
        get_local_property,
        any_local,
        any_global,
        any_set,
        any_get,
        any
    }

    public static bool CheckArePropertyInteraction(PropertyObjectType type, PropertyActionType proptype, Interaction interaction)
    {
        bool areType = false;
        bool areProp = false;

        if (proptype == PropertyActionType.any)
        {
            if ((interaction.type == Interaction.InteractionType.properties_container &&
                     (interaction.propertiesAction == Interaction.PropertiesContainerAction.getGlobalProperty
                   || interaction.propertiesAction == Interaction.PropertiesContainerAction.getLocalProperty
                   || interaction.propertiesAction == Interaction.PropertiesContainerAction.setGlobalProperty
                   || interaction.propertiesAction == Interaction.PropertiesContainerAction.setLocalProperty))
            || (interaction.type == Interaction.InteractionType.inventory &&
                     (interaction.inventoryAction == Interaction.InventoryAction.getGlobalProperty
                   || interaction.inventoryAction == Interaction.InventoryAction.getLocalProperty
                   || interaction.inventoryAction == Interaction.InventoryAction.setGlobalProperty
                   || interaction.inventoryAction == Interaction.InventoryAction.setLocalProperty))
            || (interaction.type == Interaction.InteractionType.character &&
                     (interaction.characterAction == Interaction.CharacterAction.getGlobalProperty
                   || interaction.characterAction == Interaction.CharacterAction.getLocalProperty
                   || interaction.characterAction == Interaction.CharacterAction.setGlobalProperty
                   || interaction.characterAction == Interaction.CharacterAction.setLocalProperty))
            || (interaction.type == Interaction.InteractionType.dialog &&
                     (interaction.dialogAction == Interaction.DialogAction.getGlobalProperty
                   || interaction.dialogAction == Interaction.DialogAction.getLocalProperty
                   || interaction.dialogAction == Interaction.DialogAction.setGlobalProperty
                   || interaction.dialogAction == Interaction.DialogAction.setLocalProperty)))
                areProp = true;
        }
        if (proptype == PropertyActionType.get_global_property || proptype == PropertyActionType.any_global || proptype == PropertyActionType.any_get)
        {
            if ((interaction.type == Interaction.InteractionType.properties_container &&
                     interaction.propertiesAction == Interaction.PropertiesContainerAction.getGlobalProperty)
               || (interaction.type == Interaction.InteractionType.inventory &&
                     interaction.inventoryAction == Interaction.InventoryAction.getGlobalProperty)
               || (interaction.type == Interaction.InteractionType.character &&
                     interaction.characterAction == Interaction.CharacterAction.getGlobalProperty)
                || (interaction.type == Interaction.InteractionType.dialog &&
                     interaction.dialogAction == Interaction.DialogAction.getGlobalProperty))
                areProp = true;
        }
        if (proptype == PropertyActionType.get_local_property || proptype == PropertyActionType.any_local || proptype == PropertyActionType.any_get)
        {
            if ((interaction.type == Interaction.InteractionType.properties_container &&
                     interaction.propertiesAction == Interaction.PropertiesContainerAction.getLocalProperty)
               || (interaction.type == Interaction.InteractionType.inventory &&
                     interaction.inventoryAction == Interaction.InventoryAction.getLocalProperty)
               || (interaction.type == Interaction.InteractionType.character &&
                     interaction.characterAction == Interaction.CharacterAction.getLocalProperty)
                || (interaction.type == Interaction.InteractionType.dialog &&
                     interaction.dialogAction == Interaction.DialogAction.getLocalProperty))
                areProp = true;
        }
        if (proptype == PropertyActionType.set_global_property || proptype == PropertyActionType.any_global || proptype == PropertyActionType.any_set)
        {
            if ((interaction.type == Interaction.InteractionType.properties_container &&
                     interaction.propertiesAction == Interaction.PropertiesContainerAction.setGlobalProperty)
               || (interaction.type == Interaction.InteractionType.inventory &&
                     interaction.inventoryAction == Interaction.InventoryAction.setGlobalProperty)
               || (interaction.type == Interaction.InteractionType.character &&
                     interaction.characterAction == Interaction.CharacterAction.setGlobalProperty)
                || (interaction.type == Interaction.InteractionType.dialog &&
                     interaction.dialogAction == Interaction.DialogAction.setGlobalProperty))
                areProp = true;
        }
        if (proptype == PropertyActionType.set_local_property || proptype == PropertyActionType.any_local || proptype == PropertyActionType.any_set)
        {
            if ((interaction.type == Interaction.InteractionType.properties_container &&
                     interaction.propertiesAction == Interaction.PropertiesContainerAction.setLocalProperty)
               || (interaction.type == Interaction.InteractionType.inventory &&
                     interaction.inventoryAction == Interaction.InventoryAction.setLocalProperty)
               || (interaction.type == Interaction.InteractionType.character &&
                     interaction.characterAction == Interaction.CharacterAction.setLocalProperty)
                || (interaction.type == Interaction.InteractionType.dialog &&
                     interaction.dialogAction == Interaction.DialogAction.setLocalProperty))
                areProp = true;
        }


        if (type == PropertyObjectType.any)
        {
            if (interaction.type == Interaction.InteractionType.properties_container
             || interaction.type == Interaction.InteractionType.inventory
             || interaction.type == Interaction.InteractionType.character
             || interaction.type == Interaction.InteractionType.dialog)
                areType = true;
        }
        else if (type == PropertyObjectType.properties_container)
        {
            if (interaction.type == Interaction.InteractionType.properties_container)
                areType = true;
        }
        else if (type == PropertyObjectType.inventory)
        {
            if (interaction.type == Interaction.InteractionType.inventory)
                areType = true;
        }
        else if (type == PropertyObjectType.character)
        {
            if (interaction.type == Interaction.InteractionType.character)
                areType = true;
        }
        else if (type == PropertyObjectType.dialog_option)
        {
            if (interaction.type == Interaction.InteractionType.dialog)
                areType = true;
        }
        else
            areType = false;

        return areType && areProp;
    }

}

