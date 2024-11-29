using UnityEngine;
using UnityEngine.UI;

public enum CharacterInteractingState
{
    NA,
    Default,
    Unprobed,
    Trance
}

public class UI_CharacterInteractionMenu : GlobalSingleInstanceMonoBehaviour<UI_CharacterInteractionMenu>
{
    [SerializeField] GameObject _characterInteractionContent;
    [SerializeField] Button _exitButton;
    [SerializeField] RectTransform _mainScreen;
    [SerializeField] RectTransform _miniGameScreen;

    private NPCHumanCharacterID _characterID;
    CharacterInteractingState _state = CharacterInteractingState.NA;
    private bool _isTransitioning = false;

    private UI_SecretsArea SecretsArea => GetComponent<UI_SecretsArea>();
    private UI_ScreenTransistion ScreenTransistion => GetComponent<UI_ScreenTransistion>();
    private UI_VampirePowers VampirePowers => GetComponent<UI_VampirePowers>();
    private UI_CharacterInfoArea CharacterInfo => GetComponent<UI_CharacterInfoArea>();
    private UI_TranceMenu TranceMenu => GetComponent<UI_TranceMenu>();

    protected override void Start()
    {
        base.Start();
        Deactivate();

        _exitButton.onClick.AddListener(OnExitButtonClicked);

        ScreenTransistion.Initialize(_mainScreen);
    }    

    public void Activate(NPCHumanCharacterID characterID)
    {
        _characterID = characterID;

        NpcBehaviorBB.Instance.EnterConversationWithPlayer(_characterID);
        MouseReceiver.Instance.Deactivate();

        _characterInteractionContent.SetActive(true);
        _mainScreen.gameObject.SetActive(true);

        //TODO make new class for
        _miniGameScreen.gameObject.SetActive(false);

        var characterInfo = CharacterInfoBB.Instance.GetCharacterInfo(characterID);
        _state = characterInfo.MindProbed ? CharacterInteractingState.Default : CharacterInteractingState.Unprobed;

        SecretsArea.InitializeForNewCharacter(characterID, () => _state);
        VampirePowers.InitializeForNewCharacter(characterID, () => _state);
        CharacterInfo.InitializeForNewCharacter(characterID, () => _state);
        TranceMenu.InitializeForNewCharacter(characterID, () => _state);
    }

    public void Deactivate()
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

    private void OnForgetClicked()
    {

    }

    public void TransitionState(CharacterInteractingState newState, UI_ScreenTransistion.TransitionType transition = UI_ScreenTransistion.TransitionType.FromCenter)
    {
        if (_isTransitioning)
            return;

        _isTransitioning = true;
        ScreenTransistion.Transition(transition, () => _state = newState, () => _isTransitioning = false);
    }
    private void OnExitButtonClicked()
    {
        Deactivate();

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