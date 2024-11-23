using BehaviorDesigner.Runtime.Tasks;

[TaskCategory("Custom")]
public class FaceHostileTarget : Action
{
    public override TaskStatus OnUpdate()
    {
        var ourBrain = GetComponent<NpcBrain>();
        if (ourBrain.HostileTowardsTarget == null)
            return TaskStatus.Failure;
        
        GetComponent<MvmntController>().FaceTarget(ourBrain.HostileTowardsTarget.position);

        return TaskStatus.Success;
    }
}