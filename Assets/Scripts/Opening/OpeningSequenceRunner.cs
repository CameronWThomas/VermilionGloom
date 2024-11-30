using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class OpeningSequenceRunner : MonoBehaviour
{
    [SerializeField] float _noteFlipUpTime = 1.5f;

    [SerializeField] Transform _assignmentNote;
    [SerializeField] Transform _bloodyNote;
    [SerializeField] Button _nextButton;
    [SerializeField] CarriageBounce _carriageBounce;

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

        _nextButton.onClick.AddListener(() =>
        {
            _nextButtonPressed = true;
            _nextButton.interactable = false;
        });

        StartCoroutine(OpeningSequenceRoutine());
    }

    private void Update()
    {
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

        yield return new WaitForSeconds(3f);

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

        _carriageBounce.StopCarriageRide();
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