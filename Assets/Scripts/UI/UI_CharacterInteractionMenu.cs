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

    private CharacterID _characterId;

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
            selectableTile.Initialize(secret, OnSecretSelected);
            _secretsTileList.Add(selectableTile);
        }

        _secretsTileList.First().SelectInitial();
    }

    private void OnSecretSelected(Secret secret)
    {
        _selectedSecretImage.texture = secret.IconTexture;
        if (secret.HasAdditionalCharacter)
        {
            _multiPartySelectedSecret.SetActive(true);
            _singlePartySelectedSecret.SetActive(false);
        }
        else
        {
            _multiPartySelectedSecret.SetActive(false);
            _singlePartySelectedSecret.SetActive(true);
        }

        _selectedSecretText.text = secret.Description;
    }
}