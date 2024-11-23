using System;
using UnityEngine;

//TODO update to be serializable
public abstract class Secret
{
    string _description = null;
    Guid _secretID;

    protected Secret() { }

    protected Secret(Secret secret)
    {
        Level = secret.Level;
        OriginalSecretOwner = secret.OriginalSecretOwner;
        SecretTarget = secret.SecretTarget;
        AdditionalCharacter = secret.AdditionalCharacter;
        _secretID = secret._secretID;
    }

    public abstract SecretIconIdentifier Identifier { get; }

    public CharacterID OriginalSecretOwner { get; private set; }
    public CharacterID CurrentSecretOwner { get; private set; }
    public CharacterID SecretTarget { get; private set; }
    public CharacterID AdditionalCharacter { get; private set; }

    public SecretLevel Level { get; private set; }

    public bool IsRevealed { get; private set; }

    public string Description => _description ??= CreateDescription();
    public bool HasSecretTarget => SecretTarget != null;
    public bool HasAdditionalCharacter => AdditionalCharacter != null;
    public Texture2D IconTexture => IsRevealed ? SecretResources.Instance.GetTexture(Identifier) : SecretResources.Instance.UnrevealedIconTexture;
    public bool IsASpreadSecret => CurrentSecretOwner != OriginalSecretOwner;

    public virtual bool NoCharactersInvolved => !HasSecretTarget;

    protected abstract string CreateDescription();
    protected abstract Secret Copy();

    protected void ResetDescription()
    {
        _description = CreateDescription();
    }

    public bool IsSameSecret(Secret other)
    {
        return _secretID == other._secretID;
    }

    public void UpdateSecretLevel(SecretLevel newLevel)
    {
        Level = newLevel;
    }

    public void Reveal() => IsRevealed = true;

    public Secret CreateSpreadedCopy(CharacterID newSecretOwner)
    {
        var secret = Copy();
        secret.CurrentSecretOwner = newSecretOwner;
        return secret;
    }


    protected void InitializeNew(SecretLevel level,
        CharacterID secretOwner,
        CharacterID secretTarget = null,
        CharacterID additionalCharacter = null)
    {
        _secretID = Guid.NewGuid();
        Level = level;
        CurrentSecretOwner = secretOwner ?? throw new System.Exception($"{nameof(secretOwner)} must be assigned");
        OriginalSecretOwner = secretOwner;
        SecretTarget = secretTarget;
        AdditionalCharacter = additionalCharacter;
    }


    public abstract class SecretTypeBuilder<TSecret> where TSecret : Secret
    {
        protected CharacterID _secretOwner;
        protected SecretLevel? _secretLevel;

        public SecretTypeBuilder(CharacterID secretOwner, SecretLevel? secretLevel)
        {
            _secretOwner = secretOwner;
            _secretLevel = secretLevel;
        }

        public abstract TSecret Build();

        protected void Init(TSecret secret, CharacterID target = null, CharacterID additionalCharacter = null)
        {
            ValidateHasSecretLevel();
            secret.InitializeNew(_secretLevel.Value, _secretOwner, target, additionalCharacter);
        }

        protected void ValidateHasSecretLevel()
        {
            if (!_secretLevel.HasValue)
                throw new Exception($"{nameof(TSecret)} must be assigned a secret level");
        }
        
        protected void ValidateNotNull(object nullableObject, string propertyName)
        {
            if (nullableObject == null)
                throw new Exception($"{nameof(TSecret)} must have {propertyName} assigned");
        }
    }
}
