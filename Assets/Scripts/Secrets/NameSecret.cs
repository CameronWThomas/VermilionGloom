public class NameSecret : Secret
{
    private SecretLevel _level;
    private string _name;

    public NameSecret(string name) : this(name, SecretLevel.Public)
    { }

    public NameSecret(string name, SecretLevel level)
    {
        _name = name;
        _level = level;
    }

    public override SecretLevel Level => _level;

    public override SecretIconIdentifier Identifier => SecretIconIdentifier.Name;

    public override string Description => $"They have another name: {_name}";

}