using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

[TaskCategory("Custom")]
public class IsInConversation : Conditional
{
    NpcBrain _npcBrain;

    public override void OnStart()
    {
        _npcBrain = GetComponent<NpcBrain>();
    }

    public override TaskStatus OnUpdate()
    {
        if(_npcBrain.IsInConversation || _npcBrain.IsWaitingForConversationTarget)
        {
            return TaskStatus.Success;
        }
        return TaskStatus.Failure;
    }
}
