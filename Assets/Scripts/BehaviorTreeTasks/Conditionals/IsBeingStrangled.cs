using BehaviorDesigner.Runtime.Tasks;

[TaskCategory("Custom")]
public class IsBeingStrangled : Conditional
{
    public override TaskStatus OnUpdate()
    {
        var taskStatus = NpcBehaviorBB.Instance.IsBeingStrangled(transform.GetCharacterID())
            ? TaskStatus.Success
            : TaskStatus.Failure;

        return taskStatus;
    }

    public override float GetPriority() => PriorityTier.Max.Priority();
}