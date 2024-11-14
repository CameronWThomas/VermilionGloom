using NUnit.Framework;
using System;
using System.Linq;
using UnityEngine;

public class UI_CharacterSecretsMenu : MonoBehaviour
{
    [SerializeField] private GameObject _secretsMenu;

    private UI_SecretCollections _secretCollections;

    private void Start()
    {
        _secretCollections = _secretsMenu.GetComponentInChildren<UI_SecretCollections>();
    }

    public void Open(NpcBrain brain)
    {
        _secretsMenu.SetActive(true);

        var secretCollections = brain.OthersSecretCollections.ToList();
        secretCollections.Add(brain.PersonalSecrets);

        _secretCollections.Initialize(secretCollections);
    }

    internal void Close()
    {
        _secretsMenu.SetActive(false);
    }
}
