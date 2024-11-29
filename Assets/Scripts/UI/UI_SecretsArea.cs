using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

public class UI_SecretsArea : UI_SectionBase
{
    [SerializeField] RectTransform _secretsSection;

    [Header("Secret Grid")]
    [SerializeField] GameObject _selectableSecretTilePrefab;
    [SerializeField] GridLayoutGroup _secretsGrid;

    [Header("Selected Secret")]
    [SerializeField] TMP_Text _selectedSecretText;
    [SerializeField] GameObject _singlePartySelectedSecret;
    [SerializeField] GameObject _multiPartySelectedSecret;
    //[SerializeField] TMP_Text _characterName;

    //[SerializeField] UI_Portrait _selectedCharacterPortrait;
    [SerializeField] UI_Portrait _singlePartyPortrait;
    [SerializeField] UI_Portrait _multiPartyPortrait1;
    [SerializeField] UI_Portrait _multiPartyPortrait2;

    private List<UI_SelectableSecretTile> _secretsTileList = new();

    public Secret SelectedSecret { get; private set; }


    public override void InitializeForNewCharacter(NPCHumanCharacterID characterId, Func<CharacterInteractingState> getState)
    {
        base.InitializeForNewCharacter(characterId, getState);
        
        ClearSecretTiles();
        var secrets = CharacterSecretKnowledgeBB.Instance.GetSecrets(characterId);
        AddSecrets(secrets);
    }

    public override void Deactivate()
    {
        base.Deactivate();
        ClearSecretTiles();
    }

    protected override void UpdateHidden(bool hide)
    {
        _secretsSection.gameObject.SetActive(!hide);
    }

    protected override void OnStateChanged(CharacterInteractingState state)
    {
        if (state is CharacterInteractingState.Default)
        {
            HandleNewSecrets();
            Unhide();
        }
        else
            Hide();
    }

    private void HandleNewSecrets()
    {
        var secrets = CharacterSecretKnowledgeBB.Instance.GetSecrets(_characterID);
        if (_secretsTileList.Count == secrets.Count)
            return;

        ClearSecretTiles();
        AddSecrets(secrets);
    }

    private void ClearSecretTiles()
    {
        _secretsTileList.ForEach(x => Destroy(x.gameObject));
        _secretsTileList.Clear();
    }

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
        SelectedSecret = secret;

        //_selectedSecretImage.texture = secret.IconTexture;
        //if (!secret.IsRevealed)
        //{
        //    _multiPartySelectedSecret.SetActive(false);
        //    _singlePartySelectedSecret.SetActive(false);

        //    _selectedSecretText.text = "???";
        //    return;
        //}

        //if (secret.IsASpreadSecret)
        //{ }

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

                _multiPartyPortrait1.SetCharacter(secret.SecretTarget);
                _multiPartyPortrait2.SetCharacter(secret.AdditionalCharacter);
            }
            else
            {
                _multiPartySelectedSecret.SetActive(false);
                _singlePartySelectedSecret.SetActive(true);

                _singlePartyPortrait.SetCharacter(secret.SecretTarget);
            }
        }

        _selectedSecretText.text = secret.Description;
    }

    private bool IsSecretSelected(Secret secret)
    {
        return SelectedSecret == secret;
    }
}