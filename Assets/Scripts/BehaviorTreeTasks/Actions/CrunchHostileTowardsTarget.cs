using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

[TaskCategory("Custom")]
public class CrunchHostileTowardsTarget : Action
{
    NpcBrain _npcBrain;
    TaskStatus _taskStatus;

    public override void OnStart()
    {
        //mvmntController = GetComponent<MvmntController>();
        //mvmntController.SetRunning(true);

        _taskStatus = TaskStatus.Running;
        _npcBrain = GetComponent<NpcBrain>();
        
        _npcBrain.Crunch(OnCrunchEnd);

    }

    public override TaskStatus OnUpdate() => _taskStatus;

    public void OnCrunchEnd(bool successful)
    {
        _taskStatus = successful ? TaskStatus.Success : TaskStatus.Failure;
    }
}
