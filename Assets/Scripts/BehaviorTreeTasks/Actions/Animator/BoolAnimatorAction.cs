using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

[TaskCategory("Custom/Animation")]
public class BoolAnimatorAction : Action
{
    public SharedBool Value;
    public SharedAnimationVariable AnimationVariable;
    protected Animator OurAnimator => transform.GetAnimator();

    public override TaskStatus OnUpdate()
    {
        transform.GetAnimator().SetBool(AnimationVariable.Value.VariableName(), Value.Value);
        return TaskStatus.Success;
    }

    public override string FriendlyName
    {
        get => $"Turn {AnimationVariable.Value.VariableName()} animation { (Value.Value ? "on" : "off") }";
        set { }
    }
}