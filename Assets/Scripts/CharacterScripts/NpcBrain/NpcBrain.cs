using BehaviorDesigner.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;


[RequireComponent(typeof(NPCHumanCharacterInfo))]
public partial class NpcBrain : MonoBehaviour
{
    [SerializeField] Transform _conversationTarget = null;
    [SerializeField] RoomID _currentRoom;

    [SerializeField, Tooltip("pulbic for debugging purposes")] private List<Relationship> _relationships = new();

    public bool IsDead => GetComponent<CharacterInfo>().IsDead;
    public GameObject Dragger { get; private set; } = null;
    public bool IsBeingDragged => Dragger != null;

    public GameObject Strangler { get; private set; } = null;
    public bool IsBeingStrangled => Strangler != null;
    public bool IsStrangled { get; private set; } = false;


    public bool IsInConversation => GetIsInConversation();
    public bool IsInConversationWithPlayer => ConversationTarget != null && ConversationTarget.IsPlayer();
    public bool IsInConversationWithNpc => ConversationTarget != null && ConversationTarget.IsNpc();
    public Transform ConversationTarget { get => _conversationTarget; set => _conversationTarget = value; }

    public RoomID CurrentRoom => _currentRoom;


    public NPCHumanCharacterID ID => GetComponent<NPCHumanCharacterInfo>().NPCHumanCharacterID;

    MvmntController mvmntController;
    BehaviorTree behaviorTree;
    CapsuleCollider capsuleCollider;
    NavMeshAgent navMeshAgent;
    Looker looker;
    CharacterSecretKnowledge characterSecretKnowledge;

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
        characterSecretKnowledge = GetComponent<CharacterSecretKnowledge>();
    }


    void Update()
    {
        _currentRoom = RoomBB.Instance.GetCharacterRoomID(GetComponent<CharacterInfo>().ID);

        if (looker.TryGetCharactersInSight(out var characters))
            AddSecretsForDeadCharacters(characters);

        HostilityUpdate();
    }

    public bool CanSeeTarget(CharacterID targetCharacter)
    {
        if (!FindCharactersInSight(out var characters))
            return false;

        return characters.Any(x => x.ID == targetCharacter);
    }

    // STRANGLE
    public void BeStrangled(GameObject killer)
    {
        Strangler = killer;
    }

    public void StopBeingStrangled()
    {
        BeStrangled(null);
    }

    public void StrangleDie(bool setAnimParam = true)
    {
        IsStrangled = true;
        StopBeingStrangled();
    }

    // DRAG
    public void BeDraged(GameObject dragger)
    {
        Dragger = dragger;
    }

    public void StopBeingDragged()
    {
        BeDraged(null);
    }


    //TODO move to looker
    private bool FindCharactersInSight(out List<CharacterInfo> characters)
    {
        // Don't see anyoneRoomCheck
        if (!looker.TryGetCharactersInSight(out characters))
            return false;

        // Make sure they are in the same room with us
        // TODO may not always want this. May do that foot raycast thing
        characters = characters.Where(x => RoomCheck(RoomBB.Instance.GetCharacterRoomID(x.ID))).ToList();

        return characters.Any();
    }

    private bool RoomCheck(RoomID otherRoom)
    {
        return otherRoom == CurrentRoom || otherRoom is RoomID.Unknown;
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

    private Relationship GetRelationship(CharacterID character)
    {
        if (!_relationships.Any(x => x.RelationshipTarget == character))
            _relationships.Add(new Relationship(characterSecretKnowledge, character));

        return _relationships.First(x => x.RelationshipTarget == character);
    }

    private void HandleSeeingCharacters(List<CharacterInfo> charactersInSight)
    {
        foreach (var character in charactersInSight)
        {
            if (character is not PlayerCharacterInfo playerCharacter)
                continue;


        }
    }

    /// <summary>
    /// NOTE: Do NOT actually use this method
    /// 
    /// Triggers a re-calculation of current behaviour tree. 
    /// Nice for when you expect some conditionals to change
    /// This could probably be written better, but it works.
    /// </summary>
    private void ReEvaluateTree()
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
