using BehaviorDesigner.Runtime.Tasks;

[TaskCategory("Custom/Conversation")]
public class FaceConversationTarget : Action
{
    public override TaskStatus OnUpdate()
    {
        if (!NpcBehaviorBB.Instance.IsInConversation(transform.GetCharacterID(), out var target))
            return TaskStatus.Failure;
        
        GetComponent<MvmntController>().FaceTarget(target.position);

        return TaskStatus.Success;
    }
}