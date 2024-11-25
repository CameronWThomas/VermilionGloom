using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public abstract class SequenceBase : MonoBehaviour
{
    [Header("General Segment Stuff")]
    [SerializeField] bool _sequencePlayable = true;
    [SerializeField] float _zoomCameraTime = 1f;
    [SerializeField] float _sequenceFOV = 10f;

    public bool SequenceFinished = false;

    protected VampireController _vampire;
    protected float _startingFOV;

    protected UsefulTransforms UsefulTransforms => UsefulTransforms.Instance;


    protected virtual void Start()
    {
        _vampire = GameState.Instance.Vampire;
        _startingFOV = Camera.main.fieldOfView;

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

    protected IEnumerator VampireToDefaultPosition(float duration)
    {
        yield return Slerp(_vampire.transform, UsefulTransforms.V_Default, duration);
    }

    protected IEnumerator StartSucking()
    {
        _vampire.Suck(true);
        yield break;
    }

    protected IEnumerator StopSucking()
    {
        _vampire.Suck(false);
        yield break;
    }

    protected IEnumerator Bless()
    {
        _vampire.Bless();        
        yield return new WaitForSeconds(3f);
    }

    protected static Transform PlayerTransform => PlayerStats.Instance.transform;
    protected static PlayerController PlayerController => PlayerTransform.GetComponent<PlayerController>();

    protected IEnumerator ZoomCameraStartSequence() => ZoomCamera(_startingFOV, _sequenceFOV, _zoomCameraTime);
    protected IEnumerator ZoomCameraEndSequence() => ZoomCamera(_sequenceFOV, _startingFOV, _zoomCameraTime);

    protected static IEnumerator ZoomCamera(float startFOV, float endFOV, float duration)
    {
        var startTime = Time.time;
        while (Time.time - startTime <= duration)
        {
            var elapsedTime = Time.time - startTime;
            var t = elapsedTime / duration;

            Camera.main.fieldOfView = Mathf.SmoothStep(startFOV, endFOV, t);
            yield return new WaitForNextFrameUnit();
        }

        Camera.main.fieldOfView = endFOV;
    }


    protected IEnumerator VampireToTargets(params Transform[] targets)
    {
        var navMeshAgent = _vampire.GetComponent<NavMeshAgent>();
        var mvmntController = _vampire.GetComponent<MvmntController>();

        navMeshAgent.enabled = true;
        mvmntController.enabled = true;
        yield return GoToTargets(_vampire.transform, targets);
        navMeshAgent.enabled = false;
        mvmntController.enabled = false;
    }

    protected static IEnumerator PlayerToTargets(params Transform[] targets) => GoToTargets(PlayerTransform, targets);    

    protected static IEnumerator GoToTargets(Transform movingTransform, params Transform[] targets)
    {
        var mvmntController = movingTransform.GetComponent<MvmntController>();

        foreach (var target in targets)
        {
            var walkDone = false;
            mvmntController.GoToTarget(target, () => walkDone = true, () => walkDone = true);

            while (!walkDone)
                yield return new WaitForEndOfFrame();
        }
    }

    protected static IEnumerator PlayerFaceTarget(Transform target) => FaceTarget(PlayerTransform.GetComponent<MvmntController>(), target);

    protected static IEnumerator FaceTarget(MvmntController mvmntController, Transform target)
    {
        yield return new WaitForEndOfFrame();
        mvmntController.FaceTarget(target.position);
        yield return new WaitForEndOfFrame();
    }

    protected static IEnumerator MoveCameraToPlayer(float duration, bool turnBackOnFollowCam = true)
    {
        yield return MoveCameraToTarget(PlayerTransform, duration);

        if (turnBackOnFollowCam)
            Camera.main.transform.GetComponent<FollowCam>().enabled = true;
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