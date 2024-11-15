using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UI_MenuManager : GlobalSingleInstanceMonoBehaviour<UI_MenuManager>
{
    [SerializeField] private RectTransform _bottomBar;
    [SerializeField] private RectTransform _characterInteractionScreen;
    [SerializeField] private UI_Secrets _secrets;

    private List<UI_MenuController> _menuControllers;
    private UI_MenuController _activeMenuController;

    private List<UI_MenuController> MenuControllers => _menuControllers ??= GetComponents<UI_MenuController>().ToList();
    private UI_DefaultMenuController DefaultMenuController => MenuControllers.OfType<UI_DefaultMenuController>().First();

    protected override void Start()
    {
        base.Start();
        
        foreach (var menuController in MenuControllers)
            menuController.Initialize(_bottomBar, _characterInteractionScreen, _secrets);

        _activeMenuController = DefaultMenuController;
        _activeMenuController.Activate();
    }

    public void DeactivateCurrentMenuController()
    {
        if (_activeMenuController == DefaultMenuController)
            return;

        _activeMenuController = DefaultMenuController;
        _activeMenuController.Activate();
    }

    public void TalkToNPC(CharacterSecrets characterSecrets)
    {
        var characterInteractionController = MenuControllers.OfType<UI_CharacterInteractionMenuController>().First();
        characterInteractionController.SetCharacterSecrets(characterSecrets);

        _activeMenuController = characterInteractionController;
        _activeMenuController.Activate();
    }
}
