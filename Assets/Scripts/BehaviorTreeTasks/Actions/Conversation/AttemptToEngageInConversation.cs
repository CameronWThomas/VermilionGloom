
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

[TaskCategory("Custom/Conversation")]
public class AttemptToEngageInConversation : Action
{
    public SharedTransform ConversationAttemptTarget;

    public override TaskStatus OnUpdate()
    {
        var targetBehaviorTree = ConversationAttemptTarget.Value.GetComponent<BehaviorTree>();
        if (targetBehaviorTree == null || targetBehaviorTree.IsInConversation())
            return TaskStatus.Failure;

        Owner.ConversationTarget(targetBehaviorTree.transform);
        targetBehaviorTree.ConversationTarget(transform);

        return TaskStatus.Success;
    }
}