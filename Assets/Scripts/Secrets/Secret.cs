using System;
using UnityEngine;

[Serializable]
public abstract class Secret
{
    [SerializeField] string _secretID;
    [SerializeReference] CharacterID _originalSecretOwner;
    [SerializeReference] CharacterID _currentSecretOwner;
    [SerializeReference] CharacterID _secretTarget;
    [SerializeReference] CharacterID _additionalCharacter;
    [SerializeField] SecretLevel _level;
    //[SerializeField] bool _isRevealed = true;

    string _description = null;

    protected Secret() { }

    protected Secret(Secret secret)
    {
        _level = secret.Level;
        _originalSecretOwner = secret.OriginalSecretOwner;
        _secretTarget = secret.SecretTarget;
        _additionalCharacter = secret.AdditionalCharacter;
        _secretID = secret._secretID;
        //_isRevealed = false;
    }

    public abstract SecretIconIdentifier Identifier { get; }

    public CharacterID OriginalSecretOwner => _originalSecretOwner;
    public CharacterID CurrentSecretOwner => _currentSecretOwner;
    public CharacterID SecretTarget => _secretTarget;
    public CharacterID AdditionalCharacter => _additionalCharacter;

    public SecretLevel Level => _level;

    //public bool IsRevealed => _isRevealed;

    public string Description => _description ??= CreateDescription();
    public bool HasSecretTarget => SecretTarget != null;
    public bool HasAdditionalCharacter => AdditionalCharacter != null;
    //public Texture2D IconTexture => IsRevealed ? SecretResources.Instance.GetTexture(Identifier) : SecretResources.Instance.UnrevealedIconTexture;
    public Texture2D IconTexture => SecretResources.Instance.GetTexture(Identifier);
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

    //public void Reveal() => _isRevealed = true;

    public Secret CreateSpreadedCopy(CharacterID newSecretOwner)
    {
        var secret = Copy();
        secret._currentSecretOwner = newSecretOwner;
        return secret;
    }

    protected void InitializeNew(SecretLevel level,
        CharacterID secretOwner,
        CharacterID secretTarget = null,
        CharacterID additionalCharacter = null)
    {
        _secretID = Guid.NewGuid().ToString();
        _level = level;
        _currentSecretOwner = secretOwner ?? throw new System.Exception($"{nameof(secretOwner)} must be assigned");
        _originalSecretOwner = secretOwner;
        _secretTarget = secretTarget;
        _additionalCharacter = additionalCharacter;
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
