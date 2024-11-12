using System;

public class MenuController : GlobalSingleInstanceMonoBehaviour<MenuController>
{
    private enum MenuState
    {
        Off,
        TalkingMenuOpen
    }

    private MenuState _state = MenuState.Off;
    private TalkingMenu _talkingMenu;

    protected override void Start()
    {
        base.Start();

        _talkingMenu = GetComponent<TalkingMenu>();

        OnStateChange();
    }

    public void TalkToNPC(NpcBrain brain)
    {
        if (!TryChangeState(MenuState.TalkingMenuOpen))
            return;

        _talkingMenu.SetNpc(brain);
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
                _talkingMenu.Close();
                MouseReceiver.Instance.Activate();
                break;

            case MenuState.TalkingMenuOpen:
                MouseReceiver.Instance.Deactivate();
                _talkingMenu.Open();
                break;
        }
    }    
}
