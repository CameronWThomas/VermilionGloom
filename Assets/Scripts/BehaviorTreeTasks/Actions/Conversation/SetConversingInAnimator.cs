using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

[TaskCategory("Custom/Conversation")]
public class SetConversingInAnimator : Action
{
    public SharedBool Value;

    public override void OnStart()
    {
        var animator = GetComponent<Animator>();
        animator.SetBool("conversing", Value.Value);
    }
}