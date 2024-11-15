using UnityEngine;

public class UI_DefaultMenuController : UI_MenuController
{
    public override void Activate()
    {
        MouseReceiver.Instance.Activate();

        _characterInteractionScreen.gameObject.SetActive(false);
    }
}
