using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

[TaskCategory("Custom/Animation")]
public class TriggerAnimationAction : Action
{
    public SharedAnimationVariable AnimationVariable;
    protected Animator OurAnimator => transform.GetAnimator();

    public override TaskStatus OnUpdate()
    {
        transform.GetAnimator().SetTrigger(AnimationVariable.Value.VariableName());
        return TaskStatus.Success;
    }

    public override string FriendlyName
    {
        get => $"Trigger {AnimationVariable.Value.VariableName()} animation";
        set { }
    }
}