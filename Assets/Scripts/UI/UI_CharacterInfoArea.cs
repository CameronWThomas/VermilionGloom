public class UI_CharacterInfoArea : UI_SectionBase
{
    UI_BottomBarController _bottomBarController;

    private void Start()
    {
        _bottomBarController = GetComponentInChildren<UI_BottomBarController>(true);
    }

    public override void InitializeForNewCharacter(NPCHumanCharacterID characterId)
    {
        base.InitializeForNewCharacter(characterId);
        _bottomBarController.SetInteractingCharacter(characterId);
    }

    public override void Deactivate()
    {
        base.Deactivate();
        _bottomBarController.Default();
    }
}