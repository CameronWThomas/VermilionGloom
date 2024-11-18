public class NameSecret : Secret
{
    private readonly string _name;

    public NameSecret(string name, SecretLevel level, CharacterID secretOwner)
        : base(level, secretOwner)
    {
        _name = name;
    }

    private NameSecret(NameSecret secret) : base(secret)
    {
        _name = secret._name;
    }

    public override SecretIconIdentifier Identifier => SecretIconIdentifier.Name;

    public override Secret Copy() => new NameSecret(this);

    public override string CreateDescription() => $"{SecretOwner.Name} has another name: {_name}";

}