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

        //Everyone say hi please
        VoiceBox mine = GetComponent<VoiceBox>();
        if (mine != null)
        {
            mine.PlayConvoStarter();
        }
        VoiceBox theirs = brain.GetComponent<VoiceBox>();
        if (theirs != null)
        {
            theirs.PlayConvoStarter();
        }

        UI_CharacterInteractionMenu.Instance.Activate(brain.GetNPCHumanCharacterID());
    }
}