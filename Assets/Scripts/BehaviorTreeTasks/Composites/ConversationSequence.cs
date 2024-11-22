using BehaviorDesigner.Runtime.Tasks;

[TaskCategory("Custom")]
[TaskDescription("Normal Sequence but makes sure end a conversation if there is an abort")]
public class ConversationSequence : Sequence
{
    public override void OnConditionalAbort()
    {
        EndActiveConversation();
    }

    private void EndActiveConversation()
    {
        var ourId = transform.GetNPCHumanCharacterID();
        if (NpcBehaviorBB.Instance.IsInConversation(transform.GetNPCHumanCharacterID(), out _))
            NpcBehaviorBB.Instance.EndConversation(ourId);
    }
}