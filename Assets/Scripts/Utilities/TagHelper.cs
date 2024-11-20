using UnityEngine;

public static class TagHelper
{
    public static bool IsPlayer(this Transform transform) => transform.CompareTag(GlobalConstants.PLAYER_TAG_NAME);
    public static bool IsWalkable(this Transform transform) => transform.CompareTag(GlobalConstants.WALKABLE_TAG_NAME);
    public static bool IsInteractable(this Transform transform) => transform.CompareTag(GlobalConstants.INTERACTABLE_TAG_NAME);
    public static bool IsNpc(this Transform transform) => transform.CompareTag(GlobalConstants.NPC_TAG_NAME);
}