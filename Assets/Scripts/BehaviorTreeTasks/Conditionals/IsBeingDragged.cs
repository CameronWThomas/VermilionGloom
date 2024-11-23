using BehaviorDesigner.Runtime.Tasks;

[TaskCategory("Custom")]
public class IsBeingDragged : Conditional
{
    public override TaskStatus OnUpdate()
    {
        var taskStatus = NpcBehaviorBB.Instance.IsDragged(transform.GetCharacterID())
            ? TaskStatus.Success
            : TaskStatus.Failure;

        return taskStatus;
    }

    public override float GetPriority() => PriorityTier.Max.Priority();
}