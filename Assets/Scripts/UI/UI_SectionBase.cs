using System.Diagnostics;
using UnityEngine;

public class UI_SectionBase : MonoBehaviour
{
    protected NPCHumanCharacterID _characterID;
    protected NPCHumanCharacterInfo _characterInfo;
    bool _lastMindProbed = false;

    private void Update()
    {
        if (_characterInfo == null)
            return;

        if (_characterInfo.MindProbed == _lastMindProbed)
            return;

        _lastMindProbed = _characterInfo.MindProbed;

        OnProbeMindChange(_lastMindProbed);
    }

    public virtual void InitializeForNewCharacter(NPCHumanCharacterID characterId)
    {
        _characterID = characterId;
        _characterInfo = CharacterInfoBB.Instance.GetCharacterInfo(characterId);
        _lastMindProbed = _characterInfo.MindProbed;
        OnProbeMindChange(_lastMindProbed);
    }

    public void Hide() => UpdateHidden(true);
    public void Unhide() => UpdateHidden(false);

    public virtual void Deactivate()
    {
        _characterID = null;
        _characterInfo = null;
    }

    protected virtual void UpdateHidden(bool hide) { }
    protected virtual void OnProbeMindChange(bool mindProbed) { }    
}