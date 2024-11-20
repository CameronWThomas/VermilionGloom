using System;
using System.Collections;
using UnityEngine;

public partial class NpcBrain
{
    [SerializeField] private float _conversationSearchTime = 2f;

    public Transform ConvoTarget => GetConvoTarget();    

    private NpcBrain _npcConvoTarget = null;
    private bool _isConversingWithPlayer = false;
    private IEnumerator _lookingForConversationTargetCoroutine = null;


    private enum ConversationState 
    {
        NotConversing,
        LookingForConversationTarget,
        WaitingForConversationTarget,
        GoingToConversationTarget,
        ConversingWithTarget
    }

    private ConversationState _conversationState = ConversationState.NotConversing;


    public bool HasConversationTarget => GetHasConversationTarget();
    public bool IsSearchingForConversationTarget => _conversationState is ConversationState.LookingForConversationTarget;
    public bool IsInConversation => _conversationState is ConversationState.ConversingWithTarget;
    public bool IsWaitingForConversationTarget => _conversationState is ConversationState.WaitingForConversationTarget;
    
    /// <summary>
    /// Use for when <paramref name="npcBrain"/> is trying to start the conversation (not this NPC)
    /// </summary>
    public bool TryEnterConversation(NpcBrain npcBrain)
    {
        if (HasConversationTarget)
            return false;

        _npcConvoTarget = npcBrain;
        _conversationState = ConversationState.WaitingForConversationTarget;

        return true;
    }

    /// <summary>
    /// When player tries to start the conversation with this NPC
    /// </summary>
    public void EnterConversationWithPlayer()
    {
        if (_isConversingWithPlayer) return;

        if (HasConversationTarget)
            ExitConversation();

        _isConversingWithPlayer = true;
        _conversationState = ConversationState.WaitingForConversationTarget;
    }

    public void FindConversationTarget()
    {
        if (IsSearchingForConversationTarget)
            return;

        _conversationState = ConversationState.LookingForConversationTarget;
        _lookingForConversationTargetCoroutine = LookForConversationTarget();
        StartCoroutine(_lookingForConversationTargetCoroutine);
    }    

    public void StartConversation()
    {
        _conversationState = ConversationState.ConversingWithTarget;

        GetComponent<MvmntController>().FaceTarget(_isConversingWithPlayer ? PlayerStats.Instance.transform.position : _npcConvoTarget.transform.position);
        animator.SetBool("conversing", true);

        if (_isConversingWithPlayer)
            UI_CharacterInteractionMenu.Instance.Activate(GetComponent<NPCHumanCharacterInfo>().NPCHumanCharacterID);
        else if (!_npcConvoTarget.IsInConversation)
            _npcConvoTarget.StartConversation();
    }

    public void ExitConversation()
    {
        _conversationState = ConversationState.NotConversing;
        animator.SetBool("conversing", false);
        ReEvaluateTree();
    }

    private Transform GetConvoTarget()
    {
        if (!HasConversationTarget)
            return null;

        if (_isConversingWithPlayer)
            return PlayerStats.Instance.transform;

        return _npcConvoTarget.transform;
    }


    private bool GetHasConversationTarget()
        => _conversationState switch
        {
            ConversationState.GoingToConversationTarget
            or ConversationState.WaitingForConversationTarget
            or ConversationState.ConversingWithTarget => true,
            _ => false,
        };

    private IEnumerator LookForConversationTarget()
    {
        var looker = GetComponentInChildren<Looker>();

        var searchTime = _conversationSearchTime;
        var startTime = Time.time;
        while (Time.time - startTime < searchTime)
        {
            // Someone started a convo with us (:
            if (HasConversationTarget)
                yield break;

            // See if we see an NPC and can enter a conversation with them
            if (looker.TryFindNPC(out var brain) && brain.TryEnterConversation(this))
            {
                _conversationState = ConversationState.GoingToConversationTarget;
                _npcConvoTarget = brain;
                yield break;
            }

            yield return new WaitForSeconds(Time.deltaTime);
        }

        _conversationState = ConversationState.NotConversing;
    }
}
