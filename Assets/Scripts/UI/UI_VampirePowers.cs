using System;
using UnityEngine;
using UnityEngine.UI;

public class UI_VampirePowers : UI_SectionBase
{
    [Header("Vampire power stuff")]
    [SerializeField] RectTransform _vampPowerSection;
    [SerializeField] Button _probeMind;
    [SerializeField] Button _trance;
    [SerializeField] Button _forget;

    private void Start()
    {
        //_forget.onClick.AddListener(OnForgetClicked);
    }

    public void Initialize(Action onProbeMindClicked, Action onTranceClicked)
    {
        _probeMind.onClick.AddListener(() => onProbeMindClicked());
        _trance.onClick.AddListener(() => onTranceClicked());
    }

    public override void InitializeForNewCharacter(NPCHumanCharacterID characterId)
    {
        base.InitializeForNewCharacter(characterId);
        Unhide();
    }

    protected override void OnProbeMindChange(bool mindProbed)
    {
        _trance.gameObject.SetActive(mindProbed);
        _probeMind.gameObject.SetActive(!mindProbed);
    }

    protected override void UpdateHidden(bool hide)
    {
        _vampPowerSection.gameObject.SetActive(!hide);
    }
}