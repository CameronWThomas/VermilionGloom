using System;
using UnityEngine;

public abstract class UI_MenuController : MonoBehaviour
{
    protected RectTransform _bottomBar;
    protected RectTransform _characterInteractionScreen;
    protected UI_Secrets _secrets;

    public abstract void Activate();

    public void Initialize(RectTransform bottomBar,
        RectTransform characterInteractionScreen,
        UI_Secrets secrets)
    {
        _bottomBar = bottomBar;
        _characterInteractionScreen = characterInteractionScreen;
        _secrets = secrets;
    }
}
