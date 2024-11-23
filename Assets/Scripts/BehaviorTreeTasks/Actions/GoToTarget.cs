using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

[TaskCategory("Custom")]
public class GoToTarget : Action
{
    public SharedTransform Target;

    TaskStatus _taskStatus = TaskStatus.Running;

    public override void OnStart()
    {
        _taskStatus = TaskStatus.Running;
        GetComponent<MvmntController>().GoToTarget(Target.Value, () => _taskStatus = TaskStatus.Success, () => _taskStatus = TaskStatus.Failure);
    }

    public override TaskStatus OnUpdate() => _taskStatus;

    public override void OnConditionalAbort() => GetComponent<MvmntController>().CancelMovementAction();
}