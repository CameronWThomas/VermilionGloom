public class VampireSecret : Secret
{
    private VampireSecret() { }
    private VampireSecret(VampireSecret secret) : base(secret) { }

    public override SecretIconIdentifier Identifier => SecretIconIdentifier.VampireKnowledge;

    public override bool NoCharactersInvolved => true;

    protected override Secret Copy() => new VampireSecret(this);

    protected override string CreateDescription() => $"{CurrentSecretOwner.Name} knows there is a vampire in the manor";

    public class Builder : SecretTypeBuilder<VampireSecret>
    {
        public Builder(CharacterID secretOwner) : base(secretOwner, SecretLevel.Vampiric) { }

        public override VampireSecret Build()
        {
            var vampireSecret = new VampireSecret();
            Init(vampireSecret);
            return vampireSecret;
        }
    }
}