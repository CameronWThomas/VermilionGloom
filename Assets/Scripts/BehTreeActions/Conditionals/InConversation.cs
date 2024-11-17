using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

[TaskCategory("Custom")]
public class InConversation : Conditional
{
    NpcBrain npcBrain;
    public override void OnStart()
    {
        npcBrain = GetComponent<NpcBrain>();
    }

    public override TaskStatus OnUpdate()
    {
        if(npcBrain.convoTarget != null)
        {
            return TaskStatus.Success;
        }
        return TaskStatus.Failure;
    }
}
