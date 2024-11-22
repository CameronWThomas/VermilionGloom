using UnityEngine;

public class PlayerCharacterInfo : CharacterInfo
{
    protected override void Start()
    {
        base.Start();
        CreateName();
    }

    public override CharacterType CharacterType => CharacterType.Player;
    protected override CharacterID CreateCharacterID() => new PlayerCharacterID();
}