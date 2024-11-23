using BehaviorDesigner.Runtime.Tasks;

[TaskCategory("Custom/Conversation")]
public class IsInConversationWithPlayer : Conditional
{
    public override TaskStatus OnUpdate()
    {
        return NpcBehaviorBB.Instance.IsInConversationWithPlayer(GetComponent<CharacterInfo>().ID)
            ? TaskStatus.Success
            : TaskStatus.Failure; ;
    }

    public override float GetPriority() => PriorityTier.Tier1.Priority();
}