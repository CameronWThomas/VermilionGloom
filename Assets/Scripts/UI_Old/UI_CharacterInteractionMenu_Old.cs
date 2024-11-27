using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

public class UI_CharacterInteractionMenu_Old : GlobalSingleInstanceMonoBehaviour<UI_CharacterInteractionMenu_Old>
{
    [SerializeField] private GameObject _selectableSecretTilePrefab;
    [SerializeField] private UI_PowerBar _detectivePowerBar;
    [SerializeField] private RectTransform _secretsGrid;
    [SerializeField] private RawImage _selectedSecretImage;
    [SerializeField] private TMP_Text _selectedSecretText;
    [SerializeField] private GameObject _singlePartySelectedSecret;
    [SerializeField] private GameObject _multiPartySelectedSecret;
    [SerializeField] private TMP_Text _characterName;

    [SerializeField] private UI_Portrait _selectedCharacterPortrait;
    [SerializeField] private UI_Portrait _singlePartyPortrait;
    [SerializeField] private UI_Portrait _multiPartyPortrait1;
    [SerializeField] private UI_Portrait _multiPartyPortrait2;

    [SerializeField] private List<GameObject> _hideObjectsDuringAction = new();
    [SerializeField] private UI_SecretRevealScreen _secretRevealScreen;

    private enum ScreenState { Off, Normal, RevealingSecrets }
    private ScreenState _screenState = ScreenState.Off;

    private NPCHumanCharacterID _characterId;

    private Secret _selectedSecret = null;

    private List<UI_SelectableSecretTile> _secretsTileList = new();

    public void Activate(NPCHumanCharacterID characterID)
    {
        _characterId = characterID;

        NpcBehaviorBB.Instance.EnterConversationWithPlayer(_characterId);

        _screenState = ScreenState.Normal;

        MouseReceiver.Instance.Deactivate();

        gameObject.SetActive(true);
        _detectivePowerBar.gameObject.SetActive(true);
        _hideObjectsDuringAction.ForEach(x => x.SetActive(true));
        _secretRevealScreen.gameObject.SetActive(false);

        var secrets = CharacterSecretKnowledgeBB.Instance.GetSecrets(characterID);
        _characterName.text = characterID.Name;
        //_selectedCharacterPortrait.SetContent(characterID.PortraitContent);
        _selectedCharacterPortrait.SetContent(characterID.PortraitColor);


        _detectivePowerBar.Initialize(characterID);

        AddSecrets(secrets);
    }    

    public void Deactivate()
    {
        NpcBehaviorBB.Instance.EndConversationWithPlayer(_characterId);

        if (_screenState == ScreenState.RevealingSecrets)
        {
            OnRevealScreenFinish(null, false);
            PlayerStats.Instance.TrySetPendingVampirePoints(0);
        }

        _screenState = ScreenState.Off;

        MouseReceiver.Instance.Activate();
        gameObject.SetActive(false);
        _detectivePowerBar.gameObject.SetActive(false);

        _secretsTileList.ForEach(x => Destroy(x.gameObject));
        _secretsTileList.Clear();
    }

    public void OnRevealSecretsPressed()
    {
        if (_screenState != ScreenState.Normal)
            return;

        if (!_characterId.CharacterInfo.TrySetPendingDetectivePoints(1))
            return;

        _screenState = ScreenState.RevealingSecrets;

        _hideObjectsDuringAction.ForEach(x => x.SetActive(false));
        _secretRevealScreen.gameObject.SetActive(true);
        _secretRevealScreen.Initialize(_characterId, OnRevealScreenFinish);
        
    }    

    public void OnActionComplete()
    {
        _screenState = ScreenState.Normal;

        _hideObjectsDuringAction.ForEach(x => x.SetActive(true));
        _secretRevealScreen.gameObject.SetActive(false);
    }

    private void AddSecrets(IEnumerable<Secret> secrets)
    {
        //foreach (var secret in secrets.OrderBy(x => !x.IsRevealed))
        //{
        //    var selectableTile = Instantiate(_selectableSecretTilePrefab, _secretsGrid).GetComponent<UI_SelectableSecretTile>();
        //    selectableTile.Initialize(secret, OnSecretSelected, OnSecretRevealed);
        //    _secretsTileList.Add(selectableTile);
        //}

        _secretsTileList.First().SelectInitial();
    }    

    private void OnSecretSelected(Secret secret)
    {
        _selectedSecret = secret;

        _selectedSecretImage.texture = secret.IconTexture;
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

    private void OnSecretRevealed(Secret secret)
    {
        if (_selectedSecret != secret)
            return;

        // Reselect to we can update the active secret screen
        OnSecretSelected(secret);
    }

    private void OnRevealScreenFinish(SecretLevel? level, bool gamePlayed)
    {
        if (gamePlayed)
        {
            _characterId.CharacterInfo.TryUseDetectivePoint(1);

            if (level.HasValue)
                CharacterSecretKnowledgeBB.Instance.UnlockSecret(_characterId, level.Value);
        }
        else
            _characterId.CharacterInfo.TrySetPendingDetectivePoints(0);

        OnActionComplete();
    }
}