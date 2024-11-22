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
    private void HandleChoking()
    {
        Animator.SetBool("choking", PC.IsStrangling);

        if (!PC.IsStrangling)
        {
            if (_lastStrangleTarget != null && _lastStrangleTarget.IsStrangled)
                Animator.SetTrigger("chokeKill");

            _lastStrangleTarget = null;
        }
        else if (_lastStrangleTarget == null)
        {
            _lastStrangleTarget = PC.StrangleTarget;
        }
    }
}