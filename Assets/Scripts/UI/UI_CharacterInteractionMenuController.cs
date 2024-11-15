using UnityEngine;

public class UI_CharacterInteractionMenuController : UI_MenuController
{
    private SecretKnowledge _characterSecrets;

    public void SetCharacterSecrets(SecretKnowledge characterSecrets)
    {
        _characterSecrets = characterSecrets;
    }

    public override void Activate()
    {
        MouseReceiver.Instance.Deactivate();

        _characterInteractionScreen.gameObject.SetActive(true);

        _secrets.ResetSecrets();
        _secrets.AddSecrets(_characterSecrets.Secrets);
        foreach (var rumour in _characterSecrets.Rumours)
        {
            _secrets.AddRumour(rumour);
        }
    }
}
