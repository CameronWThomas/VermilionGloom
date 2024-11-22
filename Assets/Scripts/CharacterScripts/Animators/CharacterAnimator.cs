using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class CharacterAnimator : MonoBehaviour
{
    protected static List<AnimationSyncWait> _syncList = new();    

    protected List<SyncEventType> _waitingSyncEvents = new List<SyncEventType>();

    protected MvmntController MvmntController => GetComponent<MvmntController>();
    protected Animator Animator => GetComponent<Animator>();
    protected NavMeshAgent Agent => GetComponent<NavMeshAgent>();
    protected CharacterInfo CharacterInfo => GetComponent<CharacterInfo>();

    protected virtual void Update()
    {
        Animator.SetBool("dead", CharacterInfo.IsDead);
        Animator.SetFloat("speedPercent", Agent.velocity.magnitude / MvmntController.runSpeed);
    }

    protected virtual void OnSyncOccured(SyncEventType syncEventType)
    {
    }

    protected void WaitForAnimationSync(CharacterAnimator otherAnimator, SyncEventType syncEvent)
    {
        if (_waitingSyncEvents.Contains(syncEvent))
            return;

        if (!TryFindAnimationSyncEvent(otherAnimator, this, syncEvent, out AnimationSyncWait syncWait))
        {
            syncWait = new AnimationSyncWait(this, otherAnimator, syncEvent);
            _syncList.Add(syncWait);
            syncWait.Start();
        }

        syncWait.AddOnSyncEvent(OnSync);

        _waitingSyncEvents.Add(syncEvent);
    }

    private bool TryFindAnimationSyncEvent(CharacterAnimator otherAnimator, CharacterAnimator characterAnimator, SyncEventType syncEvent, out AnimationSyncWait syncWait)
    {
        syncWait = _syncList.FirstOrDefault(x => x.IsApplicableEvent(otherAnimator, characterAnimator, syncEvent));
        return syncWait != null;
    }

    private void OnSync(AnimationSyncWait syncWait, SyncEventType syncEventType, bool prematureEnd)
    {
        if (_syncList.Contains(syncWait))
            _syncList.Remove(syncWait);

        if (!_waitingSyncEvents.Contains(syncEventType))
            return;
        
        _waitingSyncEvents.Remove(syncEventType);

        if (!prematureEnd)
            OnSyncOccured(syncEventType);
    }

    protected enum SyncEventType
    {
        Strangle,
    }

    protected class AnimationSyncWait
    {
        private List<Action<AnimationSyncWait, SyncEventType, bool>> _onSyncAction = new();
        private CoroutineContainer _syncCoroutine = null;

        public AnimationSyncWait(CharacterAnimator characterAnimator1, CharacterAnimator characterAnimator2, SyncEventType syncEvent)
        {
            CharacterAnimator1 = characterAnimator1;
            CharacterAnimator2 = characterAnimator2;
            SyncEvent = syncEvent;
        }

        public CharacterAnimator CharacterAnimator1 { get; }
        public CharacterAnimator CharacterAnimator2 { get; }
        public SyncEventType SyncEvent { get; }

        public bool IsApplicableEvent(CharacterAnimator characterAnimator1, CharacterAnimator characterAnimator2, SyncEventType syncEvent)
        {
            if (SyncEvent != syncEvent)
                return false;

            if (CharacterAnimator1 == characterAnimator1)
                return CharacterAnimator2 == characterAnimator2;

            if (CharacterAnimator2 == characterAnimator1)
                return CharacterAnimator1 == characterAnimator2;

            return false;
        }

        public void AddOnSyncEvent(Action<AnimationSyncWait, SyncEventType, bool> onSync)
        {
            _onSyncAction.Add(onSync);
        }

        public void Start()
        {
            if (_syncCoroutine != null)
                return;

            _syncCoroutine = new CoroutineContainer(CharacterAnimator1, WaitForSyncRoutine, OnSyncSuccess, OnSyncPrematureEnd);
            _syncCoroutine.Start();
        }        

        private IEnumerator WaitForSyncRoutine()
        {
            while (true)
            {
                if (CharacterAnimator1._waitingSyncEvents.Contains(SyncEvent) && CharacterAnimator2._waitingSyncEvents.Contains(SyncEvent))
                    break;

                yield return new WaitForEndOfFrame();
            }
        }

        private void OnSyncSuccess() => OnSync(false);

        private void OnSyncPrematureEnd() => OnSync(true);

        private void OnSync(bool premature)
        {
            foreach (var syncEvent in _onSyncAction)
                syncEvent?.Invoke(this, SyncEvent, premature);
        }
    }
}