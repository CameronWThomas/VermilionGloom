using BehaviorDesigner.Runtime.Tasks;

[TaskCategory("Custom")]
public class SeesHostileTowardsTarget : Conditional
{
    public SharedPriorityTierVariable Priority = new() { Value = PriorityTier.Tier2 };

    NpcBrain _ourBrain;

    public override void OnStart()
    {
        _ourBrain = transform.GetComponent<NpcBrain>();
    }

    public override TaskStatus OnUpdate()
    {
        if (_ourBrain.SeesAHostileTowardsTarget(out _))
            return TaskStatus.Success;

        return TaskStatus.Failure;
    }

    public override float GetPriority() => Priority.Value.Priority();
}