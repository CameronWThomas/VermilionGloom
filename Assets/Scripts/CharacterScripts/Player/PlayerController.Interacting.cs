public partial class PlayerController
{
    public void TryInteractingWithCharacter(NpcBrain brain)
    {
        if (GameState.Instance.LongRangeInteracting)
            EnterConversationWithNpc(brain);
        else if (!brain.IsHostile && !brain.RelationshipWithPlayerIsHostile)
            mvmntController.GoToTarget(brain.transform, () => EnterConversationWithNpc(brain));
    }

    private void EnterConversationWithNpc(NpcBrain brain)
    {
        mvmntController.FaceTarget(brain.transform.position);
        UI_CharacterInteractionMenu.Instance.Activate(brain.GetNPCHumanCharacterID());
    }
}