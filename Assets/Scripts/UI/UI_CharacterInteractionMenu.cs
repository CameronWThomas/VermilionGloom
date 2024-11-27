using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UI_CharacterInteractionMenu : GlobalSingleInstanceMonoBehaviour<UI_CharacterInteractionMenu>
{
    [SerializeField] GameObject _characterInteractionContent;
    [SerializeField] Button _exitButton;
    [SerializeField] Button _backButton;

    [Header("Vampire power stuff")]
    [SerializeField] RectTransform _vampPowerSection;
    [SerializeField] Button _probeMind;
    [SerializeField] Button _trance;
    [SerializeField] Button _forget;

    [Header("Spin transition stuff")]
    [SerializeField] RectTransform _belaImage;
    [SerializeField] GameObject _belaPrefab;
    [SerializeField, Range(0f, 20f)] float _batTransitionSpinSpeed = 15f;
    [SerializeField, Range(0f, 10f)] float _batTransitionDuration = 2f;
    [SerializeField, Range(0f, 10f)] float _batTransitionScale = 5f;

    [Header("Other")]
    [SerializeField] RectTransform _mainScreen;
    [SerializeField] RectTransform _miniGameScreen;

    [SerializeField] GameObject _selectableSecretTilePrefab;
    [SerializeField] GridLayoutGroup _secretsGrid;

    [SerializeField] TMP_Text _selectedSecretText;
    [SerializeField] GameObject _singlePartySelectedSecret;
    [SerializeField] GameObject _multiPartySelectedSecret;
    //[SerializeField] TMP_Text _characterName;

    //[SerializeField] UI_Portrait _selectedCharacterPortrait;
    [SerializeField] UI_Portrait _singlePartyPortrait;
    [SerializeField] UI_Portrait _multiPartyPortrait1;
    [SerializeField] UI_Portrait _multiPartyPortrait2;

    //[SerializeField] private List<GameObject> _hideObjectsDuringAction = new();

    private NPCHumanCharacterID _characterId;

    private Secret _selectedSecret = null;

    private List<UI_SelectableSecretTile> _secretsTileList = new();

    protected override void Start()
    {
        base.Start();
        Deactivate();

        _probeMind.onClick.AddListener(OnProbeMindClick);
        _trance.onClick.AddListener(OnTranceClicked);
        _forget.onClick.AddListener(OnForgetClicked);
        _exitButton.onClick.AddListener(OnExitButtonClicked);
        _backButton.onClick.AddListener(OnBackButtonClicked);
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

        var secrets = CharacterSecretKnowledgeBB.Instance.GetSecrets(characterID);

        UI_BottomBarController.Instance.SetInteractingCharacter(characterID);

        //_detectivePowerBar.Initialize(characterID);

        AddSecrets(secrets);

        var characterInfo = CharacterInfoBB.Instance.GetCharacterInfo(characterID);
        SetupBaseCharacterInteractionScreen(characterInfo.MindProbed);
    }

    private void SetupBaseCharacterInteractionScreen(bool mindProbed)
    {
        for (var i = 0; i < _vampPowerSection.parent.childCount; i++)
            _vampPowerSection.parent.GetChild(i).gameObject.SetActive(mindProbed);

        _vampPowerSection.gameObject.SetActive(true);
        _trance.gameObject.SetActive(mindProbed);
        _probeMind.gameObject.SetActive(!mindProbed);
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
        //_detectivePowerBar.gameObject.SetActive(false);

        _secretsTileList.ForEach(x => Destroy(x.gameObject));
        _secretsTileList.Clear();
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

    private void AddSecrets(IEnumerable<Secret> secrets)
    {
        foreach (var secret in secrets)
        {
            var selectableTile = Instantiate(_selectableSecretTilePrefab, _secretsGrid.transform).GetComponent<UI_SelectableSecretTile>();
            selectableTile.Initialize(secret, OnSecretSelected, IsSecretSelected);
            _secretsTileList.Add(selectableTile);
        }

        _secretsTileList.First().SelectInitial();
    }

    private void OnSecretSelected(Secret secret)
    {
        _selectedSecret = secret;

        //_selectedSecretImage.texture = secret.IconTexture;
        //if (!secret.IsRevealed)
        //{
        //    _multiPartySelectedSecret.SetActive(false);
        //    _singlePartySelectedSecret.SetActive(false);

        //    _selectedSecretText.text = "???";
        //    return;
        //}

        //TODO need to do add something for this case
        if (secret.IsASpreadSecret)
        { }

        if (secret.NoCharactersInvolved)
        {
            _multiPartySelectedSecret.SetActive(false);
            _singlePartySelectedSecret.SetActive(false);
        }
        else
        {
            if (secret.HasAdditionalCharacter && secret.HasSecretTarget)
            {
                _multiPartySelectedSecret.SetActive(true);
                _singlePartySelectedSecret.SetActive(false);

                //_multiPartyPortrait1.SetContent(secret.SecretOwner.PortraitContent);
                _multiPartyPortrait1.SetContent(secret.SecretTarget.PortraitColor);

                //_multiPartyPortrait2.SetContent(secret.AdditionalCharacter.PortraitContent);
                _multiPartyPortrait2.SetContent(secret.AdditionalCharacter.PortraitColor);
            }
            else
            {
                _multiPartySelectedSecret.SetActive(false);
                _singlePartySelectedSecret.SetActive(true);

                //_singlePartyPortrait.SetContent(secret.SecretOwner.PortraitContent);
                _singlePartyPortrait.SetContent(secret.SecretTarget.PortraitColor);
            }
        }

        _selectedSecretText.text = secret.Description;
    }

    private bool IsSecretSelected(Secret secret)
    {
        return _selectedSecret == secret;
    }

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

    }

    private void OnProbeMindClick()
    {
        if (_characterId == null)
            return;

        var characterInfo = CharacterInfoBB.Instance.GetCharacterInfo(_characterId);
        if (characterInfo.MindProbed)
            return;

        characterInfo.MindProbed = true;
        StartCoroutine(ProbeMindRoutine());
    }

    private void OnExitButtonClicked() => Deactivate();

    private void OnBackButtonClicked() { }

    private IEnumerator ProbeMindRoutine()
    {
        _exitButton.interactable = false;

        var newBela = Instantiate(_belaPrefab, _mainScreen);
        var newBellaRectTransform = newBela.GetComponent<RectTransform>();

        newBellaRectTransform.anchoredPosition = _belaImage.anchoredPosition + new Vector2(18f, -18f); // Add padding of the horizontal groups
        newBellaRectTransform.sizeDelta = _belaImage.sizeDelta;

        _belaImage.gameObject.SetActive(false);


        var originalSizeDelta = newBellaRectTransform.sizeDelta;
        var maxSizeDelta = newBellaRectTransform.sizeDelta * _batTransitionScale;
        var startSizeDelta = originalSizeDelta;
        var finalSizeDelta = maxSizeDelta;

        StartCoroutine(SpinRectTransform(newBellaRectTransform, _batTransitionSpinSpeed, _batTransitionDuration));

        var duration = _batTransitionDuration;
        var originalStartTime = Time.time;
        var startTime = Time.time;
        while (Time.time - originalStartTime <= duration)
        {
            var t = (Time.time - startTime) / (duration / 2f);

            if (t > 1f)
            {
                SetupBaseCharacterInteractionScreen(true);
                _belaImage.gameObject.SetActive(true);

                t = 0f;
                startTime = Time.time;
                finalSizeDelta = Vector2.zero;
                startSizeDelta = newBellaRectTransform.sizeDelta;
            }

            newBellaRectTransform.sizeDelta = Vector2.Lerp(startSizeDelta, finalSizeDelta, t);

            yield return new WaitForNextFrameUnit();
        }        

        Destroy(newBela);
        _exitButton.interactable = true;
    }

    private static IEnumerator SpinRectTransform(RectTransform rectTransform, float maxSpeed, float duration)
    {
        var startTime = Time.time;
        while (Time.time - startTime <= duration)
        {
            if (rectTransform.gameObject.IsDestroyed())
                yield break;

            var t = .2f + (Time.time - startTime) / (duration * .5f );
            var speed = Mathf.Lerp(0f, maxSpeed, Mathf.Clamp(t, 0f, 1f));

            var lastRotation = rectTransform.rotation;
            rectTransform.rotation = Quaternion.Euler(lastRotation.eulerAngles + speed * Vector3.forward);

            yield return new WaitForNextFrameUnit();
        }
    }
}