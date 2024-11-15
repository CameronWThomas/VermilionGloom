using System;
using UnityEngine;

public abstract class Secret
{
    private bool _isRevealed = false;

    public abstract SecretLevel Level { get; }
    public abstract SecretIconIdentifier Identifier { get; }
    public abstract string Description { get; }

    public virtual bool HasTarget => TargetCharacter != null;
    public virtual CharacterInfo TargetCharacter => null;

    public bool IsRevealed => _isRevealed;
    public Texture2D IconTexture => SecretResources.Instance.GetTexture(Identifier);

    public void ForceRevealSecret()
    {
        _isRevealed = true;
    }

    public abstract Secret Copy();

    //TODO eventually add method to try and reveal where it is random and based on a modifier.
}
