using BehaviorDesigner.Runtime.Tasks;

[TaskCategory("Custom")]
[TaskDescription("Goes to the first hostile target they see")]
public class GoToHostileTowardsTarget : Action
{
    TaskStatus _taskStatus;
    RoomID _lastRoom;

    NpcBrain _ourBrain;

    public override void OnStart()
    {
        _taskStatus = TaskStatus.Running;
        _ourBrain = GetComponent<NpcBrain>();
        _lastRoom = RoomBB.Instance.GetCharacterRoomID(_ourBrain.ID);

        if (!_ourBrain.SetHostileTowardsTarget())
        {
            _taskStatus = TaskStatus.Failure;
            return;
        }

        GetComponent<MvmntController>().GoToTarget(_ourBrain.HostileTowardsTarget, () => _taskStatus = TaskStatus.Success, () => _taskStatus = TaskStatus.Failure);
    }

    public override TaskStatus OnUpdate()
    {
        if (_taskStatus is TaskStatus.Failure)
            return _taskStatus;

        // If they can't see the player after entering a room, failure
        var currentRoom = RoomBB.Instance.GetCharacterRoomID(_ourBrain.ID);
        if (RoomBB.Instance.GetCharacterRoomID(_ourBrain.ID) != _lastRoom)
        {
            _lastRoom = currentRoom;
            if (!_ourBrain.CanSeeTarget(_ourBrain.HostileTowardsTarget.GetCharacterID()))
                return TaskStatus.Failure;
        }


        return _taskStatus;
    }
}