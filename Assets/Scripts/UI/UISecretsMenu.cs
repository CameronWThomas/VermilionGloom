using System;
using UnityEngine;

public class UISecretsMenu : MonoBehaviour
{
    public void Initialize(SecretCollection personalSecrets)
    {
        var secretList = GetComponentInChildren<UISecretList>(true);
        secretList.Initialize(personalSecrets);
    }
}