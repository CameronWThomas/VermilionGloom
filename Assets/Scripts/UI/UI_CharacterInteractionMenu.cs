using UnityEngine;
using UnityEngine.UI;

public enum CharacterInteractingState
{
    NA,
    Default,
    Unprobed,
    Trance,
    MiniGame
}

public class UI_CharacterInteractionMenu : GlobalSingleInstanceMonoBehaviour<UI_CharacterInteractionMenu>
{
    [SerializeField] GameObject _characterInteractionContent;
    [SerializeField] Button _exitButton;
    [SerializeField] RectTransform _mainScreen;

    private NPCHumanCharacterID _characterID;
    CharacterInteractingState _state = CharacterInteractingState.NA;
    private bool _isTransitioning = false;

    private UI_SecretsArea SecretsArea => GetComponent<UI_SecretsArea>();
    private UI_ScreenTransition ScreenTransistion => GetComponent<UI_ScreenTransition>();
    private UI_VampirePowers VampirePowers => GetComponent<UI_VampirePowers>();
    private UI_CharacterInfoArea CharacterInfo => GetComponent<UI_CharacterInfoArea>();
    private UI_TranceMenu TranceMenu => GetComponent<UI_TranceMenu>();
    private UI_MiniGameSection MiniGameSection => GetComponent<UI_MiniGameSection>();

    protected override void Start()
    {
        base.Start();

        _exitButton.onClick.AddListener(OnExitButtonClicked);

        ScreenTransistion.Initialize(_mainScreen);
        DeactivateInternal();
    }

    public void Activate(NPCHumanCharacterID characterID)
    {
        _characterID = characterID;

        NpcBehaviorBB.Instance.EnterConversationWithPlayer(_characterID);
        MouseReceiver.Instance.Deactivate();

        UI_BottomBarController.Instance.DisplayTutorialAndUpdateTutorialList(Tutorial.BaseMenu, Tutorial.Forget, Tutorial.Trance);

        _characterInteractionContent.SetActive(true);
        _mainScreen.gameObject.SetActive(true);

        var characterInfo = CharacterInfoBB.Instance.GetCharacterInfo(characterID);
        _state = characterInfo.MindProbed ? CharacterInteractingState.Default : CharacterInteractingState.Unprobed;

        SecretsArea.InitializeForNewCharacter(characterID, () => _state);
        VampirePowers.InitializeForNewCharacter(characterID, () => _state);
        CharacterInfo.InitializeForNewCharacter(characterID, () => _state);
        TranceMenu.InitializeForNewCharacter(characterID, () => _state);
        MiniGameSection.InitializeForNewCharacter(_characterID, () => _state);
    }

    public void Deactivate()
    {
        if (_characterID != null)
            DeactivateInternal();
    }    

    public void TransitionState(CharacterInteractingState newState, UI_ScreenTransition.TransitionType transition = UI_ScreenTransition.TransitionType.FromCenter, float? transitionSpeed = null, float? transitionDuration = null) 
    {
        if (_isTransitioning)
            return;

        _isTransitioning = true;
        ScreenTransistion.Transition(transition, () => _state = newState, () => _isTransitioning = false, transitionSpeed ,transitionDuration);
    }
    
    private void DeactivateInternal()
    {
        if (_characterID != null)
            NpcBehaviorBB.Instance.EndConversationWithPlayer(_characterID);

        UI_BottomBarController.Instance.Default();

        MouseReceiver.Instance.Activate();
        _characterInteractionContent.SetActive(false);

        SecretsArea.Deactivate();
        VampirePowers.Deactivate();
        CharacterInfo.Deactivate();
        TranceMenu.Deactivate();
    }

    private void OnExitButtonClicked()
    {
        DeactivateInternal();

        // TODO add transition (:
        //if (_isTransitioning)
        //    return;

        //_isTransitioning = true;
        //_exitButton.interactable = false;

        //ScreenTransistion.Transition(UI_ScreenTransistion.TransitionType.FromCenter,
        //    () => Deactivate(),
        //    () => 
        //    {
        //        _exitButton.interactable = true;
        //        _isTransitioning = false;
        //    });
    }




    //public void OnRevealSecretsPressed()
    //{
    //    if (_screenState != ScreenState.Normal)
    //        return;

    //    if (!_characterId.CharacterInfo.TrySetPendingDetectivePoints(1))
    //        return;

    //    _screenState = ScreenState.RevealingSecrets;

    //    _hideObjectsDuringAction.ForEach(x => x.SetActive(false));
    //    _secretRevealScreen.gameObject.SetActive(true);
    //    _secretRevealScreen.Initialize(_characterId, OnRevealScreenFinish);

    //}

    //public void OnActionComplete()
    //{
    //    _screenState = ScreenState.Normal;

    //    _hideObjectsDuringAction.ForEach(x => x.SetActive(true));
    //    _secretRevealScreen.gameObject.SetActive(false);
    //}

    //private void OnRevealScreenFinish(SecretLevel? level, bool gamePlayed)
    //{
    //    if (gamePlayed)
    //    {
    //        _characterId.CharacterInfo.TryUseDetectivePoint(1);

    //        if (level.HasValue)
    //            CharacterSecretKnowledgeBB.Instance.UnlockSecret(_characterId, level.Value);
    //    }
    //    else
    //        _characterId.CharacterInfo.TrySetPendingDetectivePoints(0);

    //    OnActionComplete();
    //}
}