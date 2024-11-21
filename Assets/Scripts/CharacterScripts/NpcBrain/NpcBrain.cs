using BehaviorDesigner.Runtime;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NPCHumanCharacterInfo))]
public partial class NpcBrain : MonoBehaviour
{
    MvmntController mvmntController;
    Animator animator;
    BehaviorTree behaviorTree;
    CapsuleCollider capsuleCollider;
    NavMeshAgent navMeshAgent;
    Looker looker;

    private void Start()
    {
        animator = GetComponent<Animator>();
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
