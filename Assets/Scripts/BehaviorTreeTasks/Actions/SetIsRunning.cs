using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

[TaskCategory("Custom")]
public class SetIsRunning : Action
{
    public SharedBool IsRunning;

    public override void OnStart()
    {
        GetComponent<MvmntController>().SetRunning(IsRunning.Value);
    }

    public override string FriendlyName => IsRunning.Value ? "Start running" : "Stop running";
}