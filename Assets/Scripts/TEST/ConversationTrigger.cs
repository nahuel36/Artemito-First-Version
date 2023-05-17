using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConversationTrigger : MonoBehaviour
{
    public List<Conversation> conversations;

    public void SaveConversations() { }

    public void LoadConversations() { }
}

[System.Serializable]
public class Conversation
{
    public string Name;
    public bool Foldout;
    public List<Dialogue> Dialogues;
}

[System.Serializable]
public class Dialogue
{
    public string Name;
    public bool Foldout;
    public List<string> Sentences;
}
