using UnityEngine;

public interface INpcCharacterBehaviorHelper : ICharacterBehaviorHelperEnhanced
{
    public NpcBrain NpcBrain { get; }
}

public interface ICharacterBehaviorHelper
{
    Transform Transform { get; }
    CharacterID CharacterID { get; }
    CharacterID ConversationTargetID { get; }
    bool IsInConversation { get; }
    CharacterInfo ConversationTarget { get; }
    RoomID CurrentRoom { get; }
    CharacterInfo CharacterInfo { get; }
}

public interface ICharacterBehaviorHelperEnhanced : ICharacterBehaviorHelper
{
    void UpdateConversationTarget(CharacterInfo conversationTarget);
    void EndConversation();
}

/// <summary>
/// Collection of state information that will be used by the behavior trees. Only act on with the interfaces.
/// </summary>
public class CharacterBehaviorInfo : MonoBehaviour, INpcCharacterBehaviorHelper
{
    [Header("Character Info")]
    [SerializeField] CharacterID _characterID;

    [Header("Conversation")]
    [SerializeField] bool _isInConversation = false;
    [SerializeField] CharacterInfo _conversationTarget = null;

    [Header("Location")]
    [SerializeField] RoomID _currentRoom;


    public Transform Transform => transform;
    public CharacterID CharacterID => _characterID;
    public CharacterInfo ConversationTarget => _conversationTarget;
    public bool IsInConversation => _isInConversation;
    public RoomID CurrentRoom => _currentRoom;

    public CharacterID ConversationTargetID => IsInConversation && ConversationTarget != null ? ConversationTarget.ID : null;
    public CharacterInfo CharacterInfo => GetComponent<CharacterInfo>();
    public NpcBrain NpcBrain => GetComponent<NpcBrain>();


    private void Start()
    {
        _characterID = GetComponent<CharacterInfo>().ID;
    }

    private void Update()
    {
        _currentRoom = RoomBB.Instance.GetCharacterRoomID(CharacterID);
    }

    public void UpdateConversationTarget(CharacterInfo conversationTarget)
    {
        _conversationTarget = conversationTarget;
        _isInConversation = true;
    }

    public void EndConversation()
    {
        _isInConversation = false;
        _conversationTarget = null;
    }
}