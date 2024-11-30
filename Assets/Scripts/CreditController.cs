using System.Collections;
using UnityEngine;

public class CreditController : GlobalSingleInstanceMonoBehaviour<CreditController>
{
    [SerializeField] AudioClip _finalSong;
    [SerializeField] GameObject _titleCard;

    AudioSource _audioSource;
    bool _creditsStarted = false;

    protected override void Start()
    {
        base.Start();
        _audioSource = GetComponent<AudioSource>();
        _titleCard.gameObject.SetActive(false);
    }

    public void PlayFinalSong()
    {
        if (_creditsStarted)
            return;
        _creditsStarted = true;
        _audioSource.Stop();
        _audioSource.PlayOneShot(_finalSong);
        _audioSource.loop = false;

        StartCoroutine(CreditsCoroutine());
        // 36s show title card
        // 50s play credits
    }

    private IEnumerator CreditsCoroutine()
    {
        // 243s song

        yield return new WaitForSeconds(36f);

        _titleCard.gameObject.SetActive(true);

        yield return new WaitForSeconds(14f);

        _titleCard.gameObject.SetActive(false);

        yield return new WaitForSeconds(180f);

        yield return FadeToBlackController.Instance.FadeToBlackRoutine(5f);
    }
}