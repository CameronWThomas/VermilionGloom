using BehaviorDesigner.Runtime.Tasks.Unity.UnityQuaternion;
using System;
using System.Collections;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public abstract class SequenceBase : MonoBehaviour
{
    public bool SequencePlayable = true;
    public bool SequenceFinished = false;

    protected virtual void Start()
    {
        SequenceFinished = false;
    }

    protected virtual void SetInitialState(bool sequencePlayed)
    {
        SequencePlayable = !sequencePlayed;
    }

    protected void PlaySequence()
    {
        if (!SequencePlayable)
            return;

        SequencePlayable = false;

        var sequenceRunner = GetSequenceRunner();
        OnSequenceStart();
        sequenceRunner.Run(this, OnSequenceEnd);
    }

    protected virtual void OnSequenceStart()
    {
        MouseReceiver.Instance.Deactivate();
    }

    protected virtual void OnSequenceEnd()
    {
        MouseReceiver.Instance.Activate();
        SequenceFinished = true;        
    }

    protected abstract SequenceRunner GetSequenceRunner();

    protected static IEnumerator Slerp(Transform slerpedTransform,
        Vector3 startingPosition, Vector3 finalPosition,
        float duration)
        => Slerp(slerpedTransform,
            startingPosition, slerpedTransform.rotation,
            finalPosition, slerpedTransform.rotation,
            duration);

    protected static IEnumerator Slerp(Transform slerpedTransform,
        Transform start, Transform end,
        float duration)
        => Slerp(slerpedTransform,
            start.position, start.rotation,
            end.position, end.rotation,
            duration);

    protected static IEnumerator Slerp(Transform slerpedTransform,
        Vector3 startingPosition, Quaternion startingRotation,
        Vector3 finalPosition, Quaternion finalRotation,
        float duration)
    {
        var startTime = Time.time;
        while (Time.time - startTime < duration)
        {
            var elapsedTime = Time.time - startTime;
            var t = elapsedTime / duration;

            var rotation = Quaternion.Slerp(startingRotation, finalRotation, t);
            var position = Vector3.Slerp(startingPosition, finalPosition, t);

            slerpedTransform.SetPositionAndRotation(position, rotation);

            yield return new WaitForEndOfFrame();
        }

        yield return new WaitForEndOfFrame();
        slerpedTransform.SetPositionAndRotation(finalPosition, finalRotation);
        yield return new WaitForEndOfFrame();
    }
}