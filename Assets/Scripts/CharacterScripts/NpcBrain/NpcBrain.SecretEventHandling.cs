using System;
using System.Collections.Generic;
using System.Linq;

public partial class NpcBrain
{
    public void ReceiveBroadcast(List<SecretEvent> secretEvents)
    {
        foreach (var secretEvent in secretEvents)
        {
            Action<SecretEvent> eventHandler = secretEvent.SecretEventType switch
            {
                SecretEventType.StranglingSomeone => HandleStranglingEvent,
                _ => null
            };

            eventHandler?.Invoke(secretEvent);
        }

        //if (
        //    (looker.CanSeeTarget(shouldSee) || looker.CanSeeTarget(extraObject))
        //    && !dead && !strangled)
        //{

        //    switch (type)
        //    {
        //        case BroadcastType.Drag:
        //            // Handle drag broadcast
        //            //Debug.Log(gameObject.name + " saw " + shouldSee.name + " dragging someone");
        //            SawCorpseDragging(shouldSee, extraObject);
        //            break;
        //        case BroadcastType.Strangle:
        //            // Handle strangle broadcast
        //            SawStrangling(shouldSee, extraObject);
        //            break;
        //        default:
        //            // Handle other types of broadcasts
        //            break;
        //    }
        //}
    }

    private void HandleStranglingEvent(SecretEvent secretEvent)
    {
        if (!CheckIfInvolvedCharactersInSight(secretEvent))
            return;

        if (!characterSecretKnowledge.TryGetMurderSecret(secretEvent.Originator, secretEvent.Target, out var murderSecret))
        {
            murderSecret = new MurderSecret.Builder(ID, SecretLevel.Public)
                .SetMurderer(secretEvent.Originator)
                .SetVictim(secretEvent.Target)
                .IsNotJustified()
                .WasAttempt()
                .Build();

            characterSecretKnowledge.AddSecret(murderSecret);
        }

        var relationship = GetRelationship(secretEvent.Originator);
        relationship.Reevaluate();
    }

    private bool CheckIfInvolvedCharactersInSight(SecretEvent secretEvent)
    {
        // Don't see anyone
        if (!looker.TryGetCharactersInSight(out var characters))
            return false;

        var involvedCharacters = characters.Select(x => x.ID).Where(x => x == secretEvent.Target || x == secretEvent.Originator);

        // Don't see any characters involved
        if (!involvedCharacters.Any())
            return false;

        // Make sure the characters our in our room
        // TODO may not always want this. May do that foot raycast thing
        if (involvedCharacters.All(x => RoomBB.Instance.GetCharacterRoomID(x) != CurrentRoom))
            return false;

        return true;
    }
}