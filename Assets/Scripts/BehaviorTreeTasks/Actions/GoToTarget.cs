using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

[TaskCategory("Custom")]
public class GoToTarget : Action
{
    public SharedTransform Target;

    bool _hasReachedTarget;

    public override void OnStart()
    {
        _hasReachedTarget = false;
        GetComponent<MvmntController>().GoToTarget(Target.Value, () => _hasReachedTarget = true);
    }

    public override TaskStatus OnUpdate() => _hasReachedTarget ? TaskStatus.Success : TaskStatus.Running;

    public override void OnConditionalAbort()
    {
        GetComponent<MvmntController>().CancelMovementAction();
    }
}