using System.Collections;
using UnityEngine;

public abstract class SequenceBase : MonoBehaviour
{
    [SerializeField] bool _sequencePlayable = true;
    public bool SequenceFinished = false;

    protected VampireController _vampire;


    protected virtual void Start()
    {
        _vampire = GameState.Instance.Vampire;
        SequenceFinished = false;
    }

    protected void Update()
    {
        if (!_sequencePlayable || !SequencePlayingCondition())
            return;

        PlaySequence();
    }

    protected abstract bool SequencePlayingCondition();    

    protected virtual void OnInitializePlayable() { }

    protected void SetInitialState(bool sequencePlayed)
    {
        _sequencePlayable = !sequencePlayed;
        if (_sequencePlayable)
            OnInitializePlayable();
    }

    protected void PlaySequence()
    {
        if (!_sequencePlayable)
            return;

        _sequencePlayable = false;

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

    protected static IEnumerator GoToTarget(MvmntController mvmntController, Transform target)
    {
        var walkDone = false;
        mvmntController.GoToTarget(target, () => walkDone = true, () => walkDone = true);

        while (!walkDone)
            yield return new WaitForEndOfFrame();
    }

    protected static IEnumerator FaceTarget(MvmntController mvmntController, Transform target)
    {
        yield return new WaitForEndOfFrame();
        mvmntController.FaceTarget(target.position);
        yield return new WaitForEndOfFrame();
    }

    protected static IEnumerator MoveCameraToTarget(Transform target, float duration)
    {
        var cameraTransform = Camera.main.transform;
        var followCamera = cameraTransform.GetComponent<FollowCam>();
        followCamera.enabled = false;

        var offset = followCamera.InitialOffset;

        var startingPosition = cameraTransform.position;
        var endingPosition = target.position + offset;

        yield return Slerp(cameraTransform, startingPosition, endingPosition, duration);
    }

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

    protected static IEnumerator Slerp(Transform slerpedTransform, Transform endTransform, float duration)
    {
        var position = slerpedTransform.position;
        var rotation = slerpedTransform.rotation;

        yield return Slerp(slerpedTransform,
            position, rotation,
            endTransform.position, endTransform.rotation,
            duration);
    }

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