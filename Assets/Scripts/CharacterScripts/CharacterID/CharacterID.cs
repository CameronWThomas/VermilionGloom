using System;
using UnityEngine;

[Serializable]
public abstract class CharacterID : IEquatable<CharacterID>
{
    [SerializeReference] private string _id;

    protected CharacterID()
    {
        _id = Guid.NewGuid().ToString();
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