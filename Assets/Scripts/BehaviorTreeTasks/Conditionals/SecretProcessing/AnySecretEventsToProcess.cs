using BehaviorDesigner.Runtime.Tasks;
using System.Linq;

[TaskCategory("Custom/Secret Processing")]
public class AnySecretEventsToProcess : Conditional
{
    NpcBrain _brain;

    public override void OnStart()
    {
        _brain = GetComponent<NpcBrain>();
    }

    public override TaskStatus OnUpdate()
    {
        if (_brain.AnyNewSecretEvents())
            return TaskStatus.Success;

        return TaskStatus.Failure;
    }

    public override float GetPriority() => PriorityTier.Tier2.Priority();

}