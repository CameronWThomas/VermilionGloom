
using BehaviorDesigner.Runtime.Tasks;
using Mono.Cecil.Cil;

[TaskCategory("Conversation")]
public class GoToConversationTarget : GoToOtherMvmntController
{
    protected override MvmntController GetTargetController()
    {
        return GetComponent<NpcBrain>().ConvoTarget.GetComponent<MvmntController>();
    }
}