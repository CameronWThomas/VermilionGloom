using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Rendering;
using UnityEngine.TextCore.Text;

public partial class NpcBrain
{
    public void ReceiveBroadcast(List<SecretEvent> secretEvents)
    {
        if (GetComponent<CharacterInfo>().IsDead)
            return;

        foreach (var secretEvent in secretEvents)
        {
            if (!IsSecretEventNoticable(secretEvent))
                continue;

            Action<SecretEvent> eventHandler = secretEvent.SecretEventType switch
            {
                SecretEventType.StranglingSomeone => HandleStranglingEvent,
                SecretEventType.KilledSomeone => HandleKilledSomeoneEvent,
                SecretEventType.DraggingABody => HandleDraggingABodyEvent,
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

    private bool IsSecretEventNoticable(SecretEvent secretEvent)
    {
        if (secretEvent.AdditionalCharacter == ID)
            return true;

        var noticable = secretEvent.SecretNoticability switch
        {
            SecretNoticability.Sight => CheckIfInvolvedCharactersInSight(secretEvent),
            SecretNoticability.Room => CheckIfInSameRoomAsAnyInvolvedCharacter(secretEvent),
            SecretNoticability.Everyone => true,
            _ => throw new NotImplementedException()
        };

        return noticable;
    }    

    private bool CheckIfInvolvedCharactersInSight(SecretEvent secretEvent)
    {
        // Don't see anyone
        if (!FindCharactersInSight(out var characters))
            return false;

        var involvedCharacters = characters.Select(x => x.ID).Where(x => x == secretEvent.AdditionalCharacter || x == secretEvent.Originator);

        return involvedCharacters.Any();
    }

    private bool CheckIfInSameRoomAsAnyInvolvedCharacter(SecretEvent secretEvent)
    {
        var myRoom = CurrentRoom;
        if (myRoom is RoomID.Unknown)
            return false;

        if (RoomBB.Instance.GetCharacterRoomID(secretEvent.Originator) == myRoom)
            return true;

        return RoomBB.Instance.GetCharacterRoomID(secretEvent.AdditionalCharacter) == myRoom;
    }

    private MurderSecret CreatePersonalMurderSecret(CharacterID victim, out bool wasExistingSecret)
    {
        wasExistingSecret = true;
        if (characterSecretKnowledge.TryGetMurderSecret(ID, victim, out var murderSecret))
            return murderSecret;

        wasExistingSecret = false;
        return new MurderSecret.Builder(ID, SecretLevel.Private)
            .SetMurderer(ID)
            .SetVictim(victim)
            .IsJustified()
            .WasSuccessfulMuder()
            .Build();
    }

    private void HandleStranglingEvent(SecretEvent secretEvent)
    {
        if (!characterSecretKnowledge.TryGetMurderSecret(secretEvent.Originator, secretEvent.AdditionalCharacter, out var murderSecret))
        {
            murderSecret = new MurderSecret.Builder(ID, SecretLevel.Public)
                .SetMurderer(secretEvent.Originator)
                .SetVictim(secretEvent.AdditionalCharacter)
                .IsNotJustified()
                .WasAttempt()
                .Build();

            characterSecretKnowledge.AddSecret(murderSecret);
        }

        var relationshipWithStrangler = GetRelationship(secretEvent.Originator);
        var relationshipWithStrangled = GetRelationship(secretEvent.AdditionalCharacter);
        relationshipWithStrangler.Reevaluate();
        relationshipWithStrangled.Reevaluate();
    }

    private void HandleKilledSomeoneEvent(SecretEvent secretEvent)
    {
        var relationshipWithKiller = GetRelationship(secretEvent.Originator);
        var relationshipWithVictim = GetRelationship(secretEvent.AdditionalCharacter);

        var isJustified = relationshipWithVictim.IsHostileTowards;

        if (characterSecretKnowledge.TryGetMurderSecret(secretEvent.Originator, secretEvent.AdditionalCharacter, out var murderSecret))
        {
            murderSecret.UpdateJustificationOrAttempt(isJustified, false);
            murderSecret.UpdateSecretLevel(SecretLevel.Public);
        }
        else
        {
            var murderSecretBuilder = new MurderSecret.Builder(secretEvent.Originator, SecretLevel.Public)
                .SetMurderer(secretEvent.Originator)
                .SetVictim(secretEvent.AdditionalCharacter)
                .WasSuccessfulMuder();

            if (isJustified)
                murderSecretBuilder.IsJustified();
            else
                murderSecretBuilder.IsNotJustified();

            murderSecret = murderSecretBuilder.Build();
            characterSecretKnowledge.AddSecret(murderSecret);
        }

        relationshipWithKiller.Reevaluate();
        relationshipWithVictim.Reevaluate();
    }

    private void HandleDraggingABodyEvent(SecretEvent secretEvent)
    {
        var dragSecret = characterSecretKnowledge.GetSecrets(secretEvent.Originator, secretEvent.AdditionalCharacter)
            .OfType<DragSecret>()
            .FirstOrDefault();

        if (dragSecret == null)
        {
            dragSecret = new DragSecret.Builder(ID, SecretLevel.Public)
                .SetDragger(secretEvent.Originator)
                .SetDragged(secretEvent.AdditionalCharacter)
                .Build();

            characterSecretKnowledge.AddSecret(dragSecret);
        }

        var draggerRelationship = GetRelationship(secretEvent.Originator);
        var draggedRelationship = GetRelationship(secretEvent.AdditionalCharacter);
        draggerRelationship.Reevaluate();
        draggedRelationship.Reevaluate();
    }
}