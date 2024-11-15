using NUnit.Framework;
using System;
using System.Linq;
using UnityEngine;

public class UI_CharacterSecretsMenu : MonoBehaviour
{
    private UISecretCollections SecretCollections => GetComponentInChildren<UISecretCollections>(true);

    public void Open(NpcBrain brain)
    {
        gameObject.SetActive(true);

        var secretCollections = brain.OthersSecretCollections.ToList();
        secretCollections.Add(brain.PersonalSecrets);

        SecretCollections.Initialize(secretCollections);
    }

    internal void Close()
    {
        gameObject.SetActive(false);
    }
}
