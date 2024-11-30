using System;
using UnityEngine;

public class UI_SectionBase : MonoBehaviour
{
    protected NPCHumanCharacterID _characterID;
    protected NPCHumanCharacterInfo _characterInfo;
    protected CharacterSecretKnowledge _characterSecretKnowledge;


    private Func<CharacterInteractingState> _getState;
    private CharacterInteractingState _lastState;

    protected UI_CharacterInteractionMenu CharacterInteractionMenu => GetComponent<UI_CharacterInteractionMenu>();
    protected UI_MiniGameSection MiniGameSection => GetComponent<UI_MiniGameSection>();

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
        _characterSecretKnowledge = _characterInfo.GetComponent<CharacterSecretKnowledge>();

        _lastState = getState();

        OnStateChanged(_lastState);
    }

    public void Hide() => UpdateHidden(true);
    public void Unhide() => UpdateHidden(false);

    public virtual void Deactivate()
    {
        _characterID = null;
        _characterInfo = null;
        _getState = null;
        _lastState = CharacterInteractingState.NA;
    }

    protected virtual void UpdateHidden(bool hide) { }
    protected virtual void OnStateChanged(CharacterInteractingState state) { }    
}