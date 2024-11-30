using UnityEngine;
using UnityEngine.UI;

public class UI_VampirePowers : UI_SectionBase
{
    [Header("Vampire power stuff")]
    [SerializeField] RectTransform _vampPowerSection;
    [SerializeField] Button _probeMind;
    [SerializeField] Button _trance;

    private void Start()
    {
        _trance.onClick.AddListener(() => CharacterInteractionMenu.TransitionState(CharacterInteractingState.Trance));
        _probeMind.onClick.AddListener(() => OnProbeMindClicked());
    }

    protected override void OnStateChanged(CharacterInteractingState state)
    {
        if (state is CharacterInteractingState.MiniGame)
        {
            Hide();
            return;
        }

        Unhide();

        if (state is CharacterInteractingState.Unprobed)
        {
            _trance.gameObject.SetActive(false);
            _probeMind.gameObject.SetActive(true);
        }
        else if (state is CharacterInteractingState.Default)
        {
            _trance.gameObject.SetActive(true);
            _trance.interactable = true;
            _probeMind.gameObject.SetActive(false);
        }
        else
        {
            _trance.gameObject.SetActive(true);
            _trance.interactable = false;
            _probeMind.gameObject.SetActive(false);
        }
    }

    protected override void UpdateHidden(bool hide)
    {
        _vampPowerSection.gameObject.SetActive(!hide);
    }

    private void OnProbeMindClicked()
    {
        _characterInfo.MindProbed = true;

        CharacterInfoBB.Instance.GetPlayerCharacterInfo().AddProbedCharacter(_characterID);

        CharacterInteractionMenu.TransitionState(CharacterInteractingState.Default, UI_ScreenTransition.TransitionType.FromBelaImage);
    }
}