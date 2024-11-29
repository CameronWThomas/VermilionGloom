using System;
using UnityEngine;

public class UI_SectionBase : MonoBehaviour
{
    protected NPCHumanCharacterID _characterID;
    protected NPCHumanCharacterInfo _characterInfo;

    private Func<CharacterInteractingState> _getState;
    private CharacterInteractingState _lastState;

    protected UI_CharacterInteractionMenu CharacterInteractionMenu => GetComponent<UI_CharacterInteractionMenu>();

    private void Update()
    {
        if (_getState == null)
            return;

        var newState = _getState();
        if (newState == _lastState)
            return;

        _lastState = newState;
        OnStateChanged(_lastState);
    }

    public virtual void InitializeForNewCharacter(NPCHumanCharacterID characterId, Func<CharacterInteractingState> getState)
    {
        _characterID = characterId;
        _getState = getState;

        _characterInfo = CharacterInfoBB.Instance.GetCharacterInfo(characterId);
        _lastState = getState();

        OnStateChanged(_lastState);
    }

    public void Hide() => UpdateHidden(true);
    public void Unhide() => UpdateHidden(false);

    public virtual void Deactivate()
    {
        _characterID = null;
        _characterInfo = null;
        _lastState = CharacterInteractingState.NA;
    }

    protected virtual void UpdateHidden(bool hide) { }
    protected virtual void OnStateChanged(CharacterInteractingState state) { }    
}