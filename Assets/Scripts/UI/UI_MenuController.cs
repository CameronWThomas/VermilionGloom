using System;

public class UI_MenuController : GlobalSingleInstanceMonoBehaviour<UI_MenuController>
{
    private enum MenuState
    {
        Off,
        CharactersSecret
    }

    private MenuState _state = MenuState.Off;
    private NpcBrain _targetNPC;

    private UI_CharacterSecretsMenu _charactersSecretMenu;

    protected override void Start()
    {
        base.Start();

        _charactersSecretMenu = GetComponent<UI_CharacterSecretsMenu>();

        OnStateChange();
    }


    public void TalkToNPC(NpcBrain brain)
    {
        if (!TryChangeState(MenuState.CharactersSecret))
            return;

        _targetNPC = brain;
        OnStateChange();
    }

    public void CloseMenu()
    {
        if (TryChangeState(MenuState.Off))
            OnStateChange();
    }

    private bool TryChangeState(MenuState newState)
    {
        if (_state == newState)
            return false;

        _state = newState;
        return true;
    }

    private void OnStateChange()
    {
        switch (_state)
        {
            case MenuState.Off:
                _charactersSecretMenu.Close();
                MouseReceiver.Instance.Activate();
                break;

            case MenuState.CharactersSecret:
                MouseReceiver.Instance.Deactivate();
                _charactersSecretMenu.Open(_targetNPC);
                break;
        }
    }    
}
