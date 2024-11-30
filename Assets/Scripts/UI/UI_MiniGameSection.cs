using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UI_MiniGameSection : UI_SectionBase
{
    [SerializeField] UI_MiniGame _gameSection;
    [SerializeField] Button _backButton;

    private Action<bool?> _onGameEnd = null;

    private void Start()
    {
        _backButton.onClick.AddListener(() => QuitMiniGame());
    }

    public void GetNextMiniGameEnd(Action<bool?> onGameEnd)
    {
        _onGameEnd = onGameEnd;
    }

    protected override void OnStateChanged(CharacterInteractingState state)
    {
        if (state is CharacterInteractingState.MiniGame)
            StartCoroutine(StartGame());
        else
            Hide();
    }

    protected override void UpdateHidden(bool hide)
    {
        _gameSection.gameObject.SetActive(!hide);
    }

    private IEnumerator StartGame()
    {
        Unhide();

        yield return new WaitForNextFrameUnit();

        _gameSection.Initialize(OnGameFinished);

        yield return new WaitForNextFrameUnit();

        _gameSection.PlayMiniGame();
    }    

    private void QuitMiniGame()
    {
        _gameSection.StopMiniGame();
        _onGameEnd?.Invoke(null);
    }

    private void OnGameFinished(bool successful)
    {
        _onGameEnd?.Invoke(successful);
    }
}