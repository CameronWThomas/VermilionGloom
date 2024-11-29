using System;

public class UI_CharacterInfoArea : UI_SectionBase
{
    UI_BottomBarController _bottomBarController;

    private void Start()
    {
        _bottomBarController = GetComponentInChildren<UI_BottomBarController>(true);
    }

    public override void InitializeForNewCharacter(NPCHumanCharacterID characterId, Func<CharacterInteractingState> getState)
    {
        base.InitializeForNewCharacter(characterId, getState);
        _bottomBarController.SetInteractingCharacter(characterId);
    }

    public override void Deactivate()
    {
        base.Deactivate();

        if (_bottomBarController != null)
            _bottomBarController.Default();
    }
}