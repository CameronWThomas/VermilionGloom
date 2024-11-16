public class MurderSecret : Secret
{
    public MurderSecret(SecretLevel level, CharacterID murderer, CharacterID victim = null)
        : base(level, murderer, victim)
    { }

    private MurderSecret(MurderSecret secret) : base(secret) { }

    public override SecretIconIdentifier Identifier => SecretIconIdentifier.Murder;

    public override Secret Copy() => new MurderSecret(this);

    public override string CreateDescription() => $"{SecretOwner.Name} killed {GetVictimName()}";

    private string GetVictimName()
    {
        if (!HasAdditionalCharacter)
            return "someone long ago...";

        return AdditionalCharacter.Name;
    }
}