//using NUnit.Framework;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using UnityEngine;

//public class UI_MenuManager : GlobalSingleInstanceMonoBehaviour<UI_MenuManager>
//{
//    [SerializeField] private RectTransform _bottomBar;
//    [SerializeField] private RectTransform _characterInteractionScreen;
//    [SerializeField] private UI_Secrets _secrets;
//    [SerializeField] private UI_CharacterInfo _interactingCharacterInfo;

//    private List<UI_MenuController> _menuControllers;
//    private UI_MenuController _activeMenuController;

//    private List<UI_MenuController> MenuControllers => _menuControllers ??= GetComponents<UI_MenuController>().ToList();
//    private UI_DefaultMenuController DefaultMenuController => MenuControllers.OfType<UI_DefaultMenuController>().First();

//    protected override void Start()
//    {
//        base.Start();

//        foreach (var menuController in MenuControllers)
//        {
//            menuController.Initialize(_bottomBar,
//                _characterInteractionScreen,
//                _secrets,
//                _interactingCharacterInfo);
//            menuController.Deactivate();
//        }

//        ActivateNewMenuController(DefaultMenuController);
        
//    }

//    public void DeactivateCurrentMenuController()
//    {
//        if (_activeMenuController == DefaultMenuController)
//            return;

//        ActivateNewMenuController(DefaultMenuController);
//    }

//    public void TalkToNPC(CharacterSecretKnowledge characterSecrets)
//    {
//        var characterInteractionController = MenuControllers.OfType<UI_CharacterInteractionMenuController>().First();
//        characterInteractionController.SetCharacterSecrets(characterSecrets);

//        ActivateNewMenuController(characterInteractionController);
//    }

//    private void ActivateNewMenuController(UI_MenuController newMenuController)
//    {
//        _activeMenuController?.Deactivate();
//        _activeMenuController = newMenuController;
//        _activeMenuController.Activate();
//    }
//}
