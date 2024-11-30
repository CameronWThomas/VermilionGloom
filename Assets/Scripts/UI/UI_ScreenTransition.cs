using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class UI_ScreenTransition : MonoBehaviour
{
    public enum TransitionType
    {
        FromBelaImage,
        FromCenter
    }

    [Header("Spin transition stuff")]
    [SerializeField] RectTransform _belaImage;
    [SerializeField] GameObject _belaPrefab;
    [SerializeField, Range(0f, 20f)] float _batTransitionSpinSpeed = 15f;
    [SerializeField, Range(0f, 10f)] float _batTransitionDuration = 0.5f;
    [SerializeField, Range(0f, 10f)] float _batTransitionScale = 5f;

    RectTransform _mainScreen;

    public void Initialize(RectTransform mainScreen)
    {
        _mainScreen = mainScreen;
    }

    public void Transition(TransitionType transitionType, Action onMidTransition = null, Action onEndTransition = null, float? transitionSpeed = null, float? transitionDuration = null)
    {
        StartCoroutine(ProbeMindRoutine(transitionType, onMidTransition, onEndTransition, transitionSpeed, transitionDuration));
    }

    private IEnumerator ProbeMindRoutine(TransitionType transitionType, Action onMidTransition, Action onEndTransition, float? transitionSpeed = null, float? transitionDuration = null)
    {
        var newBela = Instantiate(_belaPrefab, _mainScreen);
        var newBellaRectTransform = newBela.GetComponent<RectTransform>();

        transitionType = TransitionType.FromCenter;
        if (transitionType is TransitionType.FromBelaImage)
        {
            _belaImage.gameObject.SetActive(false);
            newBellaRectTransform.anchoredPosition = _belaImage.anchoredPosition + new Vector2(18f, -18f); // Add padding of the horizontal groups
            newBellaRectTransform.sizeDelta = _belaImage.sizeDelta;
        }
        else if (transitionType is TransitionType.FromCenter)
        {
            var mainScreenRect = _mainScreen.rect;
            newBellaRectTransform.anchoredPosition = new Vector2(mainScreenRect.width / 2f, mainScreenRect.height / -2f);
            newBellaRectTransform.sizeDelta = Vector2.zero;
        }

        var originalSizeDelta = newBellaRectTransform.sizeDelta;
        var maxSizeDelta = _belaImage.sizeDelta * _batTransitionScale;
        var startSizeDelta = originalSizeDelta;
        var finalSizeDelta = maxSizeDelta;

        float tSpeed = transitionSpeed ?? _batTransitionSpinSpeed;
        float tDuration = transitionDuration ?? _batTransitionDuration;

        StartCoroutine(SpinRectTransform(newBellaRectTransform, tSpeed, tDuration));

        var duration = _batTransitionDuration;
        var originalStartTime = Time.time;
        var startTime = Time.time;
        while (Time.time - originalStartTime <= duration)
        {
            var t = (Time.time - startTime) / (duration / 2f);

            if (t > 1f)
            {
                onMidTransition?.Invoke();
                _belaImage.gameObject.SetActive(true);

                t = 0f;
                startTime = Time.time;
                finalSizeDelta = Vector2.zero;
                startSizeDelta = newBellaRectTransform.sizeDelta;
            }

            newBellaRectTransform.sizeDelta = Vector2.Lerp(startSizeDelta, finalSizeDelta, t);

            yield return new WaitForNextFrameUnit();
        }

        Destroy(newBela);
        onEndTransition?.Invoke();
    }

    private static IEnumerator SpinRectTransform(RectTransform rectTransform, float maxSpeed, float duration)
    {
        var startTime = Time.time;
        while (Time.time - startTime <= duration)
        {
            if (rectTransform.gameObject.IsDestroyed())
                yield break;

            var t = .2f + (Time.time - startTime) / (duration * .5f);
            var speed = Mathf.Lerp(0f, maxSpeed, Mathf.Clamp(t, 0f, 1f));

            var lastRotation = rectTransform.rotation;
            rectTransform.rotation = Quaternion.Euler(lastRotation.eulerAngles + speed * Vector3.forward);

            yield return new WaitForNextFrameUnit();
        }
    }
}