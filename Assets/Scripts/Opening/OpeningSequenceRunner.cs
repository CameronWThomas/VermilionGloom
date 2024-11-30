using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

public class OpeningSequenceRunner : MonoBehaviour
{
    [SerializeField] float _noteFlipUpTime = 1.5f;

    [SerializeField] Transform _assignmentNote;
    [SerializeField] Transform _bloodyNote;
    [SerializeField] Button _nextButton;
    [SerializeField] CarriageBounce _carriageBounce;
    [SerializeField] MovingEnvironment _movingEnvironment;

    [Header("Video stuff")]
    [SerializeField] VideoPlayer _videoPlayer;
    [SerializeField] VideoClip _authors;
    [SerializeField] VideoClip _quote;
    [SerializeField] VideoClip _titleCard;

    [Header("Audio stuff")]
    [SerializeField] AudioSource _thunderAudio;
    [SerializeField] AudioSource _rumble;

    [Header("Manor Light")]
    [SerializeField] Light _manorLight;

    [Header("debug")]
    public bool SkipFirstVideos = false;


    bool _nextButtonPressed = false;

    Vector3 _noteLocalPosition;
    Quaternion _noteLocalRotation;


    private void Start()
    {
        _noteLocalPosition = _assignmentNote.localPosition;
        _noteLocalRotation = _assignmentNote.localRotation;

        _assignmentNote.gameObject.SetActive(false);
        _bloodyNote.gameObject.SetActive(false);
        _nextButton.gameObject.SetActive(false);
        _manorLight.gameObject.SetActive(false);

        _nextButton.onClick.AddListener(() =>
        {
            _nextButtonPressed = true;
            _nextButton.interactable = false;
        });

        StartCoroutine(OpeningSequenceRoutine());
    }

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.K))
        //{
        //    _manorLight.gameObject.SetActive(true);
        //    _movingEnvironment.AttachTransform(_manorLight.transform);
        //}

        //if (_manorLight.transform.position.x <= 0f)
        //{
        //    _movingEnvironment.StopMoving();
        //    _carriageBounce.StopMoving();
        //}

        //if (Input.GetKeyDown(KeyCode.F))
        //    StartCoroutine(OpeningFadeToBlackController.Instance.FadeFromBlackRoutine(5f));
        //else if (Input.GetKeyDown(KeyCode.N))
        //    StartCoroutine(FlipNote(_assignmentNote, true));
        //else if (Input.GetKeyDown(KeyCode.V))
        //    StartCoroutine(FlipNote(_assignmentNote, false));
    }

    private IEnumerator OpeningSequenceRoutine()
    {
        // TODO Play opening credit stuff

        if (!SkipFirstVideos)
        {
            yield return new WaitForSeconds(2f);

            _videoPlayer.clip = _authors;
            _videoPlayer.Play();

            yield return new WaitForSeconds(1f); // time to let the video start playing
            while (_videoPlayer.isPlaying)
                yield return new WaitForNextFrameUnit();

            _videoPlayer.clip = _quote;
            _videoPlayer.Play();

            yield return new WaitForSeconds(1f); // time to let the video start playing
            while (_videoPlayer.isPlaying)
                yield return new WaitForNextFrameUnit();

            _videoPlayer.gameObject.SetActive(false);

            yield return new WaitForSeconds(3f);
        }
        else
        {
            _videoPlayer.gameObject.SetActive(false);
        }

        yield return OpeningFadeToBlackController.Instance.FadeFromBlackRoutine(5f);

        yield return new WaitForSeconds(1f);

        yield return FlipNote(_assignmentNote, true);

        yield return new WaitForSeconds(1f);

        _nextButton.gameObject.SetActive(true);
        _nextButton.interactable = true;
        _nextButtonPressed = false;

        while (!_nextButtonPressed)
            yield return new WaitForSeconds(.1f);

        _nextButton.gameObject.SetActive(false);
        _nextButtonPressed = false;

        yield return FlipNote(_assignmentNote, false);

        yield return new WaitForSeconds(2f);

        yield return FlipNote(_bloodyNote, true);

        yield return new WaitForSeconds(1f);

        _nextButton.gameObject.SetActive(true);
        _nextButton.interactable = true;
        _nextButtonPressed = false;

        while (!_nextButtonPressed)
            yield return new WaitForSeconds(.1f);

        _nextButton.gameObject.SetActive(false);
        _nextButtonPressed = false;

        yield return FlipNote(_bloodyNote, false);

        _manorLight.gameObject.SetActive(true);

        _movingEnvironment.AttachTransform(_manorLight.transform);

        var nearStop = false;
        while (_manorLight.transform.position.x > 0f)
        {
            if (!nearStop && _manorLight.transform.position.x < .2f)
            {
                nearStop = true;
                _carriageBounce.NearingStop();
            }
            yield return new WaitForNextFrameUnit();
        }

        _movingEnvironment.StopMoving();
        _carriageBounce.StopMoving();

        yield return new WaitForSeconds(5f);

        _videoPlayer.gameObject.SetActive(true);

        _videoPlayer.clip = _titleCard;
        _videoPlayer.Play();

        _thunderAudio.Stop();
        _rumble.Play();

        yield return new WaitForSeconds(1f); // time to let the video start playing
        while (_videoPlayer.isPlaying)
            yield return new WaitForNextFrameUnit();

        SceneManager.LoadScene("TheManor");
    }

    private IEnumerator FlipUpNote(Transform note)
    {
        var startPosition = _noteLocalPosition - Vector3.up * .5f;
        var endPosition = _noteLocalPosition;

        var noteEuler = _noteLocalRotation.eulerAngles;
        var startRotation = Quaternion.Euler(-noteEuler.x, noteEuler.y, noteEuler.z);
        var endRotation = _noteLocalRotation;

        note.gameObject.SetActive(true);

        var duration = _noteFlipUpTime;
        var startTime = Time.time;
        while (Time.time - startTime <= duration)
        {
            var t = (Time.time - startTime) / duration;

            var position = Vector3.Slerp(startPosition, endPosition, t);
            var rotation = Quaternion.Slerp(startRotation, endRotation, t);

            note.SetLocalPositionAndRotation(position, rotation);

            yield return new WaitForNextFrameUnit();
        }

        note.SetLocalPositionAndRotation(endPosition, endRotation);

    }

    private IEnumerator FlipNote(Transform note, bool up)
    {
        var startPosition = _noteLocalPosition - Vector3.up;
        var endPosition = _noteLocalPosition;        

        var noteEuler = _noteLocalRotation.eulerAngles;
        var startRotation = Quaternion.Euler(0f, noteEuler.y, noteEuler.z);
        var endRotation = _noteLocalRotation;

        if (!up)
        {
            var tempPosition = startPosition;
            startPosition = endPosition;
            endPosition = tempPosition;

            var tempRotation = startRotation;
            startRotation = endRotation;
            endRotation = tempRotation;
        }

        note.gameObject.SetActive(true);

        var duration = _noteFlipUpTime;
        var startTime = Time.time;
        while (Time.time - startTime <= duration)
        {
            var t = (Time.time - startTime) / duration;

            var position = Vector3.Slerp(startPosition, endPosition, t);
            var rotation = Quaternion.Slerp(startRotation, endRotation, t);

            note.SetLocalPositionAndRotation(position, rotation);

            yield return new WaitForNextFrameUnit();
        }

        note.SetLocalPositionAndRotation(endPosition, endRotation);

        if (!up)
            note.gameObject.SetActive(false);
    }
}