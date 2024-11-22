using System;
using UnityEngine;

public abstract class Secret
{
    private string _description = null;
    private readonly Guid _secretID;

    protected Secret(SecretLevel level, CharacterID secretOwner, CharacterID additionalCharacter = null)
    {
        _secretID = Guid.NewGuid();
        Level = level;
        SecretOwner = secretOwner ?? throw new System.Exception($"{nameof(secretOwner)} must be assigned");
        AdditionalCharacter = additionalCharacter;
    }

    protected Secret(Secret secret)
    {
        Level = secret.Level;
        SecretOwner = secret.SecretOwner;
        AdditionalCharacter = secret.AdditionalCharacter;
        _secretID = secret._secretID;
    }

    public abstract SecretIconIdentifier Identifier { get; }

    public CharacterID SecretOwner { get; }
    public CharacterID AdditionalCharacter { get; }
    public SecretLevel Level { get; }
    public bool IsRevealed { get; private set; }

    public string Description => _description ??= CreateDescription();
    public bool HasAdditionalCharacter => AdditionalCharacter != null;
    public Texture2D IconTexture => IsRevealed ? SecretResources.Instance.GetTexture(Identifier) : SecretResources.Instance.UnrevealedIconTexture;

    public virtual bool NoCharactersInvolved => false;

    public abstract string CreateDescription();
    public abstract Secret Copy();

    public bool IsSameSecret(Secret other)
    {
        return _secretID == other._secretID;
    }

    public void Reveal() => IsRevealed = true;
}
