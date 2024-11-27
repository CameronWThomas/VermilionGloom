using System;
using UnityEngine;

[Serializable]
public class PlayerCharacterID : CharacterID
{
    public PlayerCharacterInfo CharacterInfo => InternalCharacterInfo as PlayerCharacterInfo;
}