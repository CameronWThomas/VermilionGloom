using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_CharacterInteractionMenu : GlobalSingleInstanceMonoBehaviour<UI_CharacterInteractionMenu>
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

    private CharacterID _characterId;

    private Secret _selectedSecret = null;

    private List<UI_SelectableSecretTile> _secretsTileList = new();

    private void Awake()
    {
        Deactivate();
    }

    public void Activate(CharacterID characterID)
    {
        _characterId = characterID;

        MouseReceiver.Instance.Deactivate();
        gameObject.SetActive(true);
        _detectivePowerBar.gameObject.SetActive(true);

        var secrets = CharacterSecretKnowledgeBB.Instance.GetSecrets(characterID);
        _characterName.text = characterID.Name;
        //_selectedCharacterPortrait.SetContent(characterID.PortraitContent);
        _selectedCharacterPortrait.SetContent(characterID.PortraitColor);


        _detectivePowerBar.Initialize(10);

        AddSecrets(secrets);
    }    

    public void Deactivate()
    {
        MouseReceiver.Instance.Activate();
        gameObject.SetActive(false);
        _detectivePowerBar.gameObject.SetActive(false);

        _secretsTileList.ForEach(x => Destroy(x.gameObject));
        _secretsTileList.Clear();
    }

    private void AddSecrets(IEnumerable<Secret> secrets)
    {
        foreach (var secret in secrets)
        {
            var selectableTile = Instantiate(_selectableSecretTilePrefab, _secretsGrid).GetComponent<UI_SelectableSecretTile>();
            selectableTile.Initialize(secret, OnSecretSelected, OnSecretRevealed);
            _secretsTileList.Add(selectableTile);
        }

        _secretsTileList.First().SelectInitial();
    }    

    private void OnSecretSelected(Secret secret)
    {
        _selectedSecret = secret;

        _selectedSecretImage.texture = secret.IconTexture;
        if (!secret.IsRevealed)
        {
            _multiPartySelectedSecret.SetActive(false);
            _singlePartySelectedSecret.SetActive(false);

            _selectedSecretText.text = "???";
            return;
        }

        if (secret.HasAdditionalCharacter)
        {
            _multiPartySelectedSecret.SetActive(true);
            _singlePartySelectedSecret.SetActive(false);

            //_multiPartyPortrait1.SetContent(secret.SecretOwner.PortraitContent);
            _multiPartyPortrait1.SetContent(secret.SecretOwner.PortraitColor);

            //_multiPartyPortrait2.SetContent(secret.AdditionalCharacter.PortraitContent);
            _multiPartyPortrait2.SetContent(secret.AdditionalCharacter.PortraitColor);
        }
        else
        {
            _multiPartySelectedSecret.SetActive(false);
            _singlePartySelectedSecret.SetActive(true);

            //_singlePartyPortrait.SetContent(secret.SecretOwner.PortraitContent);
            _singlePartyPortrait.SetContent(secret.SecretOwner.PortraitColor);
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
}