using UnityEngine;

public static class CharacterHelper
{
    public static bool GetCharacterID(this Component component, out CharacterID characterID)
    {
        characterID = component.GetCharacterID();
        return characterID != null;
    }

    public static NPCHumanCharacterID GetNPCHumanCharacterID(this Component component) => component.GetCharacterID() as NPCHumanCharacterID;
    public static CharacterID GetCharacterID(this Component component) => component.GetComponent<CharacterInfo>().ID;
}