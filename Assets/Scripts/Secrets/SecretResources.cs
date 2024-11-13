using System.Collections.Generic;
using UnityEngine;

public enum SecretLevel
{
    /// <summary>
    /// Always succeed in looking for public secrets. Other levels are chance based or depend on v-bucks
    /// </summary>
    Public,

    Private,
    Confidential,
    Vampiric
}

public enum SecretIconIdentifier
{
    Generic,
    Name,
    VampireKnowledge,
    Murder,
    Room
}

public class SecretResources : GlobalSingleInstanceMonoBehaviour<SecretResources>
{
    [SerializeField] private Texture2D _genericIconTexture;
    [SerializeField] private Texture2D _nameIconTexture;
    [SerializeField] private Texture2D _vampireKnowledgeIconTexture;
    [SerializeField] private Texture2D _murderIconTexture;
    [SerializeField] private Texture2D _roomIconTexture;

    private readonly Dictionary<SecretIconIdentifier, Texture2D> _secretIconTextures = new();

    protected override void Start()
    {
        base.Start();

        _secretIconTextures.Add(SecretIconIdentifier.Generic, _genericIconTexture);
        _secretIconTextures.Add(SecretIconIdentifier.Name, _nameIconTexture);
        _secretIconTextures.Add(SecretIconIdentifier.VampireKnowledge, _vampireKnowledgeIconTexture);
        _secretIconTextures.Add(SecretIconIdentifier.Murder, _murderIconTexture);
        _secretIconTextures.Add(SecretIconIdentifier.Room, _roomIconTexture);
    }

    public Texture2D GetTexture(SecretIconIdentifier identifier)
    {
        if (_secretIconTextures.ContainsKey(identifier))
            return _secretIconTextures[identifier];

        Debug.LogError($"Missing texture for {identifier}");
        return new Texture2D(512, 512);
    }
}
