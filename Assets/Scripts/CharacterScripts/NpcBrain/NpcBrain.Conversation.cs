using System;
using System.Collections;
using System.Linq;
using UnityEngine;

public enum ConversationState
{
    NA,
    WaitingForConversationTarget,
    GoingToConversationTarget,
    ReadyToConverse,
    ConversingWithTarget,
    ConversationEnding
}

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








    //private IEnumerator _lookingForConversationTargetCoroutine = null;

    //public ConversationState _conversationState;

    //public bool HasConversationTarget => IsInConversationState && ConvoTarget != null;
    //public bool IsConversingWithPlayer => HasConversationTarget && ConvoTarget.IsPlayer();
    //public bool IsConversingWithNpc => HasConversationTarget && ConvoTarget.IsNpc();

    //public NpcBrain NpcConvoTarget => HasConversationTarget ? ConvoTarget.GetComponent<NpcBrain>() : null;

    //public ConversationState ConversationState => IsInConversationState ? _conversationState : ConversationState.NA;


    ///// <summary>
    ///// Look for someone to talk to if not in a different state. Will reset the behaviour tree and put the state to <see cref="NpcState.None"/> on failure.
    ///// </summary>
    //public bool TryFindConversationTarget()
    //{
    //    if (State is NpcState.Conversation && HasConversationTarget)
    //        return true;

    //    return TryStartConversationWithNpcInRange();
    //}

    

    ///// <summary>
    ///// Use for when <paramref name="npcBrain"/> is trying to start the conversation (not this NPC)
    ///// </summary>
    //public bool TryEnterConversation(NpcBrain npcBrain)
    //{
    //    if (!TryUpdateState(NpcState.Conversation) || HasConversationTarget)
    //        return false;

    //    ConvoTarget = npcBrain.transform;
    //    _conversationState = ConversationState.WaitingForConversationTarget;

    //    // We want to go back and enter the conversation branch
    //    ReEvaluateTree();

    //    return true;
    //}

    ///// <summary>
    ///// When player tries to start the conversation with this NPC
    ///// </summary>
    //public void EnterConversationWithPlayer()
    //{
    //    if (IsConversingWithPlayer) return;

    //    if (HasConversationTarget)
    //        ExitConversation(ConvoTarget, false);

    //    ConvoTarget = PlayerStats.Instance.transform;
    //    GetReadyToConverse();

    //    ForceStateChange(NpcState.Conversation);
    //}

    //public void GetReadyToConverse()
    //{
    //    if (_conversationState is ConversationState.ReadyToConverse)
    //        return;

    //    _conversationState = ConversationState.ReadyToConverse;

    //    if (IsConversingWithNpc)
    //        NpcConvoTarget.GetReadyToConverse();
    //}    

    ///// <summary>
    ///// Exit the conversation with <paramref name="convoTargetToExitWith"/>
    ///// </summary>
    ///// <param name="convoTargetToExitWith"></param>
    //public void ExitConversation(Transform convoTargetToExitWith) => ExitConversation(convoTargetToExitWith, true);

    ///// <summary>
    ///// Exit the conversation
    ///// </summary>
    //public void ExitConversation() => ExitConversation(null, true);

    //public void ConverseWithTarget()
    //{
    //    if (ConversationState is ConversationState.ConversingWithTarget)
    //        return;

    //    _conversationState = ConversationState.ConversingWithTarget;
        
    //    if (IsConversingWithPlayer)
    //        UI_CharacterInteractionMenu.Instance.Activate(GetComponent<NPCHumanCharacterInfo>().NPCHumanCharacterID);
    //    else
    //        NpcConvoTarget.ConverseWithTarget();
    //}

    //private void ExitConversation(Transform convoTargetToExitWith, bool changeState)
    //{
    //    if (convoTargetToExitWith != null && ConvoTarget != convoTargetToExitWith)
    //        return;

    //    var npcConvoTarget = NpcConvoTarget;
    //    ConvoTarget = null;

    //    // If talking to an NPC, tell them to exit the conversation too
    //    if (npcConvoTarget != null)
    //        npcConvoTarget.ExitConversation(transform);

    //    //TODO do in behaviour tree
    //    animator.SetBool("conversing", false);

    //    if (changeState)
    //        ForceStateChange(NpcState.None);
    //}

    //private bool TryStartConversationWithNpcInRange()
    //{
    //    //if (!TryUpdateState(NpcState.Conversation))
    //    //    return false;

    //    var newConvoTarget = FindObjectsByType<NpcBrain>(FindObjectsSortMode.InstanceID)
    //        .Where(x => x != this && x.activeRoom == activeRoom)
    //        .FirstOrDefault(x => x.RequestConversation());

    //    if (newConvoTarget == null)
    //        return false;

    //    ForceStateChange(NpcState.Conversation, false);
    //    newConvoTarget.ForceStateChange(NpcState.Conversation, true);

    //    UpdateConvoTarget(newConvoTarget.transform);

    //    // They don't know the target yet
    //    //newConvoTarget.ConvoTarget = transform;        

    //    return true;
    //}

    //private bool RequestConversation() => State is NpcState.None;

    //private IEnumerator LookForConversationTarget()
    //{
    //    var looker = GetComponentInChildren<Looker>();

    //    var searchTime = _conversationSearchTime;
    //    var startTime = Time.time;
    //    while (Time.time - startTime < searchTime)
    //    {
    //        // Someone started a convo with us (:
    //        if (HasConversationTarget)
    //            yield break;

    //        // See if we see an NPC and can enter a conversation with them
    //        if (looker.TryFindNPC(out var brain) && brain.TryEnterConversation(this))
    //        {
    //            _conversationState = ConversationState.GoingToConversationTarget;
    //            ConvoTarget = brain.transform;
    //            yield break;
    //        }

    //        yield return new WaitForSeconds(Time.deltaTime);
    //    }

    //    TryLeaveState(NpcState.Conversation);
    //}

    //private enum ConversationState 
    //{
    //    NotConversing,
    //    LookingForConversationTarget,
    //    WaitingForConversationTarget,
    //    GoingToConversationTarget,
    //    ConversingWithTarget
    //}

    //private ConversationState _conversationState = ConversationState.NotConversing;


    //public bool HasConversationTarget => GetHasConversationTarget();
    //public bool IsSearchingForConversationTarget => _conversationState is ConversationState.LookingForConversationTarget;
    //public bool IsInConversation => _conversationState is ConversationState.ConversingWithTarget;
    //public bool IsWaitingForConversationTarget => _conversationState is ConversationState.WaitingForConversationTarget;







    //private Transform GetConvoTarget()
    //{
    //    if (!HasConversationTarget)
    //        return null;

    //    if (_isConversingWithPlayer)
    //        return PlayerStats.Instance.transform;

    //    return _npcConvoTarget.transform;
    //}


    //private bool GetHasConversationTarget()
    //    => _conversationState switch
    //    {
    //        ConversationState.GoingToConversationTarget
    //        or ConversationState.WaitingForConversationTarget
    //        or ConversationState.ConversingWithTarget => true,
    //        _ => false,
    //    };


}
