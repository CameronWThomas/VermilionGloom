using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

[TaskCategory("Custom/Conversation")]
public class FaceConversationTarget : Action
{
    public override void OnStart()
    {
        var targetTransform = NpcCharacterAIStateBB.Instance.GetInConversationTransform(GetComponent<CharacterInfo>().ID);
        GetComponent<MvmntController>().FaceTarget(targetTransform.position);
    }
}