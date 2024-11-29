using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.HID;
using UnityEngine.UI;

public class UI_CharacterInteractionMenu : GlobalSingleInstanceMonoBehaviour<UI_CharacterInteractionMenu>
{
    [SerializeField] GameObject _characterInteractionContent;
    [SerializeField] Button _exitButton;
    [SerializeField] Button _backButton;

    [Header("Trance")]
    [SerializeField] RectTransform _murderSectionAdder;
    [SerializeField] Button _beginTrance;
    [SerializeField] GridLayoutGroup _portraitPlace;
    [SerializeField] GameObject _portraitButtonPrefab;

    [Header("Other")]
    [SerializeField] RectTransform _mainScreen;
    [SerializeField] RectTransform _miniGameScreen;
    [SerializeField] RectTransform _secretExaminingArea;

    //[SerializeField] private List<GameObject> _hideObjectsDuringAction = new();

    private NPCHumanCharacterID _characterId;

    private UI_SecretsArea SecretsArea => GetComponent<UI_SecretsArea>();
    private UI_ScreenTransistion ScreenTransistion => GetComponent<UI_ScreenTransistion>();
    private UI_VampirePowers VampirePowers => GetComponent<UI_VampirePowers>();
    private UI_CharacterInfoArea CharacterInfo => GetComponent<UI_CharacterInfoArea>();

    protected override void Start()
    {
        base.Start();
        Deactivate();

        _exitButton.onClick.AddListener(OnExitButtonClicked);
        _backButton.onClick.AddListener(OnBackButtonClicked);

        ScreenTransistion.Initialize(_mainScreen);
        VampirePowers.Initialize(OnProbeMindClicked, OnTranceClicked);
    }    

    public void Activate(NPCHumanCharacterID characterID)
    {
        _characterId = characterID;

        NpcBehaviorBB.Instance.EnterConversationWithPlayer(_characterId);
        MouseReceiver.Instance.Deactivate();

        _characterInteractionContent.SetActive(true);

        _mainScreen.gameObject.SetActive(true);
        _exitButton.gameObject.SetActive(true);

        _miniGameScreen.gameObject.SetActive(false);
        _backButton.gameObject.SetActive(false);

        //_detectivePowerBar.Initialize(characterID);

        SecretsArea.InitializeForNewCharacter(characterID);
        VampirePowers.InitializeForNewCharacter(characterID);
        CharacterInfo.InitializeForNewCharacter(characterID);
    }

    public void Deactivate()
    {
        if (_characterId != null)
            NpcBehaviorBB.Instance.EndConversationWithPlayer(_characterId);

        //if (_screenState == ScreenState.RevealingSecrets)
        //{
        //    OnRevealScreenFinish(null, false);
        //    PlayerStats.Instance.TrySetPendingVampirePoints(0);
        //}

        //_screenState = ScreenState.Off;
        UI_BottomBarController.Instance.Default();

        MouseReceiver.Instance.Activate();
        _characterInteractionContent.SetActive(false);

        SecretsArea.Deactivate();
        VampirePowers.Deactivate();
        CharacterInfo.Deactivate();

        //_detectivePowerBar.gameObject.SetActive(false);
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

    private void OnForgetClicked()
    {

    }

    private void OnTranceClicked()
    {
        //SetActiveForAllChildren(_murderSectionAdder, false);

        //_murderSectionAdder.gameObject.SetActive(true);
        //_trance.gameObject.SetActive(false);
        //_probeMind.gameObject.SetActive(false);
    }

    private void OnProbeMindClicked()
    {
        if (_characterId == null)
            return;

        var characterInfo = CharacterInfoBB.Instance.GetCharacterInfo(_characterId);
        if (characterInfo.MindProbed)
            return;

        _exitButton.interactable = false;

        ScreenTransistion.Transition(UI_ScreenTransistion.TransitionType.FromBelaImage,
            () => characterInfo.MindProbed = true,
            () => _exitButton.interactable = true);
    }
    private void OnExitButtonClicked() => Deactivate();

    private void OnBackButtonClicked() { }
}