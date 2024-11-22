using System;
using System.Collections;
using UnityEngine;

public partial class PlayerController
{
    [Header("Strangle Stuff")]
    public NpcBrain StrangleTarget;
    [SerializeField]
    float strangleDist = 1f;
    [SerializeField]
    float strangleTime = 5f;

    CoroutineContainer _strangleCoroutine;

    public void Strangle(NpcBrain target)
    {
        StrangleTarget = target;
        mvmntController.GoToTarget(StrangleTarget.transform, BeginStranglingTarget);
    }

    private void BeginStranglingTarget()
    {
        if (_strangleCoroutine != null)
        {
            if (!_strangleCoroutine.HasEnded)
                return;

            _strangleCoroutine = null;
        }

        _strangleCoroutine = new CoroutineContainer(this, StrangleCoroutine, StrangleSuccessful, StrangleInterrupted);
        _strangleCoroutine.Start();
    }    

    private IEnumerator StrangleCoroutine()
    {
        animator.SetBool("choking", true);
        StrangleTarget.BeStrangled(gameObject);

        Broadcast(BroadcastType.Strangle, StrangleTarget.gameObject);

        var startStrangleTime = Time.time;
        while (Time.time - startStrangleTime <= strangleTime)
        {
            yield return new WaitForSeconds(Time.deltaTime);
        }        
    }

    private void StrangleInterrupted()
    {
        StrangleTarget.StopBeingStrangled();
        animator.SetBool("choking", false);

        StrangleTarget = null;
    }

    private void StrangleSuccessful()
    {
        StrangleTarget.IsStrangled = true;

        animator.SetTrigger("chokeKill");
        animator.SetBool("choking", false);

        //_strangleTarget.GetComponent<CharacterInfo>().Die();

        StrangleTarget = null;
    }

    // StrangleInterupt would either be called by the MouseReceiver
    // or when the player is hit by an NPC mid strangle
    private void StrangleInterupt()
    {
        //todo
        NpcBrain targetBrain = StrangleTarget.GetComponent<NpcBrain>();
        if (targetBrain != null)
        {
            targetBrain.StopBeingStrangled();
        }
        animator.SetBool("choking", false);
        StrangleTarget = null;
    }
}