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

    private CoffinController _coffinController;

    private float _startingFOV;

    private UsefulTransforms UsefulTransforms => UsefulTransforms.Instance;

    protected override void Start()
    {
        base.Start();
        var gameState = GameState.Instance;
        
        _coffinController = gameState.CoffinController;

        //TODO sequence should look to this. Not other wayu around
        SetInitialState(gameState.VampireLordVisited);
    }

    protected override bool SequencePlayingCondition()
    {
        return GetComponent<TriggerVolume>().IsPlayerPresent;
    }

    protected override void OnInitializePlayable()
    {
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
            .AddWait(1f)

            .StartAddingParallelSequenceRoutines() // Talking. Eventually add some parallel routine for the talking
            .AddParallelRoutines(() => _coffinController.CloseCoffin()) 
            .AddParallelRoutines(12f)
            .EndAddParallelRoutines()

            .AddRoutine(VampireToDefaultPosition)
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
        yield return MoveCameraToTarget(PlayerStats.Instance.transform, _moveCameraTime);
        yield return ZoomCamera(_sequenceFOV, _startingFOV);

        Camera.main.GetComponent<FollowCam>().enabled = true;
    }

    private IEnumerator MoveCameraToCoffin()
    {
        yield return MoveCameraToTarget(UsefulTransforms.V_InCoffin, _moveCameraTime);
    }    

    private IEnumerator FloatAboveCoffin()
    {
        _vampire.transform.SetPositionAndRotation(UsefulTransforms.V_InCoffin.position, UsefulTransforms.V_InCoffin.rotation);

        yield return new WaitForSeconds(Time.deltaTime);

        _vampire.gameObject.SetActive(true);

        yield return Slerp(_vampire.transform, UsefulTransforms.V_InCoffin, UsefulTransforms.V_FloatingAboveCoffin, _floatToAboveCoffinTime);
    }

    private IEnumerator VampireToDefaultPosition()
    {
        yield return Slerp(_vampire.transform, UsefulTransforms.V_Default, _vampireToDefaultPositionTime);
    } 
}