using BehaviorDesigner.Runtime;
using UnityEngine;

public static class HumanNpcBehaviorHelper
{
    private const string IsInConversationName = "IsInConversation";
    private const string ConversationTargetName = "ConversationTarget";

    public static NPCHumanCharacterID GetCharacterId(this Behavior behavior) => behavior.GetComponent<NPCHumanCharacterInfo>().NPCHumanCharacterID;

    public static bool IsInConversation(this Behavior behavior) => behavior.GetSharedBool(IsInConversationName).Value;
    public static void IsInConversation(this Behavior behavior, bool value) => behavior.GetSharedBool(IsInConversationName).Value = value;

    public static Transform ConversationTarget(this Behavior behavior) => behavior.GetSharedTransform(ConversationTargetName).Value;
    public static void ConversationTarget(this Behavior behavior, Transform target) => behavior.GetSharedTransform(ConversationTargetName).Value = target;
}