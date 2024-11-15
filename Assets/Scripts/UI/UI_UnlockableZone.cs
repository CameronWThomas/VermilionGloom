using System;
using UnityEngine;

public interface IUnlockableHelper
{
    CharacterInfo GetCharacterInfo();
    bool TryUnlock();
}

public class UI_UnlockableZone : MonoBehaviour
{
    private enum LockType
    {
        CharacterSecrets
    }

    [SerializeField] private LockType _lockType = LockType.CharacterSecrets;

    public void Lock()
    {
        gameObject.SetActive(true);
    }

    public void Unlock()
    {
        var target = transform.parent;
        while (target != null)
        {
            if (target.TryGetComponent<IUnlockableHelper>(out var lockedZoneHelper))
            {
                if (lockedZoneHelper.TryUnlock())
                    Unlock(lockedZoneHelper);
                return;
            }

            target = target.parent;
        }
    }    

    public void ForceUnlock()
    {
        gameObject.SetActive(false);
    }

    private void Unlock(IUnlockableHelper lockedZoneHelper)
    {
        var characterInfo = lockedZoneHelper.GetCharacterInfo();

        if (_lockType is LockType.CharacterSecrets)
            characterInfo.GetComponent<SecretKnowledge>().UnlockInitialSecrets();

        ForceUnlock();
    }
}