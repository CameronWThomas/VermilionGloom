using BehaviorDesigner.Runtime.Tasks;

//TODO should be combined with crunch target. And a conditional check of did kill
[TaskCategory("Custom")]
public class IsHostileTowardsTargetDead : Conditional
{
    public override TaskStatus OnUpdate()
    {
        var ourBrain = GetComponent<NpcBrain>();
        if (!ourBrain.IsHostile)
            return TaskStatus.Failure;

        var characterInfo = CharacterInfoBB.Instance.GetCharacterInfo(ourBrain.HostileTowardsTarget.GetCharacterID());
        
        return characterInfo.IsDead ? TaskStatus.Success : TaskStatus.Failure;
    }
}