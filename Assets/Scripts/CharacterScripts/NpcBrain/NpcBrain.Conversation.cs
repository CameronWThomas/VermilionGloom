using System;
using System.Collections;
using System.Linq;
using UnityEngine;

public partial class NpcBrain
{
    [Header("Conversation")]
    public Transform ConvoTarget;
    public NpcBrain NpcConvoTarget => ConvoTarget == null ? null : ConvoTarget.GetComponent<NpcBrain>();

    //TODO if hostile this should be false
    public bool IsOpenToConversing { get; } = true;

    public bool HasConversationTarget => NpcConvoTarget != null && NpcConvoTarget.ConvoTarget == transform;


    /// <summary>
    /// Look for someone to talk to
    /// </summary>
    public bool TryFindConversationTarget(out MvmntController conversationTarget)
    {
        conversationTarget = null;

        if (ConvoTarget != null)
            return true;

        var npcBrains = FindObjectsByType<NpcBrain>(FindObjectsSortMode.InstanceID).ToList();
        var npcBrainsInRoom = npcBrains
            .Where(x => x != this && x.activeRoom == activeRoom)
            .ToList();

        conversationTarget = FindObjectsByType<NpcBrain>(FindObjectsSortMode.InstanceID)
            .Where(x => x != this && x.activeRoom == activeRoom)
            .FirstOrDefault(x => x.IsOpenToConversing)
            .GetComponent<MvmntController>();

        return conversationTarget != null;
    }

    public bool TryConverseWithTarget()
    {
        if (NpcConvoTarget == null)
            return false;

        return NpcConvoTarget.TryRequestConversation(this);
    }

    private bool TryRequestConversation(NpcBrain npcBrain)
    {
        if (ConvoTarget != null && ConvoTarget != npcBrain.transform)
            return false;

        ConvoTarget = npcBrain.transform;
        return true;
    }

    private void UpdateConvoTarget(Transform target)
    {
        ConvoTarget = target;
    }
}
