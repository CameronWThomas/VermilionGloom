using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UI_CharacterInteractionMenu : GlobalSingleInstanceMonoBehaviour<UI_CharacterInteractionMenu>
{
    [SerializeField] private UI_PowerBar _detectivePowerBar;
    [SerializeField] private GameObject _selectableSecretTilePrefab;

    [SerializeField] private RectTransform _secretsGrid;

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

        _detectivePowerBar.Initialize(10);

        AddSecrets();
    }    

    public void Deactivate()
    {
        MouseReceiver.Instance.Activate();
        gameObject.SetActive(false);
        _detectivePowerBar.gameObject.SetActive(false);

        _secretsTileList.ForEach(x => Destroy(x.gameObject));
        _secretsTileList.Clear();
    }

    private void AddSecrets()
    {
        var secrets = CharacterSecretKnowledgeBB.Instance.GetSecrets(_characterId);

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

    }
}