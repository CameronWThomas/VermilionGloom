using System;

public class MurderSecret : Secret
{
    private bool _isJustified;
    private bool _isAttempt;

    private MurderSecret(bool isJustifier, bool isAttempt)
    {
        _isJustified = isJustifier;
        _isAttempt = isAttempt;
    }

    private MurderSecret(MurderSecret secret) : base(secret)
    {
        _isJustified = secret._isJustified;
        _isAttempt = secret._isAttempt;
    }

    public bool IsJustified => _isJustified;
    public bool IsAttempt => _isAttempt;

    public override SecretIconIdentifier Identifier => SecretIconIdentifier.Murder;

    protected override Secret Copy() => new MurderSecret(this);

    public override string CreateDescription()
    {
        string killLine;
        if (_isAttempt)
            killLine = _isJustified ? "attacked" : "tried to murder";
        else
            killLine = _isJustified ? "killed" : "murdered";

        var murdererName = SecretTarget.Name;
        var victimName = GetVictimName();

        return $"{murdererName} {killLine} {victimName}";
    }

    private string GetVictimName()
    {
        if (!HasAdditionalCharacter)
            return "someone long ago...";

        return AdditionalCharacter.Name;
    }

    public class Builder : SecretTypeBuilder<MurderSecret>
    {
        private CharacterID _murderer = null;
        private CharacterID _victim = null;
        private bool? _isJustified = null;
        private bool? _isAttempt = null;

        public Builder(CharacterID secretOwner, SecretLevel secretLevel) : base(secretOwner, secretLevel) { }

        public Builder SetMurderer(CharacterID murderer)
        {
            _murderer = murderer;
            return this;
        }

        public Builder SetVictim(CharacterID victim)
        {
            _victim = victim;
            return this;
        }

        public Builder IsJustified() => SetIsJustified(true);
        public Builder IsNotJustified() => SetIsJustified(false);

        public Builder WasSuccessfulMuder() => SetIsAttempt(true);
        public Builder WasAttempt() => SetIsAttempt(false);
        
        public override MurderSecret Build()
        {
            ValidateNotNull(_murderer, nameof(_murderer));
            ValidateNotNull(_isJustified, nameof(_isJustified));

            var murderSecret = new MurderSecret(_isJustified.Value, _isAttempt.Value);
            Init(murderSecret, _murderer, _victim);

            return murderSecret;
        }

        private Builder SetIsJustified(bool isJusitified)
        {
            _isJustified = isJusitified;
            return this;
        }

        private Builder SetIsAttempt(bool isAttempt)
        {
            _isAttempt = isAttempt;
            return this;
        }
    }
}