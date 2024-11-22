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
        Animator.SetBool("choking", PC.IsStrangling);
        Animator.SetBool("dragging", PC.IsDragging);

        HandleStrangleKillEvent();
    }

    private void HandleStrangleKillEvent()
    {
        if (PC.IsStrangling && _lastStrangleTarget == null)
        {
            _lastStrangleTarget = PC.StrangleTarget;
        }
        else if (!PC.IsStrangling && _lastStrangleTarget != null)
        {
            if (_lastStrangleTarget.IsStrangled)
                Animator.SetTrigger("chokeKill");
            _lastStrangleTarget = null;
        }
    }
}