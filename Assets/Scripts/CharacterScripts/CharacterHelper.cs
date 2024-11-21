using UnityEngine;

public static class CharacterHelper
{
    public static bool GetCharacterID(this Component component, out CharacterID characterID)
    {
        characterID = component.GetCharacterID();
        return characterID != null;
    }

    public static CharacterID GetCharacterID(this Component component) => component.GetComponent<CharacterInfo>().ID;
}