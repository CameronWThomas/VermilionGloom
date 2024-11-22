public class RoomSecret : Secret
{
    private SecretLevel _level;
    private RoomID _roomID;

    public RoomSecret(RoomID roomID)
    {
        _roomID = roomID;
    }

    private RoomSecret(RoomSecret secret) : base(secret)
    {
        _roomID = secret._roomID;
    }

    public override SecretIconIdentifier Identifier => SecretIconIdentifier.Room;

    protected override Secret Copy() => new RoomSecret(this);

    public override string CreateDescription() => $"{CurrentSecretOwner.Name} knows about something secret in {_roomID.ToString()}";

    public class Builder : SecretTypeBuilder<RoomSecret>
    {
        private RoomID? _roomId;

        public Builder(CharacterID secretOwner, SecretLevel secretLevel) : base(secretOwner, secretLevel) { }

        public Builder SetRoomId(RoomID roomID)
        {
            _roomId = roomID;
            return this;
        }

        public override RoomSecret Build()
        {
            ValidateNotNull(_roomId, nameof(_roomId));

            var roomSecret = new RoomSecret(_roomId.Value);
            Init(roomSecret);
            return roomSecret;
        }
    }

}