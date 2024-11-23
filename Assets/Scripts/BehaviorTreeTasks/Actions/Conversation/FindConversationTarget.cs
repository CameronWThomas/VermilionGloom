using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using System.Linq;
using UnityEngine;

[TaskCategory("Custom/Conversation")]
public class FindConversationTarget : Action
{
    public SharedTransform OutConversationTarget;

    Transform _conversationTargetTransform = null;

    public override void OnStart()
    {
        _conversationTargetTransform = null;

        var possibleConversationTargets = RoomBB.Instance.GetCharactersInMyRoom(transform.GetCharacterID())
            .OfType<NPCHumanCharacterID>()
            .Randomize()
            .ToList();

        foreach (var characterId in possibleConversationTargets)
        {
            if (!NpcBehaviorBB.Instance.IsInConversation(characterId, out var _))
            {
                _conversationTargetTransform = CharacterInfoBB.Instance.GetCharacterInfo(characterId).transform;
                break;
            }
        }
    }

    public override TaskStatus OnUpdate()
    {
        if (_conversationTargetTransform != null)
        {
            OutConversationTarget.Value = _conversationTargetTransform;
            return TaskStatus.Success;
        }

        return TaskStatus.Running;
    }
}