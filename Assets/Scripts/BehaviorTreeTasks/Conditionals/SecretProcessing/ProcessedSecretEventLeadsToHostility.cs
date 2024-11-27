using BehaviorDesigner.Runtime.Tasks;
using System.Linq;

[TaskCategory("Custom/Secret Processing")]
public class ProcessedSecretEventLeadsToHostility : Conditional
{
    NpcBrain _brain;

    public override void OnStart()
    {
        _brain = GetComponent<NpcBrain>();
    }

    public override TaskStatus OnUpdate()
    {
        var shouldGetHostile = _brain.LastSecretEventResponse?.ResponseType switch
        { 
            NpcBrain.SecretEventResponseType.Bad or NpcBrain.SecretEventResponseType.Hostile => true,
            _ => false,
        };

        if (shouldGetHostile)
            return TaskStatus.Success;

        return TaskStatus.Failure;
    }
}