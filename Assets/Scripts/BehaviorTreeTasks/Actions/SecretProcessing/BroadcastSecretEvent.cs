using BehaviorDesigner.Runtime.Tasks;
using System.Diagnostics.CodeAnalysis;

[TaskCategory("Custom/Secret Processing")]
public class BroadcastSecretEvent : Action
{
    public override TaskStatus OnUpdate()
    {
        var brain = GetComponent<NpcBrain>();

        if (brain.SecretEventToBroadcast != null)
            NpcBehaviorBB.Instance.BroadcastSecretEvent(brain.SecretEventToBroadcast);

        return TaskStatus.Success;
 
    }
}