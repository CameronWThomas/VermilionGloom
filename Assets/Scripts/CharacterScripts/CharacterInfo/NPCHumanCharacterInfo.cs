using System;
using UnityEngine;


public class NPCHumanCharacterInfo : CharacterInfo
{
    public const int MAX_DETECTIVE_POINTS = 10;

    private bool _isInitialized = false;
    private CharacterType _characterType = CharacterType.Generic;

    public override CharacterType CharacterType => _characterType;

    public NPCHumanCharacterID NPCHumanCharacterID => ID as NPCHumanCharacterID;
    public int RemainingDetectivePoints { get; private set; }
    public int PendingDetectivePoints { get; private set; }
    
    protected override void Start()
    {
        RemainingDetectivePoints = MAX_DETECTIVE_POINTS;

        base.Start();
    }

    public void Initialize(CharacterType characterType)
    {
        if (_isInitialized) return;

        _isInitialized = true;

        CreateName();

        _characterType = characterType;
    }

    public bool TryUseDetectivePoint(int points = 1)
    {
        if (points < 0 || RemainingDetectivePoints < points)
            return false;

        PendingDetectivePoints = 0;
        RemainingDetectivePoints -= points;
        return true;
    }

    public bool TrySetPendingDetectivePoints(int pendingPoints)
    {
        if (pendingPoints > RemainingDetectivePoints)
            return false;

        PendingDetectivePoints = pendingPoints;
        return true;
    }

    protected override CharacterID CreateCharacterID() => new NPCHumanCharacterID();
}