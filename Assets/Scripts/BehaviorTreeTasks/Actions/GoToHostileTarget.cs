using BehaviorDesigner.Runtime.Tasks;

[TaskCategory("Custom")]
public class GoToHostileTarget : Action
{

    public override TaskStatus OnUpdate()
    {
        return TaskStatus.Failure;
    }
}