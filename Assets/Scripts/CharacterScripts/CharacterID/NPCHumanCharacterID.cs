using System;
using UnityEngine;

[Serializable]
public class NPCHumanCharacterID : CharacterID
{
    public NPCHumanCharacterInfo CharacterInfo => InternalCharacterInfo as NPCHumanCharacterInfo;

    public int PendingDetectivePoints => CharacterInfo.PendingDetectivePoints;
    public int CurrentDetectivePoints => CharacterInfo.RemainingDetectivePoints;
}