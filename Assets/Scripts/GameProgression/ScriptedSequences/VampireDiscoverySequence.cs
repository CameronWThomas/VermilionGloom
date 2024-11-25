using System.Collections;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
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
            .AddWait(5f) //TODO need a better way to detect that you are done with the secret passage. Maybe just don't trigger scene until thats done?

            .StartAddingParallelSequenceRoutines()
            .AddRoutine(() => PlayerToTargets(UsefulTransforms.P_PreAddressingVampire, UsefulTransforms.P_AddressingVampire))
            .AddRoutine(() => ZoomCamera(_startingFOV, _sequenceFOV, _zoomCameraTime))
            .EndAddParallelRoutines()

            .AddRoutine(() => PlayerFaceTarget(_coffinController.transform)) // Face coffin
            .AddRoutine(() => MoveCameraToTarget(UsefulTransforms.V_InCoffin, _moveCameraTime)) // Camera to coffin
            .AddRoutine(() => _coffinController.OpenCoffin())
            .AddWait(1f)
            .AddRoutine(FloatAboveCoffin)
            .AddWait(1f)

            .StartAddingParallelSequenceRoutines() // Talking. Eventually add some parallel routine for the talking
            .AddRoutine(() => _coffinController.CloseCoffin()) 
            .AddWait(12f) // Remove when we have talking
            .EndAddParallelRoutines()

            .AddRoutine(() => VampireToDefaultPosition(_vampireToDefaultPositionTime))
            .AddWait(1f)
            .AddRoutine(() => MoveCameraToPlayer(_moveCameraTime))
            .AddRoutine(() => ZoomCamera(_sequenceFOV, _startingFOV, _zoomCameraTime));
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

    private IEnumerator FloatAboveCoffin()
    {
        _vampire.transform.SetPositionAndRotation(UsefulTransforms.V_InCoffin.position, UsefulTransforms.V_InCoffin.rotation);

        yield return new WaitForSeconds(Time.deltaTime);

        _vampire.gameObject.SetActive(true);

        yield return Slerp(_vampire.transform, UsefulTransforms.V_InCoffin, UsefulTransforms.V_FloatingAboveCoffin, _floatToAboveCoffinTime);
    }    
}