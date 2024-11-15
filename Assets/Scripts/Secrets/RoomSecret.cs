public class RoomSecret : Secret
{
    private SecretLevel _level;
    private string _roomDescription;

    public RoomSecret(string roomDescription) : this(roomDescription, SecretLevel.Public)
    { }

    public RoomSecret(string roomDescription, SecretLevel level)
    {
        _roomDescription = roomDescription;
        _level = level;
    }

    public override SecretLevel Level => _level;

    public override SecretIconIdentifier Identifier => SecretIconIdentifier.Room;

    public override string Description => $"There is something secret in {_roomDescription}";

    public override bool InvolvesCharacters => false;

}