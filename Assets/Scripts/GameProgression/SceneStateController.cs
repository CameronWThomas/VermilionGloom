using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneStateController : GlobalSingleInstanceMonoBehaviour<SceneStateController>
{
    private bool _gameRestarting = false;

    private void Update()
    {
        var playerCharacterInfo = CharacterInfoBB.Instance.GetPlayerCharacterInfo();
        if (playerCharacterInfo == null)
            return;

        if (!_gameRestarting && playerCharacterInfo.IsDead)
        {
            _gameRestarting = true;
            StartCoroutine(RestartGame());
        }
    }

    public IEnumerator RestartGame()
    {
        Debug.Log("Game restarting in a few secs...");

        yield return new WaitForSeconds(3f);

        yield return FadeToBlackController.Instance.FadeToBlackRoutine(3f);

        yield return new WaitForSeconds(.25f);

        Debug.Log("Game restarting");
        SceneManager.LoadScene("TheManor");
    }
}