using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

[TaskCategory("Custom")]
public class MoveToRandomOtherRoom : Action
{
    MvmntController _mvmntController;
    
    TaskStatus _taskStatus = TaskStatus.Running;
    public override void OnStart()
    {
        _mvmntController = GetComponent<MvmntController>();
        _mvmntController.SetRunning(false);

        _taskStatus = TaskStatus.Running;

        var otherRoom = RoomBB.Instance.GetRandomOtherRoom(transform.GetCharacterID());

        if (otherRoom == null)
            _taskStatus = TaskStatus.Failure;
        else 
            _mvmntController.GoToTarget(otherRoom.GetRandomPointInRoom(), () => _taskStatus = TaskStatus.Success, () => _taskStatus = TaskStatus.Failure);
    }

    public override TaskStatus OnUpdate() => _taskStatus;

    public override void OnConditionalAbort() => _mvmntController.CancelMovementAction();

}
