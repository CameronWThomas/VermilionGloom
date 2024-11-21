using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using System.Collections.Generic;
using System.Linq;

[TaskCategory("Custom/Conversation")]
[TaskDescription("Indefinitely runs until it finds another NPC Human in the room that this NPC can talk to")]
public class FindConversationTarget : Action
{
    public SharedTransform OutConversationTarget;

    NpcBrain _ourBrain;
    List<NpcBrain> _otherNpcBrains;

    public override void OnStart()
    {
        _ourBrain = GetComponent<NpcBrain>();

        var ourCharacterId = GetComponent<NPCHumanCharacterInfo>().NPCHumanCharacterID;
        var otherNpcIds = CharacterInfoBB.Instance.GetAll()
            .OfType<NPCHumanCharacterInfo>()
            .Where(x => x.NPCHumanCharacterID != ourCharacterId);
        _otherNpcBrains = otherNpcIds.Select(x => x.GetComponent<NpcBrain>()).ToList();
    }

    public override TaskStatus OnUpdate()
    {
        var npcsInSameRoom = _otherNpcBrains.Where(x => x.activeRoom == _ourBrain.activeRoom).Randomize();
        var npcNotInConversation = npcsInSameRoom
            .Select(x => NpcBehaviorBB.Instance.GetBehavior(x.GetComponent<NPCHumanCharacterInfo>().NPCHumanCharacterID))
            .FirstOrDefault(x => !x.IsInConversation());

        if (npcNotInConversation != null)
        {
            OutConversationTarget.Value = npcNotInConversation.transform;
            return TaskStatus.Success;
        }

        return TaskStatus.Running;
    }
}