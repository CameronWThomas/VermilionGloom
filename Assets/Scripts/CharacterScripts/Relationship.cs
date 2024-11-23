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
        CheckIfAnyMurdersShouldBeJustified();

        _isDead = RelevantSecrets.OfType<MurderSecret>().Any(x => !x.IsAttempt && x.AdditionalCharacter == RelationshipTarget);
        _isHostileTowards = GetIsHostileTowards();
    }

    private void CheckIfAnyMurdersShouldBeJustified()
    {
        var murderSecrets = RelevantSecrets.OfType<MurderSecret>().ToList();
        var draggedSecrets = RelevantSecrets.OfType<MurderSecret>().ToList();

        var unjustifiedMurders = murderSecrets.Where(x => x.SecretTarget == RelationshipTarget && !x.IsJustified).ToList();

        // Check if any unjustified murder comitted by the character was against someone who comitted an unjustified murder. They become justified if that is the case.
        foreach (var unjustifiedMurder in unjustifiedMurders)
        {
            var murdererWasVictimOfUnjustifiedMurder = _secretKnowledge.Secrets.OfType<MurderSecret>().Any(x => x.IsAttempt && !x.IsJustified && x.AdditionalCharacter == unjustifiedMurder.SecretTarget);
            if (!murdererWasVictimOfUnjustifiedMurder)
                continue;

            unjustifiedMurder.UpdateJustificationOrAttempt(true, unjustifiedMurder.IsAttempt);
        }
    }

    private bool GetIsHostileTowards()
    {
        if (_isDead)
            return false;

        var murderSecrets = RelevantSecrets.OfType<MurderSecret>().ToList();
        var dragSecrets = RelevantSecrets.OfType<DragSecret>().ToList();

        if (murderSecrets.Any(x => !x.IsJustified) || dragSecrets.Any(x => x.SecretTarget == RelationshipTarget))
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
        if (secret.HasAdditionalCharacter && secret.AdditionalCharacter == RelationshipTarget)
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