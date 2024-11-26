using UnityEngine;

public class NameSecret : Secret
{
    [SerializeField] private string _name;

    private NameSecret(string name)
    {
        _name = name;
    }

    private NameSecret(NameSecret secret) : base(secret)
    {
        _name = secret._name;
    }

    public override SecretIconIdentifier Identifier => SecretIconIdentifier.Name;

    protected override Secret Copy() => new NameSecret(this);

    protected override string CreateDescription() => $"{SecretTarget.Name} has another name: {_name}";

    public class Builder : SecretTypeBuilder<NameSecret>
    {
        private string _name = null;

        public Builder(CharacterID secretOwner, SecretLevel secretLevel) : base(secretOwner, secretLevel) { }

        public Builder SetName(string name)
        {
            _name = name;
            return this;
        }

        public override NameSecret Build()
        {
            ValidateNotNull(_name, nameof(_name));

            var nameSecret = new NameSecret(_name);
            Init(nameSecret, target: _secretOwner);

            return nameSecret;
        }
    }

}