//using UnityEngine;

//public class UI_CharacterInteractionMenuController : UI_MenuController, IUnlockableHelper
//{
//    [SerializeField] private UI_UnlockableZone _secretUnlockableZone;

//    private CharacterSecretKnowledge _characterSecrets;

//    public void SetCharacterSecrets(CharacterSecretKnowledge characterSecrets)
//    {
//        _characterSecrets = characterSecrets;
//    }

//    public override void Activate()
//    {
//        base.Activate();

//        MouseReceiver.Instance.Deactivate();

//        if (_characterSecrets.Secrets.IsAnySecretsRevealed)
//            _secretUnlockableZone.ForceUnlock();
//        else
//            _secretUnlockableZone.Lock();

//        _characterInteractionScreen.gameObject.SetActive(true);
//        _interactingCharacterInfo.Initialize(GetCharacterInfo());

//        _secrets.ResetSecrets();
//        _secrets.AddSecrets(_characterSecrets.Secrets);
//        _secrets.AddRumours(_characterSecrets.Rumours);
//    }

//    public CharacterInfo GetCharacterInfo()
//    {
//        return _characterSecrets.GetComponent<CharacterInfo>();
//    }

//    public bool TryUnlock()
//    {
//        var characterInfo = GetCharacterInfo();
//        return characterInfo.UseDetectivePoint();
//    }

//    public void UnlockSecret()
//    {
//        if (!GetCharacterInfo().UseDetectivePoint())
//            return;

//        _characterSecrets.RevealSecret();
//    }
//}
