using System;
using UnityEngine;
using UnityEngine.UI;

public class UI_SelectableSecretTile : MonoBehaviour
{
    [SerializeField] private RawImage _secretIcon;

    private Secret _secret;
    private Action<Secret> _onSecretSelected;
    private Action<Secret> _onSecretRevealed;
    private bool _isSecretRevealed;

    private void Update()
    {
        if (_secret != null && _isSecretRevealed != _secret.IsRevealed)
        {
            _isSecretRevealed = _secret.IsRevealed;
            SetSecretTexture();
            _onSecretRevealed(_secret);
        }
    }

    public void Initialize(Secret secret, Action<Secret> onSecretSelected, Action<Secret> onSecretRevealed)
    {
        _secret = secret;
        _onSecretSelected = onSecretSelected;
        _onSecretRevealed = onSecretRevealed;

        _isSecretRevealed = secret.IsRevealed;
        SetSecretTexture();
    }    

    public void Select()
    {
        _onSecretSelected(_secret);
    }

    public void SelectInitial()
    {
        GetComponent<Button>().Select();
        Select();
    }

    private void SetSecretTexture()
    {
        _secretIcon.texture = _secret.IconTexture;
    }
}