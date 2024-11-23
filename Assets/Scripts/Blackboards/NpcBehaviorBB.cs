using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NpcBehaviorBB : GlobalSingleInstanceMonoBehaviour<NpcBehaviorBB>
{
    private Dictionary<NPCHumanCharacterID, NpcBrain> _npcBrains = new();

    List<SecretEvent> _broadcastingSecretEvents = new();

    private void Update()
    {
        if (!_broadcastingSecretEvents.Any())
            return;

        var secretEvents = _broadcastingSecretEvents.ToList();
        foreach (var brain in _npcBrains.Values)
        {
            brain.ReceiveBroadcast(secretEvents);
        }

        var instantSecretEvents = secretEvents.Where(x => x.SecretDuration is SecretDuration.Instant).ToList();
        foreach (var instantSecretEvent in instantSecretEvents)
            _broadcastingSecretEvents.Remove(instantSecretEvent);
    }

    public void Register(NpcBrain brain)
    {
        _npcBrains.Add(brain.ID, brain);
    }

    public bool IsDead(CharacterID characterID) => GetBrain(characterID).IsDead;
    public bool IsBeingStrangled(CharacterID characterID) => GetBrain(characterID).IsBeingStrangled;
    public bool IsStrangled(CharacterID characterID) => GetBrain(characterID).IsStrangled;
    public bool IsDragged(CharacterID characterID) => GetBrain(characterID).IsBeingDragged;

    public void BroadcastSecretEvent(SecretEvent secretEvent)
    {
        if (secretEvent == null || _broadcastingSecretEvents.Contains(secretEvent)) return;

        _broadcastingSecretEvents.Add(secretEvent);
    }

    public void EndSecretEventBroadcast(SecretEvent secretEvent)
    {
        if (_broadcastingSecretEvents.Contains(secretEvent))
            _broadcastingSecretEvents.Remove(secretEvent);
    }

    public void StrangleDie(NPCHumanCharacterID characterId)
    {
        var brain = GetBrain(characterId);
        brain.StrangleDie();
    }

    public bool TryStartingConversation(CharacterID id1, CharacterID id2)
    {
        var brain1 = GetBrain(id1);
        var brain2 = GetBrain(id2);

        // If either is conversing, we can't start a conversation
        if (!IsAllowedToConverse(brain1) || !IsAllowedToConverse(brain2))
            return false;

        brain1.ConversationTarget = brain2.transform;
        brain2.ConversationTarget = brain1.transform;
        
        return true;
    }    

    public void EndConversation(CharacterID id)
    {
        var brain = GetBrain(id);
        if (!brain.IsInConversation || brain.IsInConversationWithPlayer)
            return;

        var conversationTarget = brain.ConversationTarget;        
        brain.ConversationTarget = null;

        EndConversation(conversationTarget.GetCharacterID());
    }

    public bool IsInConversation(CharacterID id, out Transform conversationTarget)
    {
        var brain = GetBrain(id);
        conversationTarget = brain.ConversationTarget;
        return brain.IsInConversation;            
    }

    public bool IsInConversationWithPlayer(CharacterID id)
    {
        var brain = GetBrain(id);
        return brain.IsInConversationWithPlayer;
    }

    public void EnterConversationWithPlayer(NPCHumanCharacterID characterID)
    {
        var brain = GetBrain(characterID);
        if (brain.IsInConversationWithPlayer)
            return;
        
        if (brain.IsInConversationWithNpc)
        {
            EndConversation(characterID);

            // This brain doesnt get IsInConversation reevaluated, so we need to reevaluate the tree. This is bad, we should not do this...
            //brain.ReEvaluateTree();
        }


        brain.ConversationTarget = PlayerStats.Instance.transform;
    }

    public void EndConversationWithPlayer(NPCHumanCharacterID characterID)
    {
        var brain = GetBrain(characterID);

        if (brain.IsInConversationWithPlayer)
            brain.ConversationTarget = null;
    }

    private NpcBrain GetBrain(CharacterID characterID)
    {
        if (characterID is NPCHumanCharacterID npcId)
            return _npcBrains[npcId];

        return null;
    }

    private bool IsAllowedToConverse(NpcBrain brain)
    {
        return !brain.IsInConversation && !brain.IsDead && !brain.IsBeingStrangled && !brain.IsStrangled;
    }
}