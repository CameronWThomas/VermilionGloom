using System;
using UnityEngine;
using UnityEngine.UI;

public class UI_SelectableSecretTile : MonoBehaviour
{
    [SerializeField] private RawImage _secretIcon;

    private Secret _secret;
    private Action<Secret> _onSecretSelected;
    private Func<Secret, bool> _isSelected;

    private ColorBlock _normalColors;
    private ColorBlock _selectedColors;
    private Button _button;

    private void Update()
    {
        if (_isSelected(_secret))
            _button.colors = _selectedColors;
        else
            _button.colors = _normalColors;
    }

    public void Initialize(Secret secret, Action<Secret> onSecretSelected, Func<Secret, bool> isSelected)
    {
        _secret = secret;
        _onSecretSelected = onSecretSelected;
        _isSelected = isSelected;

        _button = GetComponent<Button>();
        _normalColors = _button.colors;

        _selectedColors = _button.colors;
        var selectedColor = _button.colors.selectedColor;
        _selectedColors.normalColor = selectedColor;
        _selectedColors.highlightedColor = selectedColor;
        _selectedColors.pressedColor = selectedColor;
        
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