using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CreditController : GlobalSingleInstanceMonoBehaviour<CreditController>
{
    [SerializeField] AudioClip _finalSong;
    [SerializeField] GameObject _titleCard;
    [SerializeField] Image _fadeToBlack;

    AudioSource _audioSource;
    bool _creditsStarted = false;

    protected override void Start()
    {
        base.Start();
        _audioSource = GetComponent<AudioSource>();
        _titleCard.gameObject.SetActive(false);
        _fadeToBlack.gameObject.SetActive(false);
    }

    public void PlayFinalSong()
    {
        if (_creditsStarted)
            return;
        _creditsStarted = true;

        _audioSource.PlayOneShot(_finalSong);

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

        yield return FadeToBlack(5f);
    }

    private IEnumerator FadeToBlack(float duration)
    {
        var color = Color.black;
        color.a = 0f;

        _fadeToBlack.color = color;
        _fadeToBlack.gameObject.SetActive(true);

        var startTime = Time.time;
        while (Time.time - startTime < duration)
        {
            var t = (Time.time - startTime) / duration;

            color.a = Mathf.Lerp(0f, 1f, t);
            _fadeToBlack.color = color;

            yield return new WaitForNextFrameUnit();
        }
    }
}