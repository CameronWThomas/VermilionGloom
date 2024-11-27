using BehaviorDesigner.Runtime.Tasks;

[TaskCategory("Custom/Secret Processing")]
public class NewSecretEventNoticed : Conditional
{
    NpcBrain _brain;

    int _secretsToProcessCount = 0;

    public override void OnStart()
    {
        _brain = GetComponent<NpcBrain>();
        _secretsToProcessCount = 0;
    }

    public override TaskStatus OnUpdate()
    {
        var lastCount = _secretsToProcessCount;
        _secretsToProcessCount = _brain.NewSecretEvents.Count;

        if (lastCount != _secretsToProcessCount)
            return TaskStatus.Success;

        return TaskStatus.Failure;
    }

    public override float GetPriority() => PriorityTier.Tier3.Priority();

}