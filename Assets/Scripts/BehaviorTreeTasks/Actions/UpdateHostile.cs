using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

[TaskCategory("Custom")]
public class UpdateHostile : Action
{
    public SharedBool IsHostile;

    public override TaskStatus OnUpdate()
    {
        var npcBrain = GetComponent<NpcBrain>();
        if (IsHostile.Value)
            npcBrain.GetHostile();
        else
            npcBrain.EndHostility();

        return TaskStatus.Success;
    }

    public override string FriendlyName => IsHostile.Value ? "Get Hostile" : "End Hostility";

}