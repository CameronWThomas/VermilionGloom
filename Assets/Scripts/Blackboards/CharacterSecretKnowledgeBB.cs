using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class CharacterSecretKnowledgeBB : GlobalSingleInstanceMonoBehaviour<CharacterSecretKnowledgeBB>
{
    private const int INITIAL_RUMOUR_SECRETS_MAX = 3;

    [SerializeField, Range(0f, 1f)] public float _initialMurderChance = .25f;
    [SerializeField, Range(0f, 1f)] public float _randomNameChance = .1f;

    private Dictionary<NPCHumanCharacterID, CharacterSecretKnowledge> _secretKnowledgeDict = new();
    public void Register(NPCHumanCharacterID characterID, CharacterSecretKnowledge secretKnowledge)
    {
        _secretKnowledgeDict.Add(characterID, secretKnowledge);
    }

    public IReadOnlyList<Secret> GetSecrets(NPCHumanCharacterID characterId) => _secretKnowledgeDict[characterId].Secrets.ToList();

    /// <summary>
    /// Chooses a random secret for <paramref name="characterId"/> of level <paramref name="level"/> to reveal
    /// </summary>
    public void UnlockSecret(NPCHumanCharacterID characterId, SecretLevel level)
    {
        var secretKnowledge = _secretKnowledgeDict[characterId];

        // If we are revealing a secret for the owner, the first needs to be the room secret no matter what
        if (characterId.CharacterInfo.CharacterType is CharacterType.Owner)
        {
            var roomSecret = secretKnowledge.Secrets.OfType<RoomSecret>().First();
            if (!roomSecret.IsRevealed)
            {
                roomSecret.Reveal();
                return;
            }
        }

        var unlockedSecret = secretKnowledge.Secrets
            .Where(x => x.Level == level)
            .Randomize()
            .FirstOrDefault();

        unlockedSecret?.Reveal();
    }

    public bool TrySpreadSecret(NPCHumanCharacterID spreader, NPCHumanCharacterID target)
    {
        var spreadersSecrets = GetSecrets(spreader);
        var targetsSecrets = GetSecrets(target);

        var secretsUniqueToSpreader = spreadersSecrets.Where(x => targetsSecrets.All(theirSecret => !theirSecret.IsSameSecret(x)));
        var allowedToSpreadSecrets = secretsUniqueToSpreader.Where(SecretIsSpreadable);

        Secret secretToBeSpread = null;
        foreach (var secret in allowedToSpreadSecrets.Randomize())
        {
            if (secret.Level.RandomChance())
            {
                secretToBeSpread = secret;
                break;
            }
        }

        if (secretToBeSpread == null)
            return false;

        var targetSecretKnowledge = _secretKnowledgeDict[target];
        targetSecretKnowledge.AddSecret(secretToBeSpread.CreateSpreadedCopy(target));
        return true;
    }

    private bool SecretIsSpreadable(Secret secret)
    {
        // TODO this will be based on the progress in the game. Vampiric secrets won't spread until a certain point

        return secret.Level is not SecretLevel.Vampiric;
    }
}