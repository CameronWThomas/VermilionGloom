using System;
using System.Collections;
using UnityEngine;

public partial class PlayerController
{
    [Header("Strangle Stuff")]
    public NpcBrain StrangleTarget;
    [SerializeField] bool _isStrangling = false;
    [SerializeField] float _strangleDist = 1f;
    [SerializeField] float _strangleTime = 5f;

    public bool IsStrangling => _isStrangling;

    CoroutineContainer _strangleCoroutine;

    private SecretEvent _strangleSecretEvent = null;

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
        _isStrangling = true;
        StrangleTarget.BeStrangled(gameObject);

        _strangleSecretEvent = new SecretEvent(SecretEventType.StranglingSomeone, this.GetCharacterID(), StrangleTarget.GetCharacterID());
        NpcBehaviorBB.Instance.BroadcastSecretEvent(_strangleSecretEvent);

        var startStrangleTime = Time.time;
        while (Time.time - startStrangleTime <= _strangleTime)
        {
            yield return new WaitForSeconds(Time.deltaTime);
        }        
    }

    private void StrangleInterrupted()
    {
        StrangleTarget.StopBeingStrangled();
        _isStrangling = false;
        StrangleTarget = null;

        NpcBehaviorBB.Instance.EndSecretEventBroadcast(_strangleSecretEvent);
        _strangleSecretEvent = null;
    }

    private void StrangleSuccessful()
    {
        StrangleTarget.IsStrangled = true;
        StrangleTarget.Die();
        _isStrangling = false;
        StrangleTarget = null;

        NpcBehaviorBB.Instance.EndSecretEventBroadcast(_strangleSecretEvent);
        _strangleSecretEvent = null;
    }
}