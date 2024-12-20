using BehaviorDesigner.Runtime.Tasks;

[TaskCategory("Custom/Conversation")]
public class EndConversation : Action
{
    public override TaskStatus OnUpdate()
    {
        var ourId = GetComponent<CharacterInfo>().ID;

        NpcBehaviorBB.Instance.EndConversation(ourId);
        return TaskStatus.Success;
    }
}