using System;
using System.Collections.Generic;
using UnityEngine;

public class NpcBehaviorBB : GlobalSingleInstanceMonoBehaviour<NpcBehaviorBB>
{
    private Dictionary<CharacterID, INpcCharacterBehaviorHelper> _npcBehaviourInfoDict = new();
    private PlayerCharacterBehaviorInfo _playerCharacterInfo = new();

    public void Register(NPCHumanCharacterID nPCHumanCharacterID, NPCHumanCharacterInfo characterInfo)
    {
        var behaviorInfo = characterInfo.gameObject.AddComponent<CharacterBehaviorInfo>();
        _npcBehaviourInfoDict.Add(nPCHumanCharacterID, behaviorInfo);
    }

    public ICharacterBehaviorHelper GetBehaviorInfo(CharacterID characterID) => GetBehaviorInfoInternal(characterID);    

    public bool TryStartingConversation(CharacterID id1, CharacterID id2)
    {
        var behaviourInfo1 = GetBehaviorInfoInternal(id1);
        var behaviourInfo2 = GetBehaviorInfoInternal(id2);

        if (behaviourInfo1.IsInConversation || behaviourInfo2.IsInConversation)
            return false;

        behaviourInfo1.UpdateConversationTarget(behaviourInfo2.CharacterInfo);
        behaviourInfo2.UpdateConversationTarget(behaviourInfo1.CharacterInfo);
        
        return true;
    }

    public void EndConversation(CharacterID id) => EndConversation(id, false);

    public bool IsInConversation(CharacterID id) => _npcBehaviourInfoDict[id].IsInConversation;

    public void EnterConversationWithPlayer(NPCHumanCharacterID characterID)
    {
        var character = GetNpcBehaviorInfo(characterID);
        if (character.IsInConversation)
        {
            var conversationTargetId = character.ConversationTargetID;
            EndConversation(characterID);

            if (GetBehaviorInfoInternal(conversationTargetId) is INpcCharacterBehaviorHelper conversationCharacter)
                conversationCharacter.NpcBrain.ReEvaluateTree();
            character.NpcBrain.ReEvaluateTree();
        }


        var npcInfo = GetBehaviorInfoInternal(characterID);
        var playerInfo = _playerCharacterInfo;

        npcInfo.UpdateConversationTarget(playerInfo.CharacterInfo);
        playerInfo.UpdateConversationTarget(npcInfo.CharacterInfo);
    }

    public void EndConversationWithPlayer() => EndConversation(_playerCharacterInfo.CharacterID, true);

    private void EndConversation(CharacterID id, bool allowEndPlayerConversation)
    {
        var behaviourInfo = GetBehaviorInfoInternal(id);

        var playerCondition = allowEndPlayerConversation || (behaviourInfo != _playerCharacterInfo && behaviourInfo.ConversationTargetID != _playerCharacterInfo.CharacterID);
        if (!behaviourInfo.IsInConversation || !playerCondition)
            return;

        var conversationCharacterId = behaviourInfo.ConversationTargetID;
        behaviourInfo.EndConversation();

        EndConversation(conversationCharacterId, allowEndPlayerConversation);
    }

    private ICharacterBehaviorHelperEnhanced GetBehaviorInfoInternal(CharacterID characterID)
    {
        if (characterID is PlayerCharacterID)
            return _playerCharacterInfo;

        return GetNpcBehaviorInfo(characterID);
    }

    private INpcCharacterBehaviorHelper GetNpcBehaviorInfo(CharacterID characterID) => _npcBehaviourInfoDict[characterID];

    private class PlayerCharacterBehaviorInfo : ICharacterBehaviorHelperEnhanced
    {
        public CharacterID CharacterID => Transform.GetCharacterID();

        public bool IsInConversation { get; private set; }

        public CharacterInfo ConversationTarget { get; private set; }

        public CharacterID ConversationTargetID => IsInConversation && ConversationTarget != null ? ConversationTarget.ID : null;

        public RoomID CurrentRoom => RoomBB.Instance.GetCharacterRoomID(CharacterID);

        public Transform Transform => PlayerStats.Instance.transform;

        public CharacterInfo CharacterInfo => Transform.GetComponent<CharacterInfo>();

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
}