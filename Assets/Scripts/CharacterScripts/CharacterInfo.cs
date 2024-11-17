using System;
using UnityEngine;

public enum CharacterType { Generic, VanHelsing, Owner }

public class CharacterInfo : MonoBehaviour
{
    public const int MAX_DETECTIVE_POINTS = 10;

    [SerializeField] private bool _isOwner = false;

    private bool _isInitialized = false;

    private void Start()
    {
        RemainingDetectivePoints = MAX_DETECTIVE_POINTS;
        CharacterInfoBB.Instance.Register(this);

        CharacterPortraitContentBB.Instance.Register(ID);
    }

    public void Initialize(bool isVanHelsing)
    {
        if (_isInitialized) return;

        _isInitialized = true;

        Name = NameHelper.GetRandomName();

        if (_isOwner)
            CharacterType = CharacterType.Owner;
        else if (isVanHelsing)
            CharacterType = CharacterType.VanHelsing;
        else
            CharacterType = CharacterType.Generic;
            
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


    public CharacterID ID { get; } = new CharacterID();
    public string Name { get; private set; }
    public CharacterType CharacterType { get; private set; } = CharacterType.Generic;
    public int RemainingDetectivePoints { get; private set; }
    public int PendingDetectivePoints { get; private set; }
    public bool IsOwner => _isOwner;
}