using System;
using UnityEngine;

public abstract class CharacterID : IEquatable<CharacterID>
{
    private readonly Guid _id;

    public CharacterID()
    {
        _id = Guid.NewGuid();
    }

    public string Name => InternalCharacterInfo.Name;
    public Texture2D PortraitContent => CharacterPortraitContentBB.Instance.GetPortrait(this);
    public Color PortraitColor => CharacterPortraitContentBB.Instance.GetPortraitColor(this);    

    protected CharacterInfo InternalCharacterInfo => CharacterInfoBB.Instance.GetCharacterInfo(this);

    public bool Equals(CharacterID other) => other == this;
    public override bool Equals(object obj) => (obj as CharacterID) == this;
    public override int GetHashCode() => _id.GetHashCode();

    public static bool operator==(CharacterID left, CharacterID right)
    {
        if (left is null && right is null) return true;
        if (left is null || right is null) return false;
        return left._id == right._id;
    }

    public static bool operator !=(CharacterID left, CharacterID right)
    {
        if (left is null && right is null) return false;
        if (left is null || right is null) return true;
        return left._id != right._id;
    }
}