using System;
using UnityEngine;

public class PlayerCharacterID : CharacterID
{
    public PlayerCharacterInfo CharacterInfo => InternalCharacterInfo as PlayerCharacterInfo;
}