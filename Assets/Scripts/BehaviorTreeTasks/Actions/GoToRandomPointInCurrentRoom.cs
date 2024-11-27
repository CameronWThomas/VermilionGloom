using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

[TaskCategory("Custom")]
public class GoToRandomPointInCurrentRoom : Action
{
    MvmntController _mvmntController;
    TaskStatus _taskStatus = TaskStatus.Running;
    
    public override void OnStart()
    {
        _mvmntController = GetComponent<MvmntController>();
        _mvmntController.SetRunning(false);
        
        _taskStatus = TaskStatus.Running;

        var currentRoom = RoomBB.Instance.GetCharacterRoom(transform.GetCharacterID());
        if (currentRoom == null)
            _taskStatus = TaskStatus.Failure;
        else if (_mvmntController.enabled)
            _mvmntController.GoToTarget(currentRoom.GetRandomPointInRoom(), () => _taskStatus = TaskStatus.Success, () => _taskStatus = TaskStatus.Failure);
    }

    public override TaskStatus OnUpdate() => _taskStatus;

    public override void OnConditionalAbort() => _mvmntController.CancelMovementAction();
}
