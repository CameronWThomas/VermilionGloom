using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using System.Linq;

[TaskCategory("Custom/Conversation")]
public class FindConversationTarget : Action
{
    public SharedTransform OutConversationTarget;

    NPCHumanCharacterID _conversationTargetID = null;

    public override void OnStart()
    {
        var charactersInRoom = RoomBB.Instance.GetCharactersInMyRoom(transform.GetCharacterID()).ToList();
        var npcCharactersInRoom = charactersInRoom.OfType<NPCHumanCharacterID>().ToList();
        var notInConvo = npcCharactersInRoom.Where(x => !NpcBehaviorBB.Instance.IsInConversation(x)).ToList();

        _conversationTargetID = notInConvo.Randomize().FirstOrDefault();

        //_conversationTargetID = RoomBB.Instance.GetCharactersInMyRoom(transform.GetCharacterID())
        //    .OfType<NPCHumanCharacterID>()
        //    .Where(x => !NpcBehaviorBB.Instance.IsInConversation(x))
        //    .Randomize()
        //    .FirstOrDefault();
    }

    public override TaskStatus OnUpdate()
    {
        if (_conversationTargetID != null)
        {
            var behaviourInfo = NpcBehaviorBB.Instance.GetBehaviorInfo(_conversationTargetID);
            OutConversationTarget.Value = behaviourInfo.transform;
            return TaskStatus.Success;
        }

        return TaskStatus.Running;
    }
}