using BehaviorDesigner.Runtime.Tasks;
using System.Linq;

[TaskCategory("Custom/Secret Processing")]
public class ProcessSecretEvent : Action
{
    protected NpcBrain _brain;

    public override void OnStart()
    {
        _brain = GetComponent<NpcBrain>();
    }

    public override TaskStatus OnUpdate()
    {
        _brain.ProcessSecretEvent();

        if (_brain.LastSecretEventResponse == null)
            return TaskStatus.Failure;

        return TaskStatus.Success;
    }
}