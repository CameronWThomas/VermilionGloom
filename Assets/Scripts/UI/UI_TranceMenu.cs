using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_TranceMenu : UI_SectionBase
{
    [SerializeField] RectTransform _murderSection;
    [SerializeField] Button _back;
    [SerializeField] Button _beginTrance;
    [SerializeField] GridLayoutGroup _portraitPlace;
    [SerializeField] GameObject _portraitButtonPrefab;
    [SerializeField] TMP_Text _noCharacterText;

    private NpcBrain _brain;

    private List<UI_PortraitButton> _portraitButtons = new();
    private CharacterID _selectedCharacter = null;

    private void Start()
    {
        _back.onClick.AddListener(() => CharacterInteractionMenu.TransitionState(CharacterInteractingState.Default));
        _beginTrance.onClick.AddListener(() => BeginTrance());
    }   

    public override void InitializeForNewCharacter(NPCHumanCharacterID characterId, Func<CharacterInteractingState> getState)
    {
        base.InitializeForNewCharacter(characterId, getState);

        _brain = _characterInfo.GetComponent<NpcBrain>();
        _selectedCharacter = null;
        _beginTrance.interactable = false;        
    }

    public override void Deactivate()
    {
        base.Deactivate();

        _characterSecretKnowledge = null;
        _brain = null;
        _selectedCharacter = null;

        ClearPortraitButtons();
    }

    protected override void OnStateChanged(CharacterInteractingState state)
    {
        if (state is CharacterInteractingState.Trance)
        {
            _noCharacterText.gameObject.SetActive(false);
            ClearPortraitButtons();
            AddPortraitButtons();
            Unhide();
        }
        else
            Hide();
    }

    protected override void UpdateHidden(bool hide)
    {
        _murderSection.gameObject.SetActive(!hide);
    }

    private void ClearPortraitButtons()
    {
        _portraitButtons.ForEach(x => Destroy(x.gameObject));
        _portraitButtons.Clear();
    }

    private void AddPortraitButtons()
    {
        var existingMurderSecrets = _characterSecretKnowledge.Secrets.OfType<MurderSecret>();

        // Characters we know about that aren't this NPC us and we don't already have a murder secret with
        var knownCharacters = CharacterInfoBB.Instance.GetPlayerCharacterInfo().GetKnownCharacters(true)
            .Where(x => x != _characterID && existingMurderSecrets.All(murderSecret => murderSecret.SecretTarget != x))
            .ToList();

        if (!knownCharacters.Any())
        {
            _noCharacterText.gameObject.SetActive(true);
            return;
        }

        //sort known characters by distance to selected character
        CharacterInfo selected = CharacterInfoBB.Instance.GetCharacterInfo(CharacterInfoBB.Instance.GetPlayerCharacterInfo().ID);
        Dictionary<CharacterID, float> sortedList = new Dictionary<CharacterID, float>();
        foreach(var character in knownCharacters)
        {
            Debug.Log("trying to find character: "+ character.Name);
            CharacterInfo known = CharacterInfoBB.Instance.GetCharacterInfo(character);
            float dist = Vector3.Distance(selected.gameObject.transform.position, known.gameObject.transform.position);
            sortedList.Add(character, dist);
        }
        sortedList = sortedList.OrderBy(entry => entry.Value).ToDictionary(entry => entry.Key, entry => entry.Value);
        var sortedKnownCharacters = sortedList.Keys.ToList();


        foreach (var characterID in sortedKnownCharacters)
        {
            var portraitButton = Instantiate(_portraitButtonPrefab, _portraitPlace.transform).GetComponent<UI_PortraitButton>();
            _portraitButtons.Add(portraitButton);

            portraitButton.Initialize(characterID, OnSelected, IsSelected);
        }
    }

    private void OnSelected(CharacterID iD)
    {
        _selectedCharacter = iD;
        _beginTrance.interactable = true;
    }

    private bool IsSelected(CharacterID iD)
    {
        return _selectedCharacter == iD;
    }

    private void BeginTrance()
    {
        if (_selectedCharacter == null)
            return;

        MiniGameSection.GetNextMiniGameEnd(OnGameEnd);

        CharacterInteractionMenu.TransitionState(CharacterInteractingState.MiniGame);


        
    }

    private void OnGameEnd(bool? gameResults)
    {
        if (!gameResults.HasValue || !gameResults.Value)
        {
            CharacterInteractionMenu.TransitionState(CharacterInteractingState.Trance);
            return;
        }

        // Create and add a murder secret
        var murderSecret = new MurderSecret.Builder(_characterID, SecretLevel.Private)
            .SetMurderer(_selectedCharacter)
            .WasSuccessfulMuder()
            .Build();

        _characterSecretKnowledge.AddSecret(murderSecret);

        _brain.ReevaluateRelationship(_selectedCharacter);

        CharacterInteractionMenu.TransitionState(CharacterInteractingState.Default);
    }
}