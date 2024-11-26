using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class FadeToBlackController : GlobalSingleInstanceMonoBehaviour<FadeToBlackController>
{
    [SerializeField] Image _image;

    bool _firstUpdate = true;

    protected override void Start()
    {
        base.Start();
        
        _image.color = Color.black;
    }

    private void Update()
    {
        if (_firstUpdate)
        {
            _firstUpdate = false;
            StartCoroutine(FadeFromBlackRoutine(3f));
        }
    }

    public IEnumerator FadeToBlackRoutine(float duration) => FadeToColor(Color.black, duration);

    public IEnumerator FadeFromBlackRoutine(float duration) => FadeFromColor(Color.black, duration);

    private IEnumerator FadeFromColor(Color color, float duration)
    {
        _image.gameObject.SetActive(true);
        yield return FadeColor(color, 1f, 0f, duration);
        _image.gameObject.SetActive(false);
    }

    private IEnumerator FadeToColor(Color color, float duration)
    {
        _image.gameObject.SetActive(true);
        yield return FadeColor(color, 0f, 1f, duration);
    }

    private IEnumerator FadeColor(Color color, float startA, float endA, float duration)
    {
        color.a = startA;
        _image.color = color;

        var startTime = Time.time;
        while (Time.time - startTime < duration)
        {
            var t = (Time.time - startTime) / duration;

            color.a = Mathf.Lerp(startA, endA, t);
            _image.color = color;

            yield return new WaitForNextFrameUnit();
        }

        color.a = endA;
        _image.color = color;
    }
}