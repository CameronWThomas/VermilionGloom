using System;
using System.Collections.Generic;

public class NpcBehaviorBB : GlobalSingleInstanceMonoBehaviour<NpcBehaviorBB>
{
    private Dictionary<CharacterID, CharacterBehaviorInfo> _stateDict = new();

    public void Register(NPCHumanCharacterID nPCHumanCharacterID, NPCHumanCharacterInfo characterInfo)
    {
        var behaviorInfo = characterInfo.gameObject.AddComponent<CharacterBehaviorInfo>();
        _stateDict.Add(nPCHumanCharacterID, behaviorInfo);
    }

    public CharacterBehaviorInfo GetBehaviorInfo(CharacterID characterID) => _stateDict[characterID];

    public bool TryStartingConversation(CharacterID id1, CharacterID id2)
    {
        var behaviourInfo1 = GetBehaviorInfo(id1);
        var behaviourInfo2 = GetBehaviorInfo(id2);

        if (behaviourInfo1.IsInConversation || behaviourInfo2.IsInConversation)
            return false;

        behaviourInfo1.UpdateConversationTarget(behaviourInfo2.CharacterInfo);
        behaviourInfo2.UpdateConversationTarget(behaviourInfo1.CharacterInfo);
        
        return true;
    }

    public void EndConversation(CharacterID id)
    {
        var behaviourInfo = _stateDict[id];

        if (!behaviourInfo.IsInConversation)
            return;

        var conversationCharacterId = behaviourInfo.ConversationTargetID;
        behaviourInfo.EndConversation();

        EndConversation(conversationCharacterId);
    }

    public bool IsInConversation(CharacterID id) => _stateDict[id].IsInConversation;
}