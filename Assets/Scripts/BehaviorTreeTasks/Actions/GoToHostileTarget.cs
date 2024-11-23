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

        if (_ourBrain.HostileTowardsTarget == null)
        {
            // Try to set a hostile towards target
            if (!_ourBrain.SetHostileTowardsTarget())
            {
                _taskStatus = TaskStatus.Failure;
                return;
            }
        }

        GetComponent<MvmntController>().GoToTarget(_ourBrain.HostileTowardsTarget, () => _taskStatus = TaskStatus.Success, () => _taskStatus = TaskStatus.Failure);
    }

    public override TaskStatus OnUpdate()
    {
        if (_taskStatus is TaskStatus.Failure)
            return _taskStatus;

        var currentRoom = RoomBB.Instance.GetCharacterRoomID(_ourBrain.ID);
        if (RoomBB.Instance.GetCharacterRoomID(_ourBrain.ID) != _lastRoom)
        {
            // NPC has entered a new room. Make sure we can still see the target
            _lastRoom = currentRoom;

            // We can still see them, keep chasing
            if (_ourBrain.CanSeeTarget(_ourBrain.HostileTowardsTarget.GetCharacterID()))
                return _taskStatus;

            // Will either erase our hostile towards target, or find a new person to be hostile towards
            if (!_ourBrain.SetHostileTowardsTarget())
            {
                GetComponent<MvmntController>().CancelMovementAction();
                return TaskStatus.Failure;
            }
        }

        return _taskStatus;
    }
}