using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class VampireDiscoverySequence : SequenceBase
{
    //TODO some of these can be handled better by making animations if there is time

    [Header("Segment Speeds")]
    [SerializeField] float _zoomCameraTime = 1f;
    [SerializeField] float _moveCameraTime = 3f;
    [SerializeField] float _floatToAboveCoffinTime = 3f;
    [SerializeField] float _vampireToDefaultPositionTime = 3f;

    [Header("Camera Stuff")]
    [SerializeField] float _sequenceFOV = 10f;

    private VampireController _vampire;
    private CoffinController _coffinController;

    private float _startingFOV;

    private UsefulTransforms UsefulTransforms => UsefulTransforms.Instance;

    protected override void Start()
    {
        base.Start();
        var gameState = GameState.Instance;
        
        _vampire = gameState.Vampire;
        _coffinController = gameState.CoffinController;

        //TODO sequence should look to this. Not other wayu around
        SetInitialState(gameState.VampireLordVisited);
    }

    private void Update()
    {
        if (!SequencePlayable || !GetComponent<TriggerVolume>().IsPlayerPresent)
            return;

        PlaySequence();
    }

    protected override void SetInitialState(bool sequencePlayed)
    {
        base.SetInitialState(sequencePlayed);

        if (sequencePlayed)
            return;

        // Turn off the vampire
        GameState.Instance.Vampire.gameObject.SetActive(false);
    }

    protected override SequenceRunner GetSequenceRunner()
    {
        return new SequenceRunner()
            .AddWait(5f)
            .AddRoutine(PlayerToCoffinAndLookingAtIt)
            .AddRoutine(MoveCameraToCoffin)
            .AddRoutine(() => _coffinController.OpenCoffin())
            .AddWait(1f)
            .AddRoutine(FloatAboveCoffin)
            .AddWait(3f) // Talking time
            .AddRoutine(() => _coffinController.CloseCoffin()) // Talking time
            .AddWait(3f) // Talking time
            .AddRoutine(FloatDownToNormalPosition)
            .AddWait(1f)
            .AddRoutine(MoveCameraToPlayer);
    }

    protected override void OnSequenceStart()
    {
        base.OnSequenceStart();
        _startingFOV = Camera.main.fieldOfView;
    }

    protected override void OnSequenceEnd()
    {
        base.OnSequenceEnd();
        GameState.Instance.VampireLordVisited = true;
    }

    private IEnumerator PlayerToCoffinAndLookingAtIt()
    {
        var playerTransform = PlayerStats.Instance.transform;
        var mvmntController = playerTransform.GetComponent<MvmntController>();

        var finishedLookingAtCoffin = false;
        mvmntController.GoToTarget(UsefulTransforms.P_AddressingVampire, () => finishedLookingAtCoffin = true, () => finishedLookingAtCoffin = true);

        var zoomCameraEnd = false;
        var couroutine = new CoroutineContainer(this, () => ZoomCamera(_startingFOV, _sequenceFOV), () => zoomCameraEnd = true);
        couroutine.Start();

        while (!finishedLookingAtCoffin || !zoomCameraEnd)
            yield return new WaitForEndOfFrame();

        yield return new WaitForEndOfFrame();
        mvmntController.FaceTarget(_coffinController.transform.position);
        yield return new WaitForEndOfFrame();
    }

    private IEnumerator ZoomCamera(float startFOV, float endFOV)
    {
        var duration = _zoomCameraTime;
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

    private IEnumerator MoveCameraToPlayer()
    {
        yield return MoveCameraToTarget(PlayerStats.Instance.transform);
        yield return ZoomCamera(_sequenceFOV, _startingFOV);

        Camera.main.GetComponent<FollowCam>().enabled = true;
    }

    private IEnumerator MoveCameraToCoffin()
    {
        yield return MoveCameraToTarget(UsefulTransforms.V_InCoffin);
    }

    private IEnumerator MoveCameraToTarget(Transform target)
    {
        var cameraTransform = Camera.main.transform;
        var followCamera = cameraTransform.GetComponent<FollowCam>();
        followCamera.enabled = false;

        var offset = followCamera.InitialOffset;

        var startingPosition = cameraTransform.position;
        var endingPosition = target.position + offset;

        yield return Slerp(cameraTransform, startingPosition, endingPosition, _moveCameraTime);
    }

    private IEnumerator FloatAboveCoffin()
    {
        _vampire.transform.SetPositionAndRotation(UsefulTransforms.V_InCoffin.position, UsefulTransforms.V_InCoffin.rotation);

        yield return new WaitForSeconds(Time.deltaTime);

        _vampire.gameObject.SetActive(true);

        yield return Slerp(_vampire.transform, UsefulTransforms.V_InCoffin, UsefulTransforms.V_FloatingAboveCoffin, _floatToAboveCoffinTime);
    }

    private IEnumerator FloatDownToNormalPosition()
    {
        var vampirePosition = _vampire.transform.position;
        var vampireRotation = _vampire.transform.rotation;

        yield return Slerp(_vampire.transform,
            vampirePosition, vampireRotation,
            UsefulTransforms.V_Default.position, UsefulTransforms.V_Default.rotation,
            _vampireToDefaultPositionTime);
    }    
}