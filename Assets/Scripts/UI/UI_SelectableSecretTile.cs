using System;
using UnityEngine;
using UnityEngine.UI;

public class UI_SelectableSecretTile : MonoBehaviour
{
    [SerializeField] private RawImage _secretIcon;

    private Secret _secret;
    private Action<Secret> _onSecretSelected;

    public void Initialize(Secret secret, Action<Secret> onSecretSelected)
    {
        _secret = secret;
        _onSecretSelected = onSecretSelected;
        _secretIcon.texture = secret.IconTexture;
    }
    
    public void Select()
    {
        _onSecretSelected(_secret);
    }

    public void SelectInitial() => GetComponent<Button>().Select();
}