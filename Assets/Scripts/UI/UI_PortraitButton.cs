using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_PortraitButton : MonoBehaviour
{
    [SerializeField] UI_Portrait _portrait;
    [SerializeField] TMP_Text _characterNameText;

    private CharacterID _characterID;

    private Action<CharacterID> _onSelected;
    private Func<CharacterID, bool> _isSelected;

    private ColorBlock _normalColors;
    private ColorBlock _selectedColors;
    private Button _button;

    private void Update()
    {
        if (_isSelected(_characterID))
            _button.colors = _selectedColors;
        else
            _button.colors = _normalColors;
    }

    public void Initialize(CharacterID characterID, Action<CharacterID> onSelected, Func<CharacterID, bool> isSelected)
    {
        _characterID = characterID;
        _onSelected = onSelected;
        _isSelected = isSelected;

        _portrait.SetCharacter(characterID);
        _characterNameText.text = _characterID.Name;

        _button = GetComponent<Button>();
        _button.onClick.AddListener(() => OnSelect());
        _normalColors = _button.colors;

        _selectedColors = _button.colors;
        var selectedColor = _button.colors.selectedColor;
        _selectedColors.normalColor = selectedColor;
        _selectedColors.highlightedColor = selectedColor;
        _selectedColors.pressedColor = selectedColor;
    }

    public void OnSelect()
    {
        _onSelected(_characterID);
    }

    public void SelectInitial()
    {
        _button.Select();
        OnSelect();
    }
}