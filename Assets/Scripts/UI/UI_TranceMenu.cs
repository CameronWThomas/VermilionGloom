using System;
using UnityEngine;
using UnityEngine.UI;

public class UI_TranceMenu : UI_SectionBase
{
    [SerializeField] RectTransform _murderSection;
    [SerializeField] Button _back;
    [SerializeField] Button _beginTrance;
    [SerializeField] GridLayoutGroup _portraitPlace;
    [SerializeField] GameObject _portraitButtonPrefab;

    private void Start()
    {
        _back.onClick.AddListener(() => CharacterInteractionMenu.TransitionState(CharacterInteractingState.Default));
    }

    protected override void OnStateChanged(CharacterInteractingState state)
    {
        if (state is CharacterInteractingState.Trance)
            Unhide();
        else
            Hide();
    }

    protected override void UpdateHidden(bool hide)
    {
        _murderSection.gameObject.SetActive(!hide);
    }
}