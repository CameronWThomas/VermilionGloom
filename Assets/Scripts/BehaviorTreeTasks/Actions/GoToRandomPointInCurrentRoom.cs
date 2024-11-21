using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

[TaskCategory("Custom")]
public class GoToRandomPointInCurrentRoom : Action
{
    MvmntController _mvmntController;
    TaskStatus _taskStatus = TaskStatus.Running;

    Vector3 targetPosition;
    public override void OnStart()
    {
        _mvmntController = GetComponent<MvmntController>();
        _mvmntController.SetRunning(false);
        
        _taskStatus = TaskStatus.Running;

        var currentRoom = RoomBB.Instance.GetCharacterRoom(transform.GetCharacterID());

        if (_mvmntController.enabled)
            _mvmntController.GoToTarget(currentRoom.GetRandomPoiInRoom(), () => _taskStatus = TaskStatus.Success, () => _taskStatus = TaskStatus.Failure);
    }

    public override TaskStatus OnUpdate() => _taskStatus;
}
