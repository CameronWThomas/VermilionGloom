using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class NpcBrain
{
    public List<SecretEvent> NewSecretEvents { get; } = new();
    public List<SecretEvent> ProcessedSecretEvents { get; } = new();

    public SecretEventResponse LastSecretEventResponse { get; private set; } = null;


    public bool AnyNewSecretEvents() => NewSecretEvents.Any();

    
    public void ProcessSecretEvent()
    {
        LastSecretEventResponse = null;

        if (!NewSecretEvents.Any())
            return;

        var secretEvent = NewSecretEvents[0];
        NewSecretEvents.Remove(secretEvent);

        ProcessedSecretEvents.Add(secretEvent);

        LastSecretEventResponse = secretEvent switch
        {
            MurderSecretEvent murderSecretEvent => ProcessMurderSecretEvent(murderSecretEvent),
            _ => null
        };
    }

    public void ReceiveBroadcast(List<SecretEvent> secretEvents)
    {
        if (GetComponent<CharacterInfo>().IsDead || IsHostile)
            return;

        foreach (var secretEvent in secretEvents)
        {
            var isNoticable = secretEvent switch
            {
                MurderSecretEvent murder => IsNoticable(murder),
                _ => false,
            };

            if (!isNoticable)
                continue;

            if (NewSecretEvents.All(x => !x.Compare(secretEvent)) && ProcessedSecretEvents.All(x => !x.Compare(secretEvent)))
                NewSecretEvents.Add(secretEvent);
        }
    }

    private bool IsNoticable(MurderSecretEvent murder)
    {
        if (murder.Victim == ID)
            return true;

        return IsSecretEventNoticable(murder.SecretNoticability, murder.Murderer, murder.Victim);
    }

    private bool IsSecretEventNoticable(SecretNoticability noticability, params CharacterID[] involvedCharacters)
    {

        var noticable = noticability switch
        {
            SecretNoticability.Sight => CheckIfInvolvedCharactersInSight(involvedCharacters),
            SecretNoticability.Room => CheckIfInSameRoomAsAnyInvolvedCharacter(involvedCharacters),
            SecretNoticability.Everyone => true,
            _ => throw new NotImplementedException()
        };

        return noticable;
    }    

    private bool CheckIfInvolvedCharactersInSight(IEnumerable<CharacterID> involvedCharacters)
    {
        // Don't see anyone
        if (!FindCharactersInSight(out var characters))
            return false;

        return characters.Any(x => involvedCharacters.Contains(x.ID));
    }

    private bool CheckIfInSameRoomAsAnyInvolvedCharacter(IEnumerable<CharacterID> involvedCharacters)
    {
        var myRoom = CurrentRoom;
        if (myRoom is RoomID.Unknown)
            return false;

        foreach (var involvedCharacter in involvedCharacters)
        {
            if (RoomBB.Instance.GetCharacterRoomID(involvedCharacter) == myRoom)
                return true;
        }

        return false;
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
                existingMurderSecret.UpdateIsAttempt(false);
            }
            else
            {
                characterSecretKnowledge.AddSecret(new MurderSecret.Builder(ID, SecretLevel.Public)
                    .SetVictim(characterInfo.ID)
                    .WasSuccessfulMuder()
                    .Build());
            }

            var relationship = GetRelationship(characterInfo.ID);
            relationship.Reevaluate();
        }
    }

    private SecretEventResponse ProcessMurderSecretEvent(MurderSecretEvent murderSecretEvent)
    {
        var relationshipWithMurderer = GetRelationship(murderSecretEvent.Murderer);
        var murdererTransform = CharacterInfoBB.Instance.GetCharacterInfo(murderSecretEvent.Murderer).transform;

        var relationshipWithVictim = GetRelationship(murderSecretEvent.Victim);

        // If we are hostile towards the target, we don't care about the murder
        if (relationshipWithVictim.HasHostileOpinionOf)
            return new SecretEventResponse(SecretEventResponseType.Good, murdererTransform);

        SecretEventResponse secretEventResponse = null;
        if (characterSecretKnowledge.TryGetMurderSecret(murderSecretEvent.Murderer, murderSecretEvent.Victim, out var murderSecret))
        {
            murderSecret.UpdateIsAttempt(murderSecretEvent.IsAttempt);
            secretEventResponse = new SecretEventResponse(SecretEventResponseType.Hostile, murdererTransform);
        }
        else
        {
            murderSecret = new MurderSecret.Builder(ID, SecretLevel.Public)
                .SetMurderer(murderSecretEvent.Murderer)
                .SetVictim(murderSecretEvent.Victim)
                .SetIsAttempt(murderSecretEvent.IsAttempt)
                .Build();
            characterSecretKnowledge.AddSecret(murderSecret);

            var newSecretEvent = new MurderSecretEvent(murderSecretEvent.Murderer,
                murderSecretEvent.Victim,
                murderSecretEvent.IsAttempt,
                SecretNoticability.Room,
                SecretDuration.Instant);

            secretEventResponse = new SecretEventResponse(SecretEventResponseType.Bad, murdererTransform, newSecretEvent);
        }

        relationshipWithMurderer.Reevaluate();
        relationshipWithVictim.Reevaluate();

        return secretEventResponse;
    }

    public enum SecretEventResponseType
    {
        Bad,
        Good,
        Hostile, // No need to play animation, just get hostile
        NA
    }

    public class SecretEventResponse
    {
        public SecretEventResponse(SecretEventResponseType responseType, Transform secretResponseTarget, SecretEvent newSecretEvent = null)
        {
            ResponseType = responseType;
            SecretResponseTarget = secretResponseTarget;
            NewSecretEvent = newSecretEvent;
        }

        public SecretEventResponseType ResponseType { get; } = SecretEventResponseType.NA;
        public Transform SecretResponseTarget { get; }
        public SecretEvent NewSecretEvent { get; }
    }
}