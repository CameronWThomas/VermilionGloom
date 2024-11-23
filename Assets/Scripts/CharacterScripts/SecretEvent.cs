using UnityEngine;

public enum SecretEventType
{
    StranglingSomeone,
    KilledSomeone,
    DraggingABody,
    Dead
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

public class SecretEvent
{
    public SecretEvent(SecretEventType secretEventType,        
        CharacterID originator,
        CharacterID additionalCharacter,
        SecretNoticability secretNoticability,
        SecretDuration secretDuration)
    {
        SecretEventType = secretEventType;        
        Originator = originator;
        AdditionalCharacter = additionalCharacter;
        SecretNoticability = secretNoticability;
        SecretDuration = secretDuration;
    }

    public SecretEventType SecretEventType { get; }
    public CharacterID Originator { get; }
    public CharacterID AdditionalCharacter { get; }

    public SecretDuration SecretDuration { get; }
    public SecretNoticability SecretNoticability { get; }
}