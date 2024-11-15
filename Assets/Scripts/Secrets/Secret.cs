using UnityEngine;

public abstract class Secret
{
    private bool _isRevealed = false;

    public abstract SecretLevel Level { get; }
    public abstract SecretIconIdentifier Identifier { get; }
    public abstract string Description { get; }

    public virtual bool InvolvesCharacters => true;
    public virtual bool InvolvesMulitpleCharacters => false;

    public bool IsRevealed => _isRevealed;
    public Texture2D IconTexture => SecretResources.Instance.GetTexture(Identifier);

    public void ForceRevealSecret()
    {
        _isRevealed = true;
    }

    //TODO eventually add method to try and reveal where it is random and based on a modifier.
}
