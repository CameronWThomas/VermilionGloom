using System.Collections.Generic;
using System.Linq;

public class CharacterInfoBB : GlobalSingleInstanceMonoBehaviour<CharacterInfoBB>
{
    private List<CharacterInfo> _characterInfos = new();

    private bool _isInitialized = false;

    public void Register(CharacterInfo characterInfo)
    {
        _characterInfos.Add(characterInfo);
    }

    public int CharacterCount => _characterInfos.Count;

    public void Initialize()
    {
        if (_isInitialized) return;

        _isInitialized = true;


        var vanHelsingCharacter = _characterInfos.Where(x => !x.IsOwner).Randomize().First();
        foreach (var characterInfo in _characterInfos)
        {
            characterInfo.Initialize(characterInfo == vanHelsingCharacter);
        }
    }

    public CharacterInfo GetCharacterInfo(CharacterID id) => _characterInfos.First(x => x.ID == id);
}