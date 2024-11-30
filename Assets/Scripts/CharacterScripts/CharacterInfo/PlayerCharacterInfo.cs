using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class PlayerCharacterInfo : CharacterInfo
{
    [SerializeReference] List<NPCHumanCharacterID> _knownCharacters = new();

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

    public void AddProbedCharacter(NPCHumanCharacterID probedCharacter)
    {
        if (_knownCharacters == null)
            _knownCharacters = new();

        if (!_knownCharacters.Contains(probedCharacter))
            _knownCharacters.Add(probedCharacter);

        // get all characters in the same room as the probed character

        List<CharacterID> inRoom = RoomBB.Instance.GetCharactersInMyRoom(probedCharacter).ToList();
        foreach (CharacterID character in inRoom)
        {
            if(character is NPCHumanCharacterID)
            {
                NPCHumanCharacterID npc = character as NPCHumanCharacterID;
                if (!_knownCharacters.Contains(npc))
                    _knownCharacters.Add(npc);
            }
        }


        var secrets = CharacterSecretKnowledgeBB.Instance.GetSecrets(probedCharacter);
        var secretTargets = secrets.Where(x => x.HasSecretTarget).Select(x => x.SecretTarget);
        var additionalCharacters = secrets.Where(x => x.HasAdditionalCharacter).Select(x => x.AdditionalCharacter);

        foreach (var character in secretTargets.Concat(additionalCharacters).OfType<NPCHumanCharacterID>())
            AddKnownCharacter(character);


        UpdateNameplates();
    }

    public void AddKnownCharacter(NPCHumanCharacterID knownCharacter)
    {
        if (!_knownCharacters.Contains(knownCharacter))
            _knownCharacters.Add(knownCharacter);

        UpdateNameplates();
    }

    public IEnumerable<CharacterID> GetKnownCharacters(bool includePlayerID)
    {
        if (includePlayerID)
            yield return ID;

        foreach (var character in _knownCharacters)
            yield return character;

        UpdateNameplates();
    }

    private void UpdateNameplates()
    {
        foreach (var character in _knownCharacters)
        {
            var characterInfo = CharacterInfoBB.Instance.GetCharacterInfo(character);
            if (characterInfo != null)
                characterInfo.nameplate.text = characterInfo.Name;
        }
    }

    protected override CharacterID CreateCharacterID() => new PlayerCharacterID();
}