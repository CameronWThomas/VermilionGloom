using System;
using UnityEngine;

public enum MurderSecretEventType
{
    StranglingSomeone,
    KilledSomeone,
    DraggingABody,
    AttackingSomeone,
}

public static class MurderSecretEventTypeExtensions
{
    public static bool IsAttempt(this MurderSecretEventType type)
        => type switch
        {
            MurderSecretEventType.StranglingSomeone => true,
            MurderSecretEventType.AttackingSomeone => true,
            MurderSecretEventType.DraggingABody => false,
            MurderSecretEventType.KilledSomeone => false,
            _ => throw new NotImplementedException()
        };
}

public enum SecretNoticability
{
    Sight,
    Room,
    Everyone
}

public enum SecretDuration
{
    UntilCancel,
    Instant
}

public abstract class SecretEvent
{
    protected SecretEvent(SecretNoticability secretNoticability, SecretDuration secretDuration)
    {
        SecretNoticability = secretNoticability;
        SecretDuration = secretDuration;
    }

    public SecretDuration SecretDuration { get; }
    public SecretNoticability SecretNoticability { get; }

    public abstract bool Compare(SecretEvent other);
}

public class MurderSecretEvent : SecretEvent
{
    public MurderSecretEvent(CharacterID murderer, CharacterID victim, bool isAttempt, SecretNoticability secretNoticability, SecretDuration secretDuration)
        : base(secretNoticability, secretDuration)
    {
        Murderer = murderer;
        Victim = victim;
        IsAttempt = isAttempt;
    }

    public CharacterID Murderer { get; }
    public CharacterID Victim { get; }
    public bool IsAttempt { get; }

    public override bool Compare(SecretEvent other)
    {
        if (other is not MurderSecretEvent murderSecretEvent)
            return false;

        return Murderer == murderSecretEvent.Murderer && Victim == murderSecretEvent.Victim && IsAttempt == murderSecretEvent.IsAttempt;
    }
}