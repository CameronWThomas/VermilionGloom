using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering.UI;

[Serializable]
public class Relationship
{
    private readonly CharacterSecretKnowledge _secretKnowledge;
    private readonly CharacterID _relationshipTarget;

    [SerializeField] private CharacterInfo _characterInfo;

    [SerializeField] private bool _isDead = false;
    [SerializeField] private bool _isHostileTowards = false;

    public Relationship(CharacterSecretKnowledge ownerSecretKnowledge, CharacterID relationshipTarget)
    {
        _secretKnowledge = ownerSecretKnowledge;
        _relationshipTarget = relationshipTarget;

        _characterInfo = CharacterInfoBB.Instance.GetCharacterInfo(_relationshipTarget);
    }

    public CharacterID RelationshipTarget => _relationshipTarget;
    public bool IsHostileTowards => _isHostileTowards;
    public bool IsDead => _isDead;

    private List<Secret> RelevantSecrets => _secretKnowledge.Secrets.Where(IsRelevantSecret).ToList();

    public void Reevaluate()
    {
        _isDead = RelevantSecrets.OfType<MurderSecret>().Any(x => !x.IsAttempt && x.AdditionalCharacter == RelationshipTarget);
        _isHostileTowards = GetIsHostileTowards();
    }

    private bool GetIsHostileTowards()
    {
        if (_isDead)
            return false;

        if (RelevantSecrets.OfType<MurderSecret>().Any(x => !x.IsJustified))
            return true;

        return false;
    }

    private bool IsRelevantSecret(Secret secret)
    {
        if (secret.Level is SecretLevel.Vampiric)
            return true;

        if (secret.HasSecretTarget && secret.SecretTarget == _relationshipTarget)
            return true;

        // Check if we know this person was murdered
        if (secret is MurderSecret && secret.AdditionalCharacter == RelationshipTarget)
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