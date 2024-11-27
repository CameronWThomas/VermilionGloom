using UnityEngine;

public class PlayerStats : GlobalSingleInstanceMonoBehaviour<PlayerStats>
{
    [SerializeField] private int _maxVampirePoints = 10;
    [SerializeField] private int _currentVampirePoints;

    public int CurrentVampirePoints => _currentVampirePoints;
    public int MaxVampirePoints => _maxVampirePoints;
    public int PendingUseVampirePoints { get; private set; }


    protected override void Start()
    {
        base.Start();
        _currentVampirePoints = _maxVampirePoints;
    }

    public bool TrySetPendingVampirePoints(int pendingUseVampirePoints)
    {
        if (pendingUseVampirePoints > CurrentVampirePoints)
            return false;

        PendingUseVampirePoints = pendingUseVampirePoints;
        return true;
    }

    public bool TryUseVampirePoints(int vampirePoints)
    {
        if (CurrentVampirePoints < vampirePoints)
            return false;

        PendingUseVampirePoints = 0;
        _currentVampirePoints -= vampirePoints;
        return true;
    }
}
