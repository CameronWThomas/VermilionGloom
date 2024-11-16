using BehaviorDesigner.Runtime.Tasks;

[TaskCategory("Custom")]
public class IsDead : Conditional
{
    NpcBrain npcBrain;
    public override void OnStart()
    {
        npcBrain = GetComponent<NpcBrain>();
    }

    public override TaskStatus OnUpdate()
    {
        if (npcBrain != null)
        {
            if(npcBrain.dead)
                return TaskStatus.Success;
            else
                return TaskStatus.Failure;
        }
        return TaskStatus.Failure;
    }
}
