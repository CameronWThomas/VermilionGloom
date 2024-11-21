using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

[TaskCategory("Custom/Conversation")]
public class GoToConversationTarget : GoToTarget
{
    CharacterID _conversationTargetID;

    public override void OnStart()
    {
        base.OnStart();

        _conversationTargetID = Target.Value.GetCharacterID();
    }

    public override TaskStatus OnUpdate()
    {
        if (NpcBehaviorBB.Instance.IsInConversation(_conversationTargetID))
        {
            GetComponent<MvmntController>().CancelMovementAction();
            return TaskStatus.Failure;
        }

        return base.OnUpdate();
    }
}