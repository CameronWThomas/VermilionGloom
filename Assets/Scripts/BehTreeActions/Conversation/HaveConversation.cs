using BehaviorDesigner.Runtime.Tasks;
using Unity.VisualScripting;
using UnityEngine.Analytics;

[TaskCategory("Conversation")]
public class HaveConversation : Action
{
    NpcBrain _ourBrain;

    public override void OnStart()
    {
        _ourBrain = GetComponent<NpcBrain>();
        if (!_ourBrain.IsInConversation)
            _ourBrain.StartConversation();
    }

    public override TaskStatus OnUpdate()
    {
        if (_ourBrain.IsInConversation || _ourBrain.IsWaitingForConversationTarget)
            return TaskStatus.Running;

        return TaskStatus.Running;
    }
}