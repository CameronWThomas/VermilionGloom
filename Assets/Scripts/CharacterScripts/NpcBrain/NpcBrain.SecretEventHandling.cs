using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum SecretEventResponse
{
    ThumbsUp,
    Point,
    NA
}

public partial class NpcBrain
{
    [Header("Secret Processing")]
    [SerializeField] float _processedSecretCleanupTime = 5f;

    public List<SecretEvent> NewSecretEvents { get; } = new();
    public List<SecretEvent> ProcessedSecretEvents { get; } = new();

    public Secret SecretFromLastProcessedSecretEvent { get; set; } = null;
    public SecretEvent SecretEventToBroadcast { get; set; } = null;
    public SecretEventResponse SecretEventResponse { get; set; } = SecretEventResponse.NA;


    public bool AnyNewSecretEvents() => NewSecretEvents.Any();

    
    public void ProcessSecretEvent()
    {
        SecretFromLastProcessedSecretEvent = null;
        SecretEventToBroadcast = null;
        SecretEventResponse = SecretEventResponse.NA;

        if (!NewSecretEvents.Any())
            return;

        var secretEvent = NewSecretEvents[0];
        NewSecretEvents.Remove(secretEvent);

        ProcessedSecretEvents.Add(secretEvent);

        Func<SecretEvent, Secret> eventHandler = secretEvent.SecretEventType switch
        {
            SecretEventType.StranglingSomeone => x => AddOrUpdateMurderSecret(x, true),
            SecretEventType.KilledSomeone => x => AddOrUpdateMurderSecret(x, false),
            SecretEventType.AttackingSomeone => x => AddOrUpdateMurderSecret(x, true),

            SecretEventType.DraggingABody => HandleDraggingABodyEvent,
            _ => x => null
        };

        SecretFromLastProcessedSecretEvent = eventHandler(secretEvent);

        if (secretEvent.SecretEventType == SecretEventType.StranglingSomeone && secretEvent.SecretNoticability == SecretNoticability.Sight)
        {
            SecretEventToBroadcast = new SecretEvent(SecretEventType.StranglingSomeone,
                secretEvent.Originator,
                secretEvent.AdditionalCharacter,
                SecretNoticability.Room,
                SecretDuration.Instant);
        }
    }

    public void ReceiveBroadcast(List<SecretEvent> secretEvents)
    {
        if (GetComponent<CharacterInfo>().IsDead || IsHostile)
            return;

        foreach (var secretEvent in secretEvents)
        {
            if (!IsSecretEventNoticable(secretEvent))
                continue;

            if (NewSecretEvents.All(x => !x.Compare(secretEvent)) && ProcessedSecretEvents.All(x => !x.Compare(secretEvent)))
                NewSecretEvents.Add(secretEvent);
        }
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

    private void AddSecretsForDeadCharacters(List<CharacterInfo> characterInfos)
    {
        foreach (var characterInfo in characterInfos)
        {
            if (!characterInfo.IsDead)
                continue;

            var existingMurderSecret = characterSecretKnowledge.Secrets.OfType<MurderSecret>().FirstOrDefault(x => x.AdditionalCharacter == characterInfo.ID);
            if (existingMurderSecret != null)
            {
                if (!existingMurderSecret.IsAttempt)
                    continue;

                // Mark the murder as no longer an attempt, but successful
                existingMurderSecret.UpdateJustificationOrAttempt(existingMurderSecret.IsJustified, false);
            }
            else
            {
                characterSecretKnowledge.AddSecret(new MurderSecret.Builder(ID, SecretLevel.Public)
                    .SetVictim(characterInfo.ID)
                    .IsNotJustified()
                    .WasSuccessfulMuder()
                    .Build());
            }

            var relationship = GetRelationship(characterInfo.ID);
            relationship.Reevaluate();
        }
    }

    private Secret AddOrUpdateMurderSecret(SecretEvent secretEvent, bool isAttempt)
    {
        var relationshipWithKiller = GetRelationship(secretEvent.Originator);
        var relationshipWithVictim = GetRelationship(secretEvent.AdditionalCharacter);

        var isJustified = relationshipWithVictim.IsHostileTowards;

        if (characterSecretKnowledge.TryGetMurderSecret(secretEvent.Originator, secretEvent.AdditionalCharacter, out var murderSecret))
        {
            murderSecret.UpdateJustificationOrAttempt(isJustified, isAttempt);
        }
        else
        {
            var murderSecretBuilder = new MurderSecret.Builder(secretEvent.Originator, SecretLevel.Public)
                .SetMurderer(secretEvent.Originator)
                .SetVictim(secretEvent.AdditionalCharacter);

            if (isAttempt)
                murderSecretBuilder.WasAttempt();
            else
                murderSecretBuilder.WasSuccessfulMuder();

            if (isJustified)
                murderSecretBuilder.IsJustified();
            else
                murderSecretBuilder.IsNotJustified();

            murderSecret = murderSecretBuilder.Build();
            characterSecretKnowledge.AddSecret(murderSecret);
        }

        relationshipWithKiller.Reevaluate();
        relationshipWithVictim.Reevaluate();

        return murderSecret;
    }

    private Secret HandleDraggingABodyEvent(SecretEvent secretEvent)
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

        return dragSecret;
    }
}