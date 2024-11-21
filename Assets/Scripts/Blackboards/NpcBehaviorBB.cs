using BehaviorDesigner.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;

public class NpcBehaviorBB : GlobalSingleInstanceMonoBehaviour<NpcBehaviorBB>
{
    private Dictionary<NPCHumanCharacterID, BehaviorTree> _behaviorDict = new();

    public void Register(NPCHumanCharacterID nPCHumanCharacterID, BehaviorTree behaviorTree)
    {
        _behaviorDict.Add(nPCHumanCharacterID, behaviorTree);
    }

    public Behavior GetBehavior(NPCHumanCharacterID characterID) => _behaviorDict[characterID];

    public List<(NPCHumanCharacterID, Behavior)> GetAll() => _behaviorDict.Select(x => (x.Key, x.Value as Behavior)).ToList();

}