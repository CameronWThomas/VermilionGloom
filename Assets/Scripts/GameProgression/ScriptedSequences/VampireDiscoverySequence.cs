using System.Collections;
using UnityEngine;

public class VampireDiscoverySequence : SequenceBase
{
    //TODO some of these can be handled better by making animations if there is time

    [Header("Segment Speeds")]
    [SerializeField] protected float _moveCameraTime = 3f;
    [SerializeField] float _floatToAboveCoffinTime = 3f;
    [SerializeField] float _vampireToDefaultPositionTime = 3f;

    private CoffinController _coffinController;    

    protected override void Start()
    {
        base.Start();
        var gameState = GameState.Instance;
        
        _coffinController = gameState.CoffinController;
    }

    protected override bool GetIsPlayable() => !GameState.Instance.VampireLordVisited && !GameState.Instance.GameWon;

    protected override bool SequencePlayingCondition()
    {
        return GetComponent<TriggerVolume>().IsPlayerPresent;
    }

    protected override void OnInitializePlayable()
    {
        // Turn off the vampire
        GameState.Instance.Vampire.gameObject.SetActive(false);
    }

    protected override void PopulateSequenceRunner(SequenceRunner sequenceRunner)
    {
        sequenceRunner
            .AddWait(5f) //TODO need a better way to detect that you are done with the secret passage. Maybe just don't trigger scene until thats done?

            .StartAddingParallelSequenceRoutines()
            .AddRoutine(ZoomCameraStartSequence)
            .AddRoutine(() => PlayerToTargets(UsefulTransforms.P_PreAddressingVampire, UsefulTransforms.P_AddressingVampire))
            .EndParallelRoutines()

            .AddRoutine(() => PlayerFaceTarget(_coffinController.transform)) // Face coffin
            .AddRoutine(() => MoveCameraToTarget(UsefulTransforms.V_InCoffin, _moveCameraTime)) // Camera to coffin
            .AddRoutine(() => _coffinController.OpenCoffin())
            .AddWait(1f)
            .AddRoutine(FloatAboveCoffin)
            .AddWait(1f)

            .StartAddingParallelSequenceRoutines() // Talking. Eventually add some parallel routine for the talking
            .AddRoutine(() => _coffinController.CloseCoffin())
            .AddWait(1f) // Remove when we have talking
            .EndParallelRoutines()

            .AddRoutine(() => VampireToDefaultPosition(_vampireToDefaultPositionTime))
            .AddWait(1f)

            // Bless up
            .AddRoutine(Bless)

            .AddRoutine(() => MoveCameraToPlayer(_moveCameraTime))
            .AddRoutine(ZoomCameraEndSequence);
    }

    protected override void OnSequenceEnd()
    {
        base.OnSequenceEnd();
        GameState.Instance.VampireLordVisited = true;
        PlayerController pc = FindObjectOfType<PlayerController>();
        pc.vampTurned = true;
    }

    private IEnumerator FloatAboveCoffin()
    {
        _vampire.transform.SetPositionAndRotation(UsefulTransforms.V_InCoffin.position, UsefulTransforms.V_InCoffin.rotation);

        yield return new WaitForSeconds(Time.deltaTime);

        _vampire.gameObject.SetActive(true);

        yield return new WaitForSeconds(Time.deltaTime);

        _vampire.SetMaxVampyness();


        yield return Slerp(_vampire.transform, UsefulTransforms.V_InCoffin, UsefulTransforms.V_FloatingAboveCoffin, _floatToAboveCoffinTime);
    }    
}