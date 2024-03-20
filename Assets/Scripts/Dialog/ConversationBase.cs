using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Conversation Base", menuName = "Scriptable Object/Conversation Base", order = int.MaxValue)]
public class ConversationBase : ScriptableObject
{
    [SerializeField] List<NpcConversation> npcConversations;

    public ConversationBase(List<NpcConversation> npcConversation)
    {
        this.npcConversations = npcConversation;
    }

    public List<NpcConversation> NpcConversations
    {
        get { return npcConversations; }
    }
}

[System.Serializable]
public class NpcConversation
{
    [SerializeField] public Sprite talkerSprite;
    [SerializeField] public string talkerName;
    [SerializeField][TextArea] public string conversation;
    [SerializeField] public float conversationDurTime;

    public NpcConversation(Sprite talkerSprite, string conversation, float conversationDurTime)
    {
        this.talkerSprite = talkerSprite;
        this.conversation = conversation;
        this.conversationDurTime = conversationDurTime;
    }
}
