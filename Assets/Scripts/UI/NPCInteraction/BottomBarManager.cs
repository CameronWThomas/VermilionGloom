using System;
using System.Collections;
using UnityEngine;

public class BottomBarManager : MonoBehaviour
{
    public event Action<bool> OnMiniGameFinish;

    MiniGameManager _miniGameManager;

    public bool IsMiniGameRunning { get; private set; }

    private void Start()
    {
        _miniGameManager = GetComponentInChildren<MiniGameManager>();
        _miniGameManager.gameObject.SetActive(false);
    }

    public void RunMiniGame()
    {
        if (IsMiniGameRunning)
            return;

        _miniGameManager.gameObject.SetActive(true);


        StartCoroutine(WaitForGameCompletion());
    }

    private IEnumerator WaitForGameCompletion()
    {
        if (IsMiniGameRunning)
            yield break;

        IsMiniGameRunning = true;

        yield return _miniGameManager.StartMiniGame();
        
        IsMiniGameRunning = false;        
        OnMiniGameFinish?.Invoke(_miniGameManager.WasLastRunSuccessful);

        _miniGameManager.gameObject.SetActive(false);
    }
}
