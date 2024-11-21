using BehaviorDesigner.Runtime.Tasks;

[TaskCategory("Custom")]
public class IsDead : Conditional
{
    public override TaskStatus OnUpdate()
    {
        var taskStatus = NpcBehaviorBB.Instance.IsDead(transform.GetCharacterID())
            ? TaskStatus.Success
            : TaskStatus.Failure;

        return taskStatus;
    }

    public override float GetPriority() => PriorityTier.Max.Priority();
}