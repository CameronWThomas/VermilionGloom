using UnityEngine;

/// <summary>
/// Collection of state information that will be used by the behavior trees
/// NOTE: Only properties should be read unless by The <see cref="NpcBehaviorBB"/> and this instance
/// </summary>
public class CharacterBehaviorInfo : MonoBehaviour
{
    [Header("Character Info")]
    public CharacterID CharacterID;

    [Header("Conversation")]
    public bool IsInConversation = false;
    public CharacterInfo ConversationTarget = null;

    [Header("Location")]
    public RoomID CurrentRoom;


    public CharacterID ConversationTargetID => IsInConversation && ConversationTarget != null ? ConversationTarget.ID : null;
    public CharacterInfo CharacterInfo => GetComponent<CharacterInfo>();


    private void Start()
    {
        CharacterID = GetComponent<CharacterInfo>().ID;
    }

    private void Update()
    {
        CurrentRoom = RoomBB.Instance.GetCharacterRoomID(CharacterID);
    }

    public void UpdateConversationTarget(CharacterInfo conversationTarget)
    {
        ConversationTarget = conversationTarget;
        IsInConversation = true;
    }

    public void EndConversation()
    {
        IsInConversation = false;
        ConversationTarget = null;
    }
}