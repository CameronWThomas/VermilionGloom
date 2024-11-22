using BehaviorDesigner.Runtime.Tasks;

[TaskCategory("Custom")]
public class Die : Action
{
    public override void OnStart()
    {
        NpcBehaviorBB.Instance.StrangleDie(transform.GetNPCHumanCharacterID());
    }
}