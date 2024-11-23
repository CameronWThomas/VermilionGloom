using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CharacterInfoBB : GlobalSingleInstanceMonoBehaviour<CharacterInfoBB>
{
    private List<CharacterInfo> _characterInfos = new();

    public void Register(CharacterInfo characterInfo)
    {
        _characterInfos.Add(characterInfo);
    }

    public CharacterInfo GetCharacterInfo(CharacterID id) => _characterInfos.First(x => x.ID == id);

    public NPCHumanCharacterInfo GetCharacterInfo(NPCHumanCharacterID id)
        => _characterInfos.OfType<NPCHumanCharacterInfo>().First(x => x.NPCHumanCharacterID == id);

    public List<CharacterInfo> GetAll() => _characterInfos.ToList();    
}