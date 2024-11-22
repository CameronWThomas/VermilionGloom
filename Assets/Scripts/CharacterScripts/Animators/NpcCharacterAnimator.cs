using System;
using UnityEngine;

public class NpcCharacterAnimator : CharacterAnimator
{
    private bool _isStrangledTriggered = false;
    private GameObject _lastStrangler = null;

    private NpcBrain Brain => GetComponent<NpcBrain>();

    protected override void Update()
    {
        base.Update();
        Animator.SetBool("conversing", Brain.IsInConversation);
        Animator.SetBool("dragged", Brain.IsBeingDragged);
        Animator.SetBool("dead", Brain.IsDead);

        HandleChoked();

        if (Brain.IsStrangled && !_isStrangledTriggered)
        {
            _isStrangledTriggered = true;
            Animator.SetTrigger("chokeKill");
        }
    }

    protected override void OnSyncOccured(SyncEventType syncEventType)
    {
        if (syncEventType is SyncEventType.Strangle)
            Animator.SetBool("choked", true);
    }

    private void HandleChoked()
    {
        if (!Brain.IsBeingStrangled)
        {
            _lastStrangler = null;
            Animator.SetBool("choked", false);
            return;
        }

        if (_lastStrangler == null)
        {
            _lastStrangler = Brain.Strangler;
            WaitForAnimationSync(_lastStrangler.GetComponent<CharacterAnimator>(), SyncEventType.Strangle);
        }
    }
}