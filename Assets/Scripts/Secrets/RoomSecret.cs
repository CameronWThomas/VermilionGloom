public class RoomSecret : Secret
{
    private SecretLevel _level;
    private string _roomDescription;

    public RoomSecret(string roomDescription, SecretLevel level, CharacterID secretOwner)
        : base(level, secretOwner)
    { 
        _roomDescription = roomDescription;
    }

    private RoomSecret(RoomSecret secret) : base(secret)
    {
        _roomDescription = secret._roomDescription;
    }

    public override SecretIconIdentifier Identifier => SecretIconIdentifier.Room;

    public override Secret Copy() => new RoomSecret(this);

    public override string CreateDescription() => $"{SecretOwner.Name} knows about something secret in {_roomDescription}";

}