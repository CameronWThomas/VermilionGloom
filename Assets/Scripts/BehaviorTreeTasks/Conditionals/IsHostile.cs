using BehaviorDesigner.Runtime.Tasks;

[TaskCategory("Custom")]
public class IsHostile : Conditional
{
    NpcBrain _ourBrain;

    public override void OnStart()
    {
        _ourBrain = transform.GetComponent<NpcBrain>();
    }

    public override TaskStatus OnUpdate()
    {
        if (_ourBrain.IsHostile)
            return TaskStatus.Success;

        return TaskStatus.Failure;
    }

    public override float GetPriority() => PriorityTier.Tier4.Priority();
}