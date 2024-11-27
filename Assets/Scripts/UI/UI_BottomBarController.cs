using System;
using TMPro;
using Unity.VisualScripting;
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

    protected static PlayerController PlayerController => PlayerStats.Instance.GetComponent<PlayerController>();

    private Sprite _unselectedSprite;
    private SpriteState _unselectedSpriteState;

    private Sprite _selectedSprite;
    private SpriteState _selectedSpriteState;

    protected override void Start()
    {
        base.Start();

        _hostile.onClick.AddListener(() => HostileClick(true));
        _interacting.onClick.AddListener(() => HostileClick(false));
        _running.onClick.AddListener(() => RunClick());

        _unselectedSprite = _hostile.image.sprite;
        _unselectedSpriteState = _hostile.spriteState;

        _selectedSprite = _hostile.spriteState.selectedSprite;
        _selectedSpriteState = new SpriteState
        {
            pressedSprite = _selectedSprite,
            selectedSprite = _selectedSprite,
            highlightedSprite = _selectedSprite,
            disabledSprite = _hostile.spriteState.disabledSprite
        };


        UpdateRunningButtons(false);
        UpdateHostileButtons(false);

        Off();
    }    

    private void Update()
    {
        UpdateRunningButtons(PlayerController.IsRunning);
        UpdateHostileButtons(PlayerController.hostile);
    }    

    public void Off()
    {
        _normalText.SetActive(false);
        _interactingCharacter.SetActive(false);
        SetButtonStates(true);
    }

    public void SetInteractingCharacter(CharacterID characterID)
    {
        _normalText.SetActive(false);
        _interactingCharacter.SetActive(true);

        SetButtonStates(false);

        //_characterPortrait.SetContent(characterID.PortraitContent);
        _characterPortrait.SetContent(characterID.PortraitColor);
        _characterName.text = characterID.Name;

        _characterTalking.text = "...";
    }

    private void UpdateHostileButtons(bool isHostile)
    {
        _hostile.image.sprite = isHostile ? _selectedSprite : _unselectedSprite;
        _interacting.image.sprite = !isHostile ? _selectedSprite : _unselectedSprite;

        _hostile.spriteState = isHostile ? _selectedSpriteState : _unselectedSpriteState;
        _interacting.spriteState = !isHostile ? _selectedSpriteState : _unselectedSpriteState;

        if (isHostile)
            _hostile.Select();
        else
            _interacting.Select();
    }

    private void UpdateRunningButtons(bool isRunning)
    {
        _running.image.sprite = isRunning ? _selectedSprite : _unselectedSprite;
        _running.spriteState = isRunning ? _selectedSpriteState : _unselectedSpriteState;

        if (isRunning)
            _running.Select();
    }

    private void HostileClick(bool isHostile)
    {
        PlayerController.GetHostile(isHostile);
    }

    private void RunClick()
    {
        PlayerController.Run(!PlayerController.IsRunning);
    }

    private void SetButtonStates(bool enabled)
    {
        _hostile.interactable = enabled;
        _interacting.interactable = enabled;
        _running.interactable = enabled;
    }
}