using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

public class UI_CharacterInteractionMenu : GlobalSingleInstanceMonoBehaviour<UI_CharacterInteractionMenu>
{
    [SerializeField] GameObject _characterInteractionContent;
    [SerializeField] Button _exitButton;
    [SerializeField] Button _backButton;

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
        foreach (var secret in secrets.OrderBy(x => !x.IsRevealed))
        {
            var selectableTile = Instantiate(_selectableSecretTilePrefab, _secretsGrid.transform).GetComponent<UI_SelectableSecretTile>();
            selectableTile.Initialize(secret, OnSecretSelected, OnSecretRevealed);
            _secretsTileList.Add(selectableTile);
        }

        _secretsTileList.First().SelectInitial();
    }

    private void OnSecretSelected(Secret secret)
    {
        _selectedSecret = secret;

        //_selectedSecretImage.texture = secret.IconTexture;
        if (!secret.IsRevealed)
        {
            _multiPartySelectedSecret.SetActive(false);
            _singlePartySelectedSecret.SetActive(false);

            _selectedSecretText.text = "???";
            return;
        }

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

    private void OnSecretRevealed(Secret secret)
    {
        if (_selectedSecret != secret)
            return;

        // Reselect to we can update the active secret screen
        OnSecretSelected(secret);
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
}