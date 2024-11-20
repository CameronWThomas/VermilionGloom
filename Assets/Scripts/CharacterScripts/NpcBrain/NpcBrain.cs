using UnityEngine;

public enum NpcState
{
    None,
    Wandering,
    Conversation
}

[RequireComponent(typeof(NPCHumanCharacterInfo))]
public partial class NpcBrain : MonoBehaviour
{
    [Header("Base Info")]
    /// <summary>public for inspector visibility. Do not change</summary>
    public NpcState State = NpcState.None;

    public bool IsInConversationState => State is NpcState.Conversation;

    private bool TryUpdateState(NpcState desiredState)
    {
        if (State != desiredState && State is not NpcState.None)
            return false;

        State = desiredState;
        return true;
    }

    private bool TryLeaveState(NpcState expectedCurrentState)
    {
        if (State != expectedCurrentState)
            return false;

        State = NpcState.None;
        return true;
    }

    private void ForceStateChange(NpcState newNpcState, bool reEvealuateTree = true)
    {
        State = newNpcState;

        if (reEvealuateTree)
            ReEvaluateTree();
    }

    /// <summary>
    /// Triggers a re-calculation of current behaviour tree. 
    /// Nice for when you expect some conditionals to change
    /// This could probably be written better, but it works.
    /// </summary>
    private void ReEvaluateTree()
    {
        behaviorTree.StopAllCoroutines();
        behaviorTree.StopAllTaskCoroutines();
        behaviorTree.ExternalBehavior = behaviorTree.ExternalBehavior;
        behaviorTree.enabled = false;
        behaviorTree.enabled = true;
        behaviorTree.Start();
    }
}
