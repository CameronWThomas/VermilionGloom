using BehaviorDesigner.Runtime.Tasks;

//TODO should be combined with crunch target. And a conditional check of did kill
[TaskCategory("Custom")]
public class KillHostileTowardsTarget : Action
{
    public override TaskStatus OnUpdate()
    {
        var ourBrain = GetComponent<NpcBrain>();
        if (!ourBrain.IsHostile)
            return TaskStatus.Failure;

        var characterInfo = CharacterInfoBB.Instance.GetCharacterInfo(ourBrain.HostileTowardsTarget.GetCharacterID());
        characterInfo.Die();

        ourBrain.AddPersonalMurderSecret(characterInfo.ID);
        //TODO broadcast a murder event

        return TaskStatus.Success;
    }
}