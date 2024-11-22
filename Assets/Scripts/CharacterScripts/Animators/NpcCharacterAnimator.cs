using System;
using UnityEngine;

public class NpcCharacterAnimator : CharacterAnimator
{
    private bool _isStrangledTriggered = false;

    private NpcBrain Brain => GetComponent<NpcBrain>();

    protected override void Update()
    {
        base.Update();

        Animator.SetBool("conversing", Brain.IsInConversation);
        Animator.SetBool("dragged", Brain.IsBeingDragged);
        Animator.SetBool("dead", Brain.IsDead);
        Animator.SetBool("choked", Brain.IsBeingStrangled);
        Animator.SetBool("combat", Brain.IsHostile);

        if (Brain.IsStrangled && !_isStrangledTriggered)
        {
            _isStrangledTriggered = true;
            Animator.SetTrigger("chokeKill");
        }
    }
}