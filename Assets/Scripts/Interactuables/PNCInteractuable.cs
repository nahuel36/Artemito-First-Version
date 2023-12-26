using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;



[System.Flags]
public enum VariableType
{
    none = 0,
    boolean_type = 1 << 0,
    integer_type = 1 << 1,
    string_type = 1 << 2,
    float_type = 1 << 3,
    object_type = 1 << 4,
    everything = ~0
}


[System.Flags]
public enum PropertyObjectType
{
    character = (1 << 0),
    room_object = (1 << 1),
    inventory = (1 << 2),
    propertiesContainer = (1 << 3),
    dialogOption = (1 << 4),
    any = ~0
}

[System.Flags]
public enum PropertyActionType
{
    setGlobalProperty = 1 << 0,
    getGlobalProperty = 1 << 1,
    setLocalProperty = 1 << 2,
    getLocalProperty = 1 << 3,
    anySet = setGlobalProperty | setLocalProperty,
    anyGet = getGlobalProperty | getLocalProperty,
    anyLocal = setLocalProperty | getLocalProperty,
    anyGlobal = getGlobalProperty | setGlobalProperty,
    any = ~0
}


[System.Serializable]
public class Verb
{
    public string name = "";
    public bool isLikeGive;
    public bool isLikeUse;
    public int index;
}


[System.Serializable]
public class VerbInteractions
{
    public Verb verb = new Verb();
    public bool use = true;
    public AttempsContainer attempsContainer = new AttempsContainer();
}

[System.Serializable]
public class InventoryItemAction {
    public int specialIndex = -1;
    public Verb verb;
    public PNCInteractuable sceneObject;
    public AttempsContainer attempsContainer;
}

[System.Serializable]
public class AttempsContainer{
    public enum Iteration { 
        inOrderAndRepeatLastOne,
        inOrderAndRestartAgain,
        random
    }
    public Iteration attempsIteration;
    public List<InteractionsAttemp> attemps = new List<InteractionsAttemp>();
    public bool expandedInInspector;
    public int executedTimes = 0;
}

[System.Serializable]
public class InteractionsAttemp
{
    public List<Interaction> interactions = new List<Interaction>();
    public bool expandedInInspector;
}

[System.Serializable]
public class CustomArgument
{
    public string name;

    public VariableType type;

    public string stringArgument;
    public bool boolArgument;
    public int intArgument;
    public Object objectArgument;

    public bool resultBool;

    public InteractionObjectsType interactionType;
    public Verb verb;
    public int itemIndex;
    public int interactuableID;

    public bool expandedInInspector;
}

[System.Serializable]
public class Interaction
{
    public enum InteractionType
    {
        character,
        propertiesContainer,
        inventory,
        dialog,
        custom
    }
    public InteractionType type;

    public bool expandedInInspector;

    public enum InventoryAction { 
        useAsInventory,
        addInventory,
        quitInventory,
        haveInventory, 
        setLocalProperty,
        getLocalProperty,
        setGlobalProperty,
        getGlobalProperty
    }
    public InventoryAction inventoryAction;
    public int inventorySelected;
    //CHARACTER
    public enum CharacterAction
    {
        say,
        sayWithScript,
        walk,
        walkStraight,
        setLocalProperty,
        getLocalProperty,
        setGlobalProperty,
        getGlobalProperty
    }
    public PNCCharacter character;
    public CharacterAction characterAction;
    public string WhatToSay;
    public Transform WhereToWalk;
    //PROPERTIES
    public enum PropertiesContainerAction
    {
        setLocalProperty,
        getLocalProperty,
        setGlobalProperty,
        getGlobalProperty
    }
    public int globalPropertySelected;
    public int localPropertySelected;
    public PropertiesContainerAction propertiesAction;
    public PNCPropertiesContainer propertyObject;

    public enum DialogAction
    { 
        startDialog,
        endCurrentDialog,
        changeEntry,
        changeOptionState,
        changeOptionText,
        getOptionState,
        getNumberOfExecutions,
        areAllSubdialogOptionsDisabled,
        setLocalProperty,
        getLocalProperty,
        setGlobalProperty,
        getGlobalProperty
    }
    public DialogAction dialogAction;
    public Dialog dialogSelected;
    public int newDialogEntry;
    public DialogOption.current_state newOptionState;
    public string newOptionText;
    public int subDialogIndex;
    public int optionIndex;

    public Conditional.GetPropertyAction OnCompareResultTrueAction;
    public Conditional.GetPropertyAction OnCompareResultFalseAction;
    public int LineToGoOnTrueResult;
    public int LineToGoOnFalseResult;
    //CUSTOM
    public UnityEvent<List<CustomArgument>> action;
    
    public VariableType global_variablesToChange;
    public VariableType local_variablesToChange;

    public VariableType global_variablesToCompare;
    public VariableType local_variablesToCompare;

    public bool local_defaultBooleanValue;
    public int local_defaultIntegerValue;
    public string local_defaultStringValue;
    public bool global_defaultBooleanValue;
    public int global_defaultIntegerValue;
    public string global_defaultStringValue;

    public enum ChangeBooleanOperation
    { 
        setValue, toggle
    }

    public ChangeBooleanOperation changeBooleanOperation;

    public enum ChangeIntegerOrFloatOperation
    { 
        add, set, subtract
    }

    public ChangeIntegerOrFloatOperation changeIntegerOrFloatOperation;

    public enum ChangeStringOperation
    {
        change, replace
    }

    public ChangeStringOperation changeStringOperation;
    public string replaceValueToFind;
    public enum CompareIntegerOrFloatOperation
    {
        isGreaterThan, isLessThan, areEqual
    }

    public CompareIntegerOrFloatOperation compareIntegerOrFloatOperation;

    public enum CompareStringOperation
    { 
        areEqualCaseSensitive,
        areEqualCaseInsensitive, 
        containsCaseSensitive,
        containsCaseInsensitive
    }

    public CompareStringOperation compareStringOperation;

    public bool global_BooleanValue;
    public string global_StringValue;
    public int global_IntegerValue;
    public bool local_BooleanValue;
    public string local_StringValue;
    public int local_IntegerValue;
    public MonoBehaviour SayScript;
    public bool CanSkip;
    public enum CustomScriptAction
    { 
        customScript,
        customBoolean
    }

    public CustomScriptAction customScriptAction;
    public List<CustomArgument> customActionArguments = new List<CustomArgument>();//51
    //averiguar sobre deep copy / clone
    public void Copy(Interaction destiny)
    {
        destiny.action = action;
        destiny.character = character;
        destiny.characterAction = characterAction;
        destiny.expandedInInspector = expandedInInspector;
        destiny.inventoryAction = inventoryAction;
        destiny.inventorySelected = inventorySelected;
        destiny.dialogAction = dialogAction;
        destiny.dialogSelected = dialogSelected;
        destiny.newDialogEntry = newDialogEntry;
        destiny.newOptionState = newOptionState;
        destiny.newOptionText = newOptionText;
        destiny.subDialogIndex = subDialogIndex;
        destiny.optionIndex = optionIndex;
        destiny.globalPropertySelected = globalPropertySelected;
        destiny.localPropertySelected = localPropertySelected;
        destiny.type = type;
        destiny.propertyObject = propertyObject;
        destiny.propertiesAction = propertiesAction;
        destiny.WhatToSay = WhatToSay;
        destiny.WhereToWalk = WhereToWalk;
        destiny.local_variablesToChange = local_variablesToChange;
        destiny.local_variablesToCompare = local_variablesToCompare;
        destiny.global_variablesToChange = global_variablesToChange;
        destiny.global_variablesToCompare = global_variablesToCompare;
        destiny.local_BooleanValue = local_BooleanValue;
        destiny.local_IntegerValue = local_IntegerValue;
        destiny.local_StringValue = local_StringValue;
        destiny.global_BooleanValue = global_BooleanValue;
        destiny.global_IntegerValue = global_IntegerValue;
        destiny.global_StringValue = global_StringValue;
        destiny.changeBooleanOperation = changeBooleanOperation;
        destiny.compareIntegerOrFloatOperation = compareIntegerOrFloatOperation;
        destiny.changeIntegerOrFloatOperation = changeIntegerOrFloatOperation;
        destiny.replaceValueToFind = replaceValueToFind;
        destiny.changeStringOperation = changeStringOperation;
        destiny.compareStringOperation = compareStringOperation;
        destiny.local_defaultBooleanValue = local_defaultBooleanValue;
        destiny.local_defaultIntegerValue = local_defaultIntegerValue;
        destiny.local_defaultStringValue = local_defaultStringValue;
        destiny.global_defaultBooleanValue = global_defaultBooleanValue;
        destiny.global_defaultIntegerValue = global_defaultIntegerValue;
        destiny.global_defaultStringValue = global_defaultStringValue;
        destiny.LineToGoOnFalseResult = LineToGoOnFalseResult;
        destiny.LineToGoOnTrueResult = LineToGoOnTrueResult;
        destiny.OnCompareResultFalseAction = OnCompareResultFalseAction;
        destiny.OnCompareResultTrueAction = OnCompareResultTrueAction;
        destiny.SayScript = SayScript;
        destiny.CanSkip = CanSkip;
        destiny.customScriptAction = customScriptAction;
        destiny.customActionArguments = customActionArguments;//51
    }
}

public enum InteractionObjectsType
{
    inventoryIninventory,
    inventoryInObject,
    objectInObject,
    verbInObject,
    verbInInventory,
    dialogOption,
    sceneEvent,
    finishedTimer,
    unhandledEvent,

}

public interface PNCPropertyInterface
{
    public LocalProperty[] LocalProperties { get; set; }
    public GlobalProperty[] GlobalProperties { get; set; }
}


[System.Serializable]
public abstract class PNCInteractuable : PNCPropertiesContainer
{
    public string interactuableName;
    
    public int priority = 0;

    public List<VerbInteractions> verbs = new List<VerbInteractions>();
    public List<InventoryItemAction> inventoryActions = new List<InventoryItemAction>();

    public Settings settings;
    private void Start()
    {
        settings = Resources.Load<Settings>("Settings/Settings");
    }


    public Verb[] GetActiveVerbs() 
    {
        List<Verb> activeVerbs = new List<Verb>();
        for (int i = 0; i < settings.verbs.Length; i++)
        {
            bool founded = false;
            for (int j = 0; j < verbs.Count; j++)
            {
                if (settings.verbs[i].index == verbs[j].verb.index)
                {
                    if (verbs[j].use || settings.alwaysShowAllVerbs)
                    {
                        activeVerbs.Add(verbs[j].verb);
                        founded = true;
                    }
                }
            }
            if(!founded && settings.alwaysShowAllVerbs)
                activeVerbs.Add(settings.verbs[i]);
        }
        return activeVerbs.ToArray();
    }


    public void RunInventoryInteraction(InventoryItem item, Verb verb)
    {
        int index = InventoryManager.Instance.getInventoryActionsIndex(item, inventoryActions, verb);
        if (index != -1)
        {
            InteractionUtils.RunAttempsInteraction(inventoryActions[index].attempsContainer, InteractionObjectsType.inventoryInObject, verb, new InventoryItem[] { item }, new PNCInteractuable[] { this });
        }
        else
        {
            InteractionUtils.RunUnhandledEvents(InteractionObjectsType.inventoryInObject, verb, new InventoryItem[] { item }, new PNCInteractuable[] { this });
        }
    }

    public void RunObjectAsInventoryInteraction(PNCInteractuable pncObject, Verb verb)
    {
        int index = InventoryManager.Instance.getInventoryActionsIndex(pncObject, inventoryActions, verb);
        if (index != -1)
        {
            InteractionUtils.RunAttempsInteraction(inventoryActions[index].attempsContainer, InteractionObjectsType.objectInObject, verb, null, new PNCInteractuable[] { this ,pncObject});
        }
        else
        {
            InteractionUtils.RunUnhandledEvents(InteractionObjectsType.objectInObject, verb, null, new PNCInteractuable[] { this, pncObject });
        }
    }

    public void RunVerbInteraction(Verb verb)
    {
        VerbInteractions verbToRun = InteractionUtils.FindVerb(verb, verbs);

        if (verbToRun != null)
            InteractionUtils.RunAttempsInteraction(verbToRun.attempsContainer, InteractionObjectsType.verbInObject, verb, null, new PNCInteractuable[] {this });
        else
            InteractionUtils.RunUnhandledEvents(InteractionObjectsType.verbInObject, verb, null, new PNCInteractuable[] { this });
    }
}
