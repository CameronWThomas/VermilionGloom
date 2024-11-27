using System;
using System.Collections;
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

    private RectMask2D _mask;
    private bool _isHidden = false;
    private float _defaultTopPadding;

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

        _mask = GetComponent<RectMask2D>();
        _isHidden = false;
        _defaultTopPadding = _mask.padding.w;

        UpdateRunningButtons(false);
        UpdateHostileButtons(false);

        Default();
    }

    private void Update()
    {
        UpdateRunningButtons(PlayerController.IsRunning);
        UpdateHostileButtons(PlayerController.hostile);
        UpdateButtonInteractability();
    }

    public void Default()
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

    public void SetHidden(bool hidden) => StartCoroutine(HideRoutine(hidden));

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

    private void UpdateButtonInteractability()
    {
        var enabled = MouseReceiver.Instance.IsActivated;

        _hostile.interactable = enabled;
        _interacting.interactable = enabled;
        _running.interactable = enabled;
    }

    private IEnumerator HideRoutine(bool hide)
    {
        if (_isHidden == hide)
            yield break;

        _isHidden = hide;

        var padding = _mask.padding;
        var start = hide ? _defaultTopPadding : _defaultTopPadding * -1;
        var end = start * -1;

        var duration = 3f;
        var startTime = Time.time;
        while (Time.time - startTime <= duration)
        {
            var t = (Time.time - startTime) / duration;

            padding.w = Mathf.Lerp(start, end, t);
            _mask.padding = padding;

            yield return new WaitForNextFrameUnit();
        }
    }
}