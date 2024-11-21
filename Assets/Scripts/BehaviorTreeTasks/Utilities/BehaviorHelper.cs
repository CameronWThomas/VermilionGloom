using BehaviorDesigner.Runtime;

public static class BehaviorHelper
{
    public static SharedBool GetSharedBool(this Behavior behavior, string variableName) => behavior.GetVariable(variableName) as SharedBool;

    public static SharedTransform GetSharedTransform(this Behavior behavior, string variableName) => behavior.GetVariable(variableName) as SharedTransform;
}