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
    }

    public bool TryPlayAnimationForReponseType(NpcBrain.SecretEventResponseType responseType, out float animationDuration)
    {
        animationDuration = 2f;

        switch (responseType)
        {
            case NpcBrain.SecretEventResponseType.Good:
                Animator.SetTrigger("good");
                return true;

            case NpcBrain.SecretEventResponseType.Bad:
                Animator.SetTrigger("bad");
                return true;

            default:
                Animator.ResetTrigger("good");
                Animator.ResetTrigger("bad");
                return false;
        }
        
    }
}