using UnityEngine;

public abstract class Secret
{
    public abstract SecretLevel Level { get; }
    public abstract SecretIconIdentifier Identifier { get; }
    public abstract string Description { get; }

    public Texture2D IconTexture => SecretResources.Instance.GetTexture(Identifier);
}
