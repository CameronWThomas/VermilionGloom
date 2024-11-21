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
    NPCHumanCharacterID _ourId;

    public override void OnStart()
    {
        _ourBrain = GetComponent<NpcBrain>();

        _ourId = GetComponent<NPCHumanCharacterInfo>().NPCHumanCharacterID;
    }

    public override TaskStatus OnUpdate()
    {
        if (NpcCharacterAIStateBB.Instance.TryFindCharacterInOurRoomNotInConversation(_ourId, out var characterTransform))
        {
            OutConversationTarget.Value = characterTransform;
            return TaskStatus.Success;
        }

        return TaskStatus.Running;
    }
}