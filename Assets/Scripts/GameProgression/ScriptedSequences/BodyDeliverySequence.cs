using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class BodyDeliverySequence : SequenceBase
{
    private NpcBrain _npcHuman;

    protected override bool GetIsPlayable()
    {
        return !GameState.Instance.GameWon && GameState.Instance.BodyDeliverCount + 1 < GameState.Instance.WinGameBodyCount;
    }

    protected override bool SequencePlayingCondition()
    {
        if (!BaseBodyDeliveryRequirements())
            return false;

        return GameState.Instance.BodyDeliverCount + 1 < GameState.Instance.WinGameBodyCount;
    }

    protected bool BaseBodyDeliveryRequirements()
    {
        var triggerVolume = GetComponent<TriggerVolume>();

        if (GameState.Instance.GameWon ||
            !GameState.Instance.VampireLordVisited ||
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

    protected override void PopulateSequenceRunner(SequenceRunner sequenceRunner)
    {
        AddUpToVampireWalkingBackToCoffin(sequenceRunner);

        sequenceRunner
            .AddRoutine(() => VampireToDefaultPosition(3f))

            .StartAddingParallelSequenceRoutines()
            .AddRoutine(() => PlayerToTargets(UsefulTransforms.P_AddressingVampire))
            .EndParallelRoutines()

            // Talking
            .AddWait(12f) // Remove when we have talking

            .AddRoutine(Bless)

            .AddRoutine(() => MoveCameraToPlayer(3f))
            .AddRoutine(ZoomCameraEndSequence)
            ;
    }

    protected void AddUpToVampireWalkingBackToCoffin(SequenceRunner sequenceRunner)
    {
        sequenceRunner
            .AddWait(5f)

            // Brining body to vampire
            .StartAddingParallelSequenceRoutines()
            .AddRoutine(ZoomCameraStartSequence)
            .AddRoutine(() => PlayerToTargets(UsefulTransforms.P_PreAddressingVampire, UsefulTransforms.P_DragBodyToLocation))
            .EndParallelRoutines()
            .AddRoutine(StopDragging)

            .StartAddingParallelSequenceRoutines()
            .AddRoutine(() => MoveCameraToTarget(_npcHuman.transform, 1f))
            .AddRoutine(() => PlayerToTargets(UsefulTransforms.P_WatchSuck))
            .EndParallelRoutines()

            .AddRoutine(() => PlayerFaceTarget(UsefulTransforms.P_AddressingVampire))
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
            .EndParallelRoutines()
            .AddWait(.25f);
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