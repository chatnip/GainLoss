using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Conversation Base", menuName = "Scriptable Object/Conversation Base", order = int.MaxValue)]
public class ConversationBase : ScriptableObject
{
    public enum targetGO
    {
        player, another
    }

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
    [SerializeField] public ConversationBase.targetGO targetGO;
    [Tooltip("CameraPos = Npc Pos + this Value")] [SerializeField] public Vector3 CameraPos;
    [SerializeField] public string AnimationTriggerName;

    public NpcConversation(Sprite talkerSprite, string conversation, float conversationDurTime)
    {
        this.talkerSprite = talkerSprite;
        this.conversation = conversation;
        this.conversationDurTime = conversationDurTime;
    }
}
