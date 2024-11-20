
using BehaviorDesigner.Runtime.Tasks;

[TaskCategory("Conversation")]
public class FindConversationTarget : Action
{
    NpcBrain _ourBrain;

    public override void OnStart()
    {
        _ourBrain = GetComponent<NpcBrain>();

        if (!_ourBrain.HasConversationTarget || !_ourBrain.IsSearchingForConversationTarget)
            _ourBrain.FindConversationTarget();
    }

    public override TaskStatus OnUpdate()
    {
        if (_ourBrain.HasConversationTarget)
            return TaskStatus.Success;

        if (_ourBrain.IsSearchingForConversationTarget)
            return TaskStatus.Running;

        return TaskStatus.Failure;
    }
}