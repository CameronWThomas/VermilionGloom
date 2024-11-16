using System.Collections.Generic;
using System.Linq;

public class CharacterInfoBB : GlobalSingleInstanceMonoBehaviour<CharacterInfoBB>
{
    private List<CharacterInfo> _characterInfos = new();

    public void Register(CharacterInfo characterInfo)
    {
        _characterInfos.Add(characterInfo);
    }

    public int CharacterCount => _characterInfos.Count;

    public CharacterInfo GetCharacterInfo(CharacterID id) => _characterInfos.First(x => x.ID == id);
}