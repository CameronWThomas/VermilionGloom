//using System;
//using UnityEngine;

//public abstract class UI_MenuController : MonoBehaviour
//{
//    protected RectTransform _bottomBar;
//    protected RectTransform _characterInteractionScreen;
//    protected UI_Secrets _secrets;
//    protected UI_CharacterInfo _interactingCharacterInfo;

//    public bool IsActivated { get; set; } = false;

//    public virtual void Activate()
//    {
//        IsActivated = true;
//    }

//    public void Deactivate()
//    {
//        IsActivated = false;
//    }

//    private void Update()
//    {
//        if (IsActivated)
//            InternalUpdate();
//    }

//    protected virtual void InternalUpdate()
//    { }

//    public void Initialize(RectTransform bottomBar,
//        RectTransform characterInteractionScreen,
//        UI_Secrets secrets,
//        UI_CharacterInfo interactingCharacterInfo)
//    {
//        _bottomBar = bottomBar;
//        _characterInteractionScreen = characterInteractionScreen;
//        _secrets = secrets;
//        _interactingCharacterInfo = interactingCharacterInfo;
//    }
//}
