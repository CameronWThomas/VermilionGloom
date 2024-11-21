using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

[TaskCategory("Custom")]
public class FaceTarget : Action
{
    public SharedTransform Target;

    public override void OnStart()
    {
        GetComponent<MvmntController>().FaceTarget(Target.Value.position);
    }
}