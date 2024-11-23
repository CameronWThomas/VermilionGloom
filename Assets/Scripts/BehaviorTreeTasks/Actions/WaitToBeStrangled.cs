using BehaviorDesigner.Runtime.Tasks;

[TaskCategory("Custom")]
public class WaitToBeStrangled : Action
{
    NPCHumanCharacterID _characterId;

    public override void OnStart()
    {
        _characterId = transform.GetNPCHumanCharacterID();
    }

    public override TaskStatus OnUpdate()
    {
        if (NpcBehaviorBB.Instance.IsDead(_characterId))
            return TaskStatus.Success;

        if (NpcBehaviorBB.Instance.IsBeingStrangled(_characterId) ||
            NpcBehaviorBB.Instance.IsStrangled(_characterId))
            return TaskStatus.Running;

        return TaskStatus.Failure;
    }
}