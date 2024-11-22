using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Relationship
{
    private readonly CharacterSecretKnowledge _secretKnowledge;
    private readonly CharacterID _relationshipTarget;

    private readonly CharacterInfo _characterInfo;

    public Relationship(CharacterSecretKnowledge ownerSecretKnowledge, CharacterID relationshipTarget)
    {
        _secretKnowledge = ownerSecretKnowledge;
        _relationshipTarget = relationshipTarget;

        _characterInfo = CharacterInfoBB.Instance.GetCharacterInfo(_relationshipTarget);
    }

    public bool IsHostile => GetIsHostile();
    public bool IsDead => _characterInfo.IsDead;

    private IEnumerable<Secret> RelevantSecrets
        => _secretKnowledge.Secrets.Where(IsRelevantSecret);
    
    private bool GetIsHostile()
    {
        if (RelevantSecrets.OfType<MurderSecret>().Any(x => !x.IsJustified))
            return false;

        return false;
    }

    private bool IsRelevantSecret(Secret secret)
    {
        if (secret.Level is SecretLevel.Vampiric)
            return true;

        if (secret.HasSecretTarget && secret.SecretTarget == _relationshipTarget)
            return true;

        return false;
    }
}

public static class RelationshipExtensions
{
    /// <summary>
    /// Creates a <see cref="Relationship"/> using the secrets that <paramref name="component"/> has
    /// </summary>
    public static Relationship Create(this Component component, CharacterID characterID)
        => new Relationship(component.GetComponent<CharacterSecretKnowledge>(), characterID);
}