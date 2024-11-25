using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class BodyDeliverySequence : SequenceBase
{
    private NpcBrain _npcHuman;

    protected override SequenceRunner GetSequenceRunner()
    {
        return new SequenceRunner()
            .AddWait(5f)

            // Brining body to vampire
            .StartAddingParallelSequenceRoutines()
            .AddRoutine(ZoomCameraStartSequence)
            .AddRoutine(() => PlayerToTargets(UsefulTransforms.P_PreAddressingVampire, UsefulTransforms.P_DragBodyToLocation))
            .EndAddParallelRoutines()
            .AddRoutine(StopDragging)
            
            .StartAddingParallelSequenceRoutines()
            .AddRoutine(() => MoveCameraToTarget(_npcHuman.transform, 1f))
            .AddRoutine(() => PlayerToTargets(UsefulTransforms.P_WatchSuck))
            .EndAddParallelRoutines()

            .AddRoutine(() => PlayerFaceTarget(_vampire.transform))
            .AddWait(2f)

            // Vampire goes to body and sucks it up
            .AddRoutine(() => Slerp(_vampire.transform, UsefulTransforms.V_InFrontOfCoffin, 3f))
            .AddWait(.25f)
            .AddRoutine(() => VampireToTargets(UsefulTransforms.P_AddressingVampire))
            .AddRoutine(StartSucking)
            .AddWait(3f)
            .AddRoutine(StopSucking)

            // Go back to coffin
            .StartAddingParallelSequenceRoutines()
            .AddRoutine(() => VampireToTargets(UsefulTransforms.V_InFrontOfCoffin))
            .AddRoutine(GetRidOfNPC)
            .EndAddParallelRoutines()
            .AddWait(.25f)
            .AddRoutine(() => VampireToDefaultPosition(3f))

            .StartAddingParallelSequenceRoutines()
            .AddRoutine(() => PlayerToTargets(UsefulTransforms.P_AddressingVampire))
            .EndAddParallelRoutines()

            // Talking
            .AddWait(12f) // Remove when we have talking

            .AddRoutine(Bless)

            .AddRoutine(() => MoveCameraToPlayer(3f))
            .AddRoutine(ZoomCameraEndSequence)
            ;
    }    

    protected override bool SequencePlayingCondition()
    {
        var triggerVolume = GetComponent<TriggerVolume>();

        if (!GameState.Instance.VampireLordVisited ||
            !triggerVolume.IsPlayerPresent ||
            !triggerVolume.IsNpcBodyPresent)
            return false;

        return true;
    }

    protected override void OnSequenceStart()
    {
        base.OnSequenceStart();
        _npcHuman = PlayerController.DragTarget;
    }

    protected override void OnSequenceEnd()
    {
        base.OnSequenceEnd();
        GameState.Instance.BodyDeliverCount++;
    }

    private IEnumerator StopDragging()
    {
        PlayerController.StopDragging();

        _npcHuman.GetComponent<NavMeshAgent>().enabled = false;
        _npcHuman.GetComponent<MvmntController>().enabled = false;

        yield break;
    }
    
    private IEnumerator GetRidOfNPC()
    {
        

        yield return Slerp(_npcHuman.transform,
            _npcHuman.transform.position,
            _npcHuman.transform.position + Vector3.down * 5f,
            5f);

        _npcHuman = null;
    }
}