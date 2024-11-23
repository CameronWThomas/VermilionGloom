using BehaviorDesigner.Runtime.Tasks;
using System.Linq;

[TaskCategory("Custom/Conversation")]
[TaskDescription("Possibly spread a secret to our conversation target. Will always return success.")]
public class SpreadSecretToConversationTarget : Action
{
    public override TaskStatus OnUpdate()
    {
        TrySpreadSecrets();

        return TaskStatus.Success;
    }

    private void TrySpreadSecrets()
    {
        var ourId = transform.GetNPCHumanCharacterID();
        if (NpcBehaviorBB.Instance.IsInConversationWithPlayer(ourId) ||
            !NpcBehaviorBB.Instance.IsInConversation(ourId, out var targetTransform))
            return;

        var targetId = transform.GetNPCHumanCharacterID();
        CharacterSecretKnowledgeBB.Instance.TrySpreadSecret(ourId, targetId);
    }
}