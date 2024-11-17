using UnityEngine;

public abstract class Secret
{
    private string _description = null;

    protected Secret(SecretLevel level, CharacterID secretOwner, CharacterID additionalCharacter = null)
    {
        Level = level;
        SecretOwner = secretOwner ?? throw new System.Exception($"{nameof(secretOwner)} must be assigned");
        AdditionalCharacter = additionalCharacter;
    }

    protected Secret(Secret secret)
    {
        Level = secret.Level;
        SecretOwner = secret.SecretOwner;
        AdditionalCharacter = secret.AdditionalCharacter;
    }

    public abstract SecretIconIdentifier Identifier { get; }

    public CharacterID SecretOwner { get; }
    public CharacterID AdditionalCharacter { get; }
    public SecretLevel Level { get; }
    public bool IsRevealed { get; private set; }

    public string Description => _description ??= CreateDescription();
    public bool HasAdditionalCharacter => AdditionalCharacter != null;
    public Texture2D IconTexture => IsRevealed ? SecretResources.Instance.GetTexture(Identifier) : SecretResources.Instance.UnrevealedIconTexture;
        
    public abstract string CreateDescription();
    public abstract Secret Copy();

    public void ForceRevealSecret() => IsRevealed = true;
}
