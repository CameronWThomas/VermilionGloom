using BehaviorDesigner.Runtime;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;


[RequireComponent(typeof(NPCHumanCharacterInfo))]
public partial class NpcBrain : MonoBehaviour
{
    [SerializeField] Transform _conversationTarget = null;
    [SerializeField] RoomID _currentRoom;

    public bool IsDead => GetComponent<CharacterInfo>().IsDead;
    public GameObject Dragger { get; private set; } = null;
    public bool IsBeingDragged => Dragger != null;

    public GameObject Strangler { get; private set; } = null;
    public bool IsBeingStrangled => Strangler != null;
    public bool IsStrangled { get; set; } = false;


    public bool IsInConversation => GetIsInConversation();
    public bool IsInConversationWithPlayer => ConversationTarget != null && ConversationTarget.IsPlayer();
    public bool IsInConversationWithNpc => ConversationTarget != null && ConversationTarget.IsNpc();
    public Transform ConversationTarget { get => _conversationTarget; set => _conversationTarget = value; }


    public NPCHumanCharacterID ID => GetComponent<NPCHumanCharacterInfo>().NPCHumanCharacterID;

    MvmntController mvmntController;
    BehaviorTree behaviorTree;
    CapsuleCollider capsuleCollider;
    NavMeshAgent navMeshAgent;
    Looker looker;

    private void Start()
    {
        NpcBehaviorBB.Instance.Register(this);
        
        mvmntController = GetComponent<MvmntController>();
        behaviorTree = GetComponent<BehaviorTree>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        looker = GetComponentInChildren<Looker>();

        var roomID = GetInitialRoomID();
        RoomBB.Instance.UpdateCharacterLocation(GetComponent<CharacterInfo>().ID, roomID);

        // lazy solution (:
        if (!TryGetComponent<CharacterSecretKnowledge>(out _))
            transform.AddComponent<CharacterSecretKnowledge>();
    }


    void Update()
    {
        //OtherUpdate();

        _currentRoom = RoomBB.Instance.GetCharacterRoomID(GetComponent<CharacterInfo>().ID);
    }


    private bool GetIsInConversation()
    {
        if (ConversationTarget == null)
            return false;

        if (IsInConversationWithPlayer)
            return true;

        if (IsInConversationWithNpc)
        {
            // Make sure the other NPC agrees that we are in a conversation
            var npcBrain = ConversationTarget.GetComponent<NpcBrain>();
            return npcBrain.ConversationTarget == transform;
        }

        return false;
    }


    

    /// <summary>
    /// Triggers a re-calculation of current behaviour tree. 
    /// Nice for when you expect some conditionals to change
    /// This could probably be written better, but it works.
    /// </summary>
    public void ReEvaluateTree()
    {
        behaviorTree.StopAllCoroutines();
        behaviorTree.StopAllTaskCoroutines();
        behaviorTree.ExternalBehavior = behaviorTree.ExternalBehavior;
        behaviorTree.enabled = false;
        behaviorTree.enabled = true;
        behaviorTree.Start();
    }

    private RoomID GetInitialRoomID()
    {
        var allRooms = FindObjectsByType<Room>(FindObjectsSortMode.None);
        allRooms = allRooms.OrderBy(r => r.socialScore).Reverse().ToArray();

        var currentRoom = allRooms.FirstOrDefault(x => x.PointIsInRoom(transform.position));

        if (currentRoom == null)
        {
            Debug.LogWarning($"Unable to figure out starting room for {gameObject.name}");
            return RoomID.Unknown;
        }

        return currentRoom.ID;
    }
}
