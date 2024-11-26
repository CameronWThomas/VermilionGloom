using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FinalBodyDeliverySequence : BodyDeliverySequence
{
    [Header("Final sequence Parameters")]
    [SerializeField] int _blessingRepeats = 3;
    [SerializeField] float _maxVibrateDistance = .2f;
    [SerializeField] List<SkinnedMeshRenderer> _playerVampireMeshes = new();

    float _blessTime = 0f;

    protected override bool GetIsPlayable()
    {
        return !GameState.Instance.GameWon;
    }

    protected override void PopulateSequenceRunner(SequenceRunner sequenceRunner)
    {
        AddUpToVampireWalkingBackToCoffin(sequenceRunner);
        _blessTime = _blessingRepeats * 4.125f;

        sequenceRunner
            .StartAddingParallelSequenceRoutines()
            .AddRoutine(() => PlayerToTargets(UsefulTransforms.P_AddressingVampire))
            .AddRoutine(() => VampireFaceTarget(UsefulTransforms.P_AddressingVampire))
            .EndParallelRoutines()

            .AddRoutine(() => PlayerFaceTarget(_vampire.transform))

            .AddRoutine(PlayFinalSong)

            .StartAddingParallelSequenceRoutines()
            .AddRoutine(Bless, maxDuration: _blessTime + 5f, repeat: _blessingRepeats)
            .AddRoutine(RampPlayerVampyness, maxDuration: _blessTime + 5f)
            .AddRoutine(VibratePlayer, maxDuration: _blessTime + 5f)
            .EndParallelRoutines()

            .AddRoutine(TurnPlayerIntoVampire)
            .AddWait(1f)

            .AddRoutine(() => MoveCameraToPlayer(3f))
            .AddRoutine(ZoomCameraEndSequence)
            ;
    }
    

    protected override bool SequencePlayingCondition()
    {
        if (!BaseBodyDeliveryRequirements())
            return false;

        return GameState.Instance.BodyDeliverCount + 1 == GameState.Instance.WinGameBodyCount;
    }

    protected override void OnSequenceEnd()
    {
        base.OnSequenceEnd();
        GameState.Instance.GameWon = true;
    }

    private IEnumerator VibratePlayer()
    {
        var playerStartingPosition = PlayerTransform.position;

        var startTime = Time.time;
        while (Time.time - startTime <= _blessTime)
        {
            var t = (Time.time - startTime) / _blessTime;

            var magnitude = Mathf.Lerp(0, _maxVibrateDistance, t);

            var randomDirection = Vector3.up * UnityEngine.Random.Range(0f, 1f) +
                Vector3.forward * UnityEngine.Random.Range(0f, 1f) +
                Vector3.right * UnityEngine.Random.Range(0f, 1f);

            PlayerTransform.position = playerStartingPosition + randomDirection.normalized * magnitude;
            yield return new WaitForNextFrameUnit();
        }

        yield return new WaitForNextFrameUnit();
        PlayerTransform.position = playerStartingPosition;
        yield return new WaitForNextFrameUnit();
    }

    private IEnumerator RampPlayerVampyness()
    {
        var playerAnimator = PlayerTransform.GetComponent<Animator>();

        playerAnimator.SetFloat("vampyness", 0);
        yield return new WaitForNextFrameUnit();

        var startTime = Time.time;
        while (Time.time - startTime <= _blessTime)
        {
            var t = (Time.time - startTime) / _blessTime;

            var vampyness = Mathf.Lerp(0f, 1f, t);
            playerAnimator.SetFloat("vampyness", vampyness);

            yield return new WaitForNextFrameUnit();
        }

        yield return new WaitForNextFrameUnit();
        playerAnimator.SetFloat("vampyness", 1f);
        yield return new WaitForNextFrameUnit();
    }

    private IEnumerator TurnPlayerIntoVampire()
    {
        //TODO move somewhere else
        foreach (var meshRenderer in PlayerTransform.GetComponentsInChildren<SkinnedMeshRenderer>())
            meshRenderer.enabled = false;

        foreach (var meshRenderer in _playerVampireMeshes)
            meshRenderer.enabled = true;

        yield break;
    }

    private IEnumerator PlayFinalSong()
    {
        CreditController.Instance.PlayFinalSong();
        yield break;
    }
}