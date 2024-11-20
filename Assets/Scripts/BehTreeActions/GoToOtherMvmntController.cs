
using BehaviorDesigner.Runtime.Tasks;
using Mono.Cecil.Cil;

[TaskCategory("Custom")]
public abstract class GoToOtherMvmntController : Action
{
    MvmntController _ourController;
    MvmntController _targetController;

    bool _targetReached = false;

    public override void OnStart()
    {
        _ourController = GetComponent<MvmntController>();
        _targetController = GetTargetController();

        _ourController.GoToTarget(_targetController.transform, () => _targetReached = true);
    }

    public override TaskStatus OnUpdate()
    {
        if (_targetReached)
            return TaskStatus.Success;

        return TaskStatus.Running;
    }

    protected abstract MvmntController GetTargetController();
}