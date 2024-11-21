using BehaviorDesigner.Runtime.Tasks;

[TaskCategory("Custom/Conversation")]
public class FaceConversationTarget : Action
{
    public override TaskStatus OnUpdate()
    {
        var ourBehaviourInfo = NpcBehaviorBB.Instance.GetBehaviorInfo(transform.GetCharacterID());

        if (ourBehaviourInfo.ConversationTarget == null)
            return TaskStatus.Failure;

        var targetTransform = ourBehaviourInfo.ConversationTarget.transform;
        GetComponent<MvmntController>().FaceTarget(targetTransform.position);

        return TaskStatus.Success;
    }
}