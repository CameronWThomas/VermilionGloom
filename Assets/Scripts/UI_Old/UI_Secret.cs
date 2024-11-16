using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Secret : MonoBehaviour
{
    [SerializeField] public GameObject _rumourCharacter;
    [SerializeField] public GameObject _targetOfSecret;
    [SerializeField] public TMP_Text _descriptionText;
    [SerializeField] public RawImage _tileImage;

    public void Initialize(Secret secret, bool isRumour)
    {
        SetupTileTexture(secret);
        SetupRelevantCharacters(secret, isRumour);
        SetupDescription(secret);
    }    

    private void SetupTileTexture(Secret secret)
    {
        _tileImage.texture = secret.IconTexture;
    }

    private void SetupRelevantCharacters(Secret secret, bool isRumour)
    {
        _rumourCharacter.SetActive(false);
        _targetOfSecret.SetActive(false);

        if (isRumour)
            _rumourCharacter.SetActive(true);
        
        if (secret.HasAdditionalCharacter)
            _targetOfSecret.SetActive(true);
    }

    private void SetupDescription(Secret secret)
    {
        _descriptionText.text = secret.Description;
    }
}