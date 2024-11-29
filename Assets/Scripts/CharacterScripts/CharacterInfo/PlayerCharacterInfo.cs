using NUnit.Framework;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering.Universal;
using UnityEngine.Video;

public class PlayerCharacterInfo : CharacterInfo
{
    [SerializeReference] List<NPCHumanCharacterID> _probedCharacters = new();

    public override void Die()
    {
        if (GameState.Instance.GameWon)
            return;

        base.Die();

        GetComponent<MvmntController>().enabled = false;
        GetComponent<NavMeshAgent>().enabled = false;
    }

    public override bool Damage()
    {
        if (GameState.Instance.GameWon)
            return false;

        UI_CharacterInteractionMenu.Instance.Deactivate();

        return base.Damage();
    }

    public override void ReturnToLife()
    {
        GetComponent<MvmntController>().enabled = true;
        GetComponent<NavMeshAgent>().enabled = true;
    }

    public override CharacterType CharacterType => CharacterType.Player;

    public void AddProbedCharacter(NPCHumanCharacterID character)
    {
        if (_probedCharacters == null)
            _probedCharacters = new();

        if (!_probedCharacters.Contains(character))
            _probedCharacters.Add(character);
    }

    public IEnumerable<CharacterID> GetKnownCharacters(bool includePlayerID)
    {
        if (includePlayerID)
            yield return ID;

        if (_probedCharacters == null)
            yield break;

        // Look through the characters secret that we have probed. Take all characters we know about from the secrets
        var returnedCharacters = new List<CharacterID> { ID };
        foreach (var probedCharacter in _probedCharacters)
        {
            if (!returnedCharacters.Contains(probedCharacter))
            {
                yield return probedCharacter;
                returnedCharacters.Add(probedCharacter);
            }

            var secrets = CharacterSecretKnowledgeBB.Instance.GetSecrets(probedCharacter);

            var secretTargets = secrets.Where(x => x.HasSecretTarget).Select(x => x.SecretTarget);
            var additionalCharacters = secrets.Where(x => x.HasAdditionalCharacter).Select(x => x.AdditionalCharacter);
            var characters = secretTargets.Concat(additionalCharacters).ToList();

            foreach (var character in characters)
            {
                if (!returnedCharacters.Contains(character))
                {
                    yield return character;
                    returnedCharacters.Add(character);
                }
            }
        }
    }

    protected override CharacterID CreateCharacterID() => new PlayerCharacterID();
}