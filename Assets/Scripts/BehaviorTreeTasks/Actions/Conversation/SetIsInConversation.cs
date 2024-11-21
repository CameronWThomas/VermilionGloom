using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

[TaskCategory("Custom/Conversation")]
public class SetIsInConversation : Action
{
    public SharedBool Value;

    public override void OnStart()
    {
        Owner.IsInConversation(Value.Value);
    }
}