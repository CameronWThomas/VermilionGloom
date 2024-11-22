using UnityEngine;
using UnityEngine.AI;

public class CharacterAnimator : MonoBehaviour
{
    protected MvmntController MvmntController => GetComponent<MvmntController>();
    protected Animator Animator => GetComponent<Animator>();
    protected NavMeshAgent Agent => GetComponent<NavMeshAgent>();
    protected CharacterInfo CharacterInfo => GetComponent<CharacterInfo>();

    protected virtual void Update()
    {
        Animator.SetBool("dead", CharacterInfo.IsDead);
        Animator.SetFloat("speedPercent", Agent.velocity.magnitude / MvmntController.runSpeed);
    }
}