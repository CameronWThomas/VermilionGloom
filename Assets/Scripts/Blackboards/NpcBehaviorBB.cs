using System;
using System.Collections.Generic;
using UnityEngine;

public class NpcBehaviorBB : GlobalSingleInstanceMonoBehaviour<NpcBehaviorBB>
{
    private Dictionary<NPCHumanCharacterID, NpcBrain> _npcBrains = new();

    public void Register(NpcBrain brain)
    {
        _npcBrains.Add(brain.ID, brain);
    }

    public bool IsDead(CharacterID characterID) => GetBrain(characterID).IsDead;
    public bool IsBeingStrangled(CharacterID characterID) => GetBrain(characterID).IsBeingStrangled;
    public bool IsStrangled(CharacterID characterID) => GetBrain(characterID).IsStrangled;

    public bool IsDragged(CharacterID characterID) => GetBrain(characterID).IsBeingDragged;


    public void StrangleDie(NPCHumanCharacterID characterId)
    {
        var brain = GetBrain(characterId);
        brain.GetComponent<NPCHumanCharacterInfo>().Die();
        brain.StopBeingStrangled();
    }

    public bool TryStartingConversation(CharacterID id1, CharacterID id2)
    {
        var brain1 = GetBrain(id1);
        var brain2 = GetBrain(id2);

        // If either is conversing, we can't start a conversation
        if (brain1.IsInConversation || brain1.IsInConversation)
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

    public void EnterConversationWithPlayer(NPCHumanCharacterID characterID)
    {
        var brain = GetBrain(characterID);
        if (brain.IsInConversationWithPlayer)
            return;
        
        if (brain.IsInConversationWithNpc)
        {
            EndConversation(characterID);

            // This brain doesnt get IsInConversation reevaluated, so we need to reevaluate the tree
            brain.ReEvaluateTree();
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
}