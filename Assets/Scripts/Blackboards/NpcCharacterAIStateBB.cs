using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class NpcCharacterAIStateBB : GlobalSingleInstanceMonoBehaviour<NpcCharacterAIStateBB>
{
    private Dictionary<CharacterID, CharacterStateInfo> _stateDict = new();

    public void Register(NPCHumanCharacterID nPCHumanCharacterID, NPCHumanCharacterInfo characterInfo)
    {
        _stateDict.Add(nPCHumanCharacterID, CreateCharacterStateInfo(characterInfo));
    }    

    public bool TryFindCharacterInOurRoomNotInConversation(NPCHumanCharacterID callingId, out Transform characterTransform)
    {
        var activeRoom = _stateDict[callingId].Brain.activeRoom;

        var stateInfo = _stateDict.Where(x => x.Key != callingId && x.Value.Brain.activeRoom == activeRoom)
            .Randomize()
            .FirstOrDefault()
            .Value;

        characterTransform = stateInfo == null ? null : stateInfo.Transform;
        return characterTransform != null;
    }

    public bool TryStartingConversation(CharacterID id1, CharacterID id2)
    {
        var stateInfo1 = _stateDict[id1];
        var stateInfo2 = _stateDict[id2];

        if (stateInfo1.IsInConversation || stateInfo2.IsInConversation)
            return false;

        stateInfo1.SetInConversationCharacter(id2);
        stateInfo2.SetInConversationCharacter(id1);
        return true;
    }

    public void EndConversation(CharacterID id)
    {
        var stateInfo = _stateDict[id];

        if (!stateInfo.IsInConversation)
            return;

        var conversationCharacterId = stateInfo.InConversationCharacter;
        stateInfo.RemoveInConversationCharacter();

        EndConversation(conversationCharacterId);
    }

    public bool IsInConversation(CharacterID id)
    {
        return _stateDict[id].IsInConversation;
    }

    public Transform GetInConversationTransform(CharacterID id)
        => _stateDict[_stateDict[id].InConversationCharacter].Transform;

    private CharacterStateInfo CreateCharacterStateInfo(NPCHumanCharacterInfo characterInfo)
    {
        var stateInfo = new CharacterStateInfo();
        stateInfo.CharacterInfo = characterInfo;

        return stateInfo;
    }

    private class CharacterStateInfo
    {
        public NPCHumanCharacterInfo CharacterInfo;
        
        public CharacterID InConversationCharacter = null;

        public NpcBrain Brain => CharacterInfo.GetComponent<NpcBrain>();
        public Transform Transform => CharacterInfo.transform;
        public bool IsInConversation { get; private set; }

        public void SetInConversationCharacter(CharacterID id)
        {
            InConversationCharacter = id;
            IsInConversation = true;
        }

        public void RemoveInConversationCharacter()
        {
            InConversationCharacter = null;
            IsInConversation = false;
        }
    }
}