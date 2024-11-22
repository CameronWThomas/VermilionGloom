using UnityEngine;

public class PlayerCharacterAnimator : CharacterAnimator
{
    private PlayerController PC => GetComponent<PlayerController>();

    private NpcBrain _lastStrangleTarget = null;

    protected override void Update()
    {
        base.Update();

        Animator.SetBool("sneaking", PC.sneaking);
        Animator.SetBool("combat", PC.hostile);
        Animator.SetBool("dragging", PC.IsDragging);

        HandleChoking();
    }

    protected override void OnSyncOccured(SyncEventType syncEventType)
    {
        if (syncEventType is SyncEventType.Strangle)
            Animator.SetBool("choking", true);
    }

    private void HandleChoking()
    {
        if (!PC.IsStrangling)
        {
            if (_lastStrangleTarget != null && _lastStrangleTarget.IsStrangled)
                Animator.SetTrigger("chokeKill");

            _lastStrangleTarget = null;
            Animator.SetBool("choking", false);
            return;
        }

        if (_lastStrangleTarget == null)
        {
            _lastStrangleTarget = PC.StrangleTarget;
            WaitForAnimationSync(_lastStrangleTarget.GetComponent<CharacterAnimator>(), SyncEventType.Strangle);
        }
    }
}