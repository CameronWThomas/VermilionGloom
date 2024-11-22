using UnityEngine;

public enum SecretEventType
{
    StranglingSomeone,
    KilledSomeone,
    DraggingABody,
    Dead
}

public class SecretEvent
{
    public SecretEvent(SecretEventType secretEventType, CharacterID originator, CharacterID target)
    {
        SecretEventType = secretEventType;
        Originator = originator;
        Target = target;
    }

    public SecretEventType SecretEventType { get; }
    public CharacterID Originator { get; }
    public CharacterID Target { get; }

    //TODO include something for events that alert everyone in the room
}