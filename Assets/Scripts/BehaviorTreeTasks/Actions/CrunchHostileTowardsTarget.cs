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
        _taskStatus = TaskStatus.Running;
        _npcBrain = GetComponent<NpcBrain>();

        _npcBrain.Crunch(() => _taskStatus = TaskStatus.Success, () => _taskStatus = TaskStatus.Failure);
    }

    public override TaskStatus OnUpdate() => _taskStatus;
}
