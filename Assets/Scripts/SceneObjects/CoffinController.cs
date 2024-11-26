using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class CoffinController : MonoBehaviour
{
    [SerializeField] CoffinState _coffinStartingState = CoffinState.Closed;
    [SerializeField] float _defaultOpenCloseTime = 5f;
    [SerializeField] Transform _coffinClosedTop;
    [SerializeField] Transform _coffinOpenTop;
    [SerializeField] Transform _coffinMovingTop;

    CoffinState _coffinState;

    private void Start()
    {
        SetActiveStatesForCoffinTop(_coffinStartingState);

        _coffinState = _coffinStartingState;
    }    

    public IEnumerator OpenCoffin() => SetCoffinState(CoffinState.Open);
    public IEnumerator CloseCoffin() => SetCoffinState(CoffinState.Closed);

    private void SetActiveStatesForCoffinTop(CoffinState coffinState)
    {
        var closedOpenTopActive = coffinState is CoffinState.Open;
        var closedClosedTopActive = coffinState is CoffinState.Closed;

        _coffinClosedTop.gameObject.SetActive(closedClosedTopActive);
        _coffinOpenTop.gameObject.SetActive(closedOpenTopActive);
        _coffinMovingTop.gameObject.SetActive(false);
    }

    private IEnumerator SetCoffinState(CoffinState newCoffinState)
    {
        if (_coffinState == newCoffinState)
            yield break;

        _coffinState = newCoffinState;

        var (startingTransform, endingTransform, earlyRotate) = newCoffinState switch
        {
            CoffinState.Open => (_coffinClosedTop, _coffinOpenTop, false),
            _ or CoffinState.Closed => (_coffinOpenTop, _coffinClosedTop, true),
        };

        yield return CoffinRoutine(_defaultOpenCloseTime, startingTransform, endingTransform, earlyRotate);
    }

    private IEnumerator CoffinRoutine(float duration, Transform startingTransform, Transform endingTransform, bool earlyRotate)
    {
        //TODO eventually just make a coffin and play an animation. In fact, at that point all of this will not really be needed

        // Set the moving coffin top to the starting position and make sure on it is visible
        _coffinMovingTop.SetPositionAndRotation(startingTransform.position, startingTransform.rotation);
        _coffinMovingTop.gameObject.SetActive(true);
        startingTransform.gameObject.SetActive(false);
        endingTransform.gameObject.SetActive(false);

        yield return new WaitForNextFrameUnit();

        // Open position is slightly lower than the tops normal position. Handle that in the rotation
        var verticalDiff = endingTransform.position.y - startingTransform.position.y;
        if (earlyRotate)
            yield return RotateCoffinTop(duration / 3f, startingTransform.rotation, endingTransform.rotation, verticalDiff);

        var startingPosition = startingTransform.position;
        var endingPosition = endingTransform.position;
        if (earlyRotate)
            startingPosition.y = endingPosition.y;
        else
            endingPosition.y = startingPosition.y;

        var startTime = Time.time;
        while (Time.time - startTime <= duration)
        {
            var elapsedTime = Time.time - startTime;
            var t = elapsedTime / duration;

            _coffinMovingTop.position = Vector3.Slerp(startingPosition, endingPosition, t);
            yield return new WaitForNextFrameUnit();

        }

        if (!earlyRotate)
            yield return RotateCoffinTop(duration / 3f, startingTransform.rotation, endingTransform.rotation, verticalDiff);

        yield return new WaitForNextFrameUnit();

        SetActiveStatesForCoffinTop(_coffinState);
    }

    private IEnumerator RotateCoffinTop(float duration, Quaternion startingRotation, Quaternion endingRotation, float verticalDiff)
    {
        var startingPosition = _coffinMovingTop.position;

        var startTime = Time.time;
        while (Time.time - startTime <= duration)
        {
            var elapsedTime = Time.time - startTime;
            var t = elapsedTime / duration;

            var rotation = Quaternion.Slerp(startingRotation, endingRotation, t);
            var position = Vector3.Slerp(startingPosition, startingPosition + verticalDiff * Vector3.up, t);
            
            _coffinMovingTop.SetPositionAndRotation(position, rotation);
            yield return new WaitForNextFrameUnit();
        }

        _coffinMovingTop.rotation = endingRotation;
        yield return new WaitForNextFrameUnit();
    }

    private enum CoffinState
    {
        Closed,
        Open,
        Transitioning
    }
}