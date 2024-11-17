using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

[TaskCategory("Custom")]
public class ShouldKillTarget : Conditional
{
    NpcBrain npcBrain;
    public override void OnStart()
    {
        npcBrain = GetComponent<NpcBrain>();
    }

    public override TaskStatus OnUpdate()
    {
        if (npcBrain.combatTarget != null)
        {
            return TaskStatus.Success;
        }
        return TaskStatus.Failure;
    }
}
