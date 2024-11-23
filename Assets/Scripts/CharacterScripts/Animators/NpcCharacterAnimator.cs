using System;
using TMPro;
using UnityEngine;

public class NpcCharacterAnimator : CharacterAnimator
{
    private bool _isStrangledTriggered = false;
    private bool _crunchTriggered = false;

    private NpcBrain Brain => GetComponent<NpcBrain>();

    protected override void Update()
    {
        base.Update();

        Animator.SetBool("conversing", Brain.IsInConversation && !Brain.IsHostile); // Want to make sure conversing animation is canceled if we get hostile
        Animator.SetBool("dragged", Brain.IsBeingDragged);
        Animator.SetBool("dead", Brain.IsDead);
        Animator.SetBool("choked", Brain.IsBeingStrangled);
        Animator.SetBool("combat", Brain.IsHostile);

        if (Brain.IsCrunching)
        {
            if (!_crunchTriggered)
            {
                _crunchTriggered = true;
                Animator.SetTrigger("crunch");
            }
            else
                Animator.ResetTrigger("crunch");
        }
        else
            _crunchTriggered = false;

        if (Brain.IsStrangled && !_isStrangledTriggered)
        {
            _isStrangledTriggered = true;
            Animator.SetTrigger("chokeKill");
        }

        HandleResponseState();
    }

    private void HandleResponseState()
    {
        var responseState = Brain.SecretEventResponse;
        switch (responseState)
        {
            case SecretEventResponse.Good:
                Animator.SetTrigger("good");
                break;

            case SecretEventResponse.Bad:
                Animator.SetTrigger("bad");
                break;

            default:
                Animator.ResetTrigger("good");
                Animator.ResetTrigger("bad");
                break;
        }
        
    }
}