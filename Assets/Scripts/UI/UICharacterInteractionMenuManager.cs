using System;
using UnityEngine;

public class UICharacterInteractionMenuManager : MonoBehaviour
{
    [SerializeField] private UISecretsMenu _secretsMenu;
    [SerializeField] private UIRumorsMenu _rumorsMenu;

    private enum MenuMode { Secrets, Rumors }
    private MenuMode _menuMode;
    
    public void Initialize(NpcBrain brain)
    {
        _secretsMenu.Initialize(brain.PersonalSecrets);
        _rumorsMenu.Initialize(brain.OthersSecretCollections);

        UpdateMenuMode(MenuMode.Secrets);
    }

    private void UpdateMenuMode(MenuMode menuMode)
    {
        switch (menuMode)
        {
            case MenuMode.Secrets:
                _secretsMenu.gameObject.SetActive(true);
                _rumorsMenu.gameObject.SetActive(false);
                break;

            case MenuMode.Rumors:
                _secretsMenu.gameObject.SetActive(false);
                _rumorsMenu.gameObject.SetActive(true);
                break;
        }
    }
}