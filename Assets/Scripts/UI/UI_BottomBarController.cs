using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_BottomBarController : GlobalSingleInstanceMonoBehaviour<UI_BottomBarController>
{
    [SerializeField] Button _hostile;
    [SerializeField] Button _interacting;
    [SerializeField] Button _running;

    [SerializeField] GameObject _normalText;
    [SerializeField] TMP_Text _generalDescriptionText;

    [SerializeField] GameObject _interactingCharacter;
    [SerializeField] UI_Portrait _characterPortrait;
    [SerializeField] TMP_Text _characterName;
    [SerializeField] TMP_Text _characterTalking;

    protected override void Start()
    {
        base.Start();

        Off();
    }

    public void Off()
    {
        _normalText.SetActive(false);
        _interactingCharacter.SetActive(false);
    }

    public void SetInteractingCharacter(CharacterID characterID)
    {
        _normalText.SetActive(false);
        _interactingCharacter.SetActive(true);

        //_characterPortrait.SetContent(characterID.PortraitContent);
        _characterPortrait.SetContent(characterID.PortraitColor);
        _characterName.text = characterID.Name;

        _characterTalking.text = "...";
    }
}