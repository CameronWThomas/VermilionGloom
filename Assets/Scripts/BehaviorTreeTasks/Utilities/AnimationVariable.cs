using BehaviorDesigner.Runtime;
using System;

public enum AnimationVariable
{
    Conversing,
    Dead,
    Chocked,
    ChokeKill,
    Dragged
}

public static class AnimationVariableHelper
{
    public static string VariableName(this AnimationVariable variable)
    {
        return variable switch
        {
            AnimationVariable.Conversing => "conversing",
            AnimationVariable.Dead => "dead",
            AnimationVariable.Chocked => "choked",
            AnimationVariable.ChokeKill => "chokeKill",
            AnimationVariable.Dragged => "dragged",
            _ => throw new NotImplementedException()
        };
    }
}

public class SharedAnimationVariable : SharedVariable<AnimationVariable>
{
    public static implicit operator SharedAnimationVariable(AnimationVariable value) { return new SharedAnimationVariable { mValue = value }; }
}