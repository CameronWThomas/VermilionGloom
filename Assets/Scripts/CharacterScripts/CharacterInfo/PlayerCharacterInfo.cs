using TMPro.EditorUtilities;
using UnityEngine;

public class PlayerCharacterInfo : CharacterInfo
{
    public override CharacterType CharacterType => CharacterType.Player;

    protected override CharacterID CreateCharacterID() => new PlayerCharacterID();    
}