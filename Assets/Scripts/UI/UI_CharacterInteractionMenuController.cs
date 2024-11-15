using UnityEngine;

public class UI_CharacterInteractionMenuController : UI_MenuController
{
    private CharacterSecrets _characterSecrets;

    public void SetCharacterSecrets(CharacterSecrets characterSecrets)
    {
        _characterSecrets = characterSecrets;
    }

    public override void Activate()
    {
        MouseReceiver.Instance.Deactivate();

        _characterInteractionScreen.gameObject.SetActive(true);

        _secrets.ResetSecrets();
        _secrets.AddSecrets(_characterSecrets.PersonalSecrets);
        foreach (var secrets in _characterSecrets.Rumours)
        {
            _secrets.AddSecrets(secrets);
        }
    }
}
