using System;
using System.Collections;
using UnityEngine;

public partial class PlayerController
{
    [Header("Strangle Stuff")]
    public NpcBrain StrangleTarget;
    [SerializeField] bool _isStrangling = false;
    [SerializeField] float _strangleTime = 5f;

    public bool IsStrangling => _isStrangling;

    CoroutineContainer _strangleCoroutine;

    private SecretEvent _strangleSecretEvent = null;

    public void Strangle(NpcBrain target)
    {
        StrangleTarget = target;
        mvmntController.GoToTarget(StrangleTarget.transform, BeginStranglingTarget);
    }

    public void OnEndStrangle()
    {
        MouseReceiver.Instance.Activate();
    }

    public void InterruptStrangling()
    {
        if (_strangleCoroutine != null)
        {
            _strangleCoroutine.Stop();
            _strangleCoroutine = null;
        }
    }

    private void BeginStranglingTarget()
    {
        if (_strangleCoroutine != null)
        {
            if (!_strangleCoroutine.HasEnded)
                return;

            _strangleCoroutine = null;
        }

        MouseReceiver.Instance.Deactivate();

        _strangleCoroutine = new CoroutineContainer(this, StrangleCoroutine, StrangleSuccessful, StrangleInterrupted);
        _strangleCoroutine.Start();
    }    

    private IEnumerator StrangleCoroutine()
    {
        _isStrangling = true;
        StrangleTarget.BeStrangled(gameObject);

        _strangleSecretEvent = new SecretEvent(SecretEventType.StranglingSomeone, this.GetCharacterID(), StrangleTarget.GetCharacterID(), SecretNoticability.Sight, SecretDuration.UntilCancel);
        NpcBehaviorBB.Instance.BroadcastSecretEvent(_strangleSecretEvent);

        var startStrangleTime = Time.time;
        while (Time.time - startStrangleTime <= _strangleTime)
        {
            if (GetComponent<CharacterInfo>().IsDead)
            {
                StrangleInterrupted();
            }

            yield return new WaitForSeconds(Time.deltaTime);
        }        
    }

    private void StrangleInterrupted()
    {
        MouseReceiver.Instance.Activate();

        if (_strangleCoroutine != null)
            _strangleCoroutine.Stop();
        _strangleCoroutine = null;

        if (StrangleTarget != null)
            StrangleTarget.StopBeingStrangled();

        _isStrangling = false;
        StrangleTarget = null;

        NpcBehaviorBB.Instance.EndSecretEventBroadcast(_strangleSecretEvent);
        _strangleSecretEvent = null;
    }

    private void StrangleSuccessful()
    {
        StrangleTarget.StrangleDie();
        _isStrangling = false;
        StrangleTarget = null;

        NpcBehaviorBB.Instance.EndSecretEventBroadcast(_strangleSecretEvent);
        _strangleSecretEvent = null;
    }
}