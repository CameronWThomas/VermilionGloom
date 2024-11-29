using System;
using UnityEngine;

[Serializable]
public class MurderSecret : Secret
{
    [SerializeField]private bool _isAttempt;

    private MurderSecret(bool isAttempt)
    {
        _isAttempt = isAttempt;
    }

    private MurderSecret(MurderSecret secret) : base(secret)
    {
        _isAttempt = secret._isAttempt;
    }

    public bool IsAttempt => _isAttempt;

    public override SecretIconIdentifier Identifier => SecretIconIdentifier.Murder;

    public void UpdateIsAttempt(bool isAttempt)
    {
        _isAttempt = isAttempt;
        ResetDescription();
    }

    protected override Secret Copy() => new MurderSecret(this);

    protected override string CreateDescription()
    {
        var killLine = _isAttempt ? "attacked" : "murdered";

        var murdererName = GetMurdererName();
        var victimName = GetVictimName();

        return $"{murdererName} {killLine} {victimName}";
    }

    private string GetVictimName()
    {
        if (!HasAdditionalCharacter)
            return "someone";

        return AdditionalCharacter.Name;
    }

    private string GetMurdererName()
    {
        if (!HasSecretTarget)
            return "Someone";

        return SecretTarget.Name;
    }

    public class Builder : SecretTypeBuilder<MurderSecret>
    {
        private CharacterID _murderer = null;
        private CharacterID _victim = null;
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

        public Builder WasSuccessfulMuder() => SetIsAttempt(false);
        public Builder WasAttempt() => SetIsAttempt(true);
        public Builder SetIsAttempt(bool isAttempt)
        {
            _isAttempt = isAttempt;
            return this;
        }

        public override MurderSecret Build()
        {
            var murderSecret = new MurderSecret(_isAttempt.Value);
            Init(murderSecret, _murderer, _victim);

            return murderSecret;
        }        
    }
}