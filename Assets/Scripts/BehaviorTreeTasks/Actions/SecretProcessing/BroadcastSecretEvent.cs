using BehaviorDesigner.Runtime.Tasks;
using System.Diagnostics.CodeAnalysis;

[TaskCategory("Custom/Secret Processing")]
public class BroadcastSecretEvent : Action
{
    public override TaskStatus OnUpdate()
    {
        var secretEvent = GetComponent<NpcBrain>().LastSecretEventResponse?.NewSecretEvent;

        if (secretEvent != null)
            NpcBehaviorBB.Instance.BroadcastSecretEvent(secretEvent);

        return TaskStatus.Success;
 
    }
}