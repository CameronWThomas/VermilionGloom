
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

[TaskCategory("Custom/Conversation")]
public class AttemptToEngageInConversation : Action
{
    public SharedTransform ConversationAttemptTarget;

    public override TaskStatus OnUpdate()
    {
        var ourId = GetComponent<CharacterInfo>().ID;
        var theirId = ConversationAttemptTarget.Value.GetComponent<CharacterInfo>().ID;

        if (NpcBehaviorBB.Instance.TryStartingConversation(ourId, theirId))
        {
            VoiceBox voiceBox = GetComponent<VoiceBox>();
            if(voiceBox != null)
            {
                voiceBox.PlayConvoStarter();
            }
            return TaskStatus.Success;
        }

        return TaskStatus.Failure;
    }
}