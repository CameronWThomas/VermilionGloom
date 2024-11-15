public class VampireSecret : Secret
{
    public override Secret Copy() => new VampireSecret();

    public override SecretLevel Level => SecretLevel.Vampiric;

    public override SecretIconIdentifier Identifier => SecretIconIdentifier.VampireKnowledge;

    public override string Description => "Knows theres a vampire here";

    public override bool HasTarget => false;
}