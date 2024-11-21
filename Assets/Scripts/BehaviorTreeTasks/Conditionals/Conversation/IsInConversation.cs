using BehaviorDesigner.Runtime.Tasks;

[TaskCategory("Custom/Conversation")]
public class IsInConversation : Conditional
{
    public override TaskStatus OnUpdate()
    {
        return NpcCharacterAIStateBB.Instance.IsInConversation(GetComponent<CharacterInfo>().ID)
            ? TaskStatus.Success
            : TaskStatus.Failure; ;
    }

    public override float GetPriority() => 1f;
}