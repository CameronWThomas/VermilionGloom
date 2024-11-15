using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Secret : MonoBehaviour
{
    [SerializeField] public GameObject _singleCharacter;
    [SerializeField] public GameObject _multiCharacter;
    [SerializeField] public TMP_Text _descriptionText;
    [SerializeField] public RawImage _tileImage;

    public void Initialize(Secret secret)
    {
        SetupTileTexture(secret);
        SetupRelevantCharacters(secret);
        SetupDescription(secret);
    }    

    private void SetupTileTexture(Secret secret)
    {
        _tileImage.texture = secret.IconTexture;
    }

    private void SetupRelevantCharacters(Secret secret)
    {
        if (!secret.InvolvesCharacters)
        {
            _singleCharacter.SetActive(false);
            _multiCharacter.SetActive(false);
        }
        else if (secret.InvolvesMulitpleCharacters)
        {
            _singleCharacter.SetActive(true);
            _multiCharacter.SetActive(false);
        }
        else
        {
            _singleCharacter.SetActive(false);
            _multiCharacter.SetActive(true);
        }
    }

    private void SetupDescription(Secret secret)
    {
        _descriptionText.text = secret.Description;
    }
}