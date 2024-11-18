using UnityEngine;

public class PlayerStats : GlobalSingleInstanceMonoBehaviour<PlayerStats>
{
    [SerializeField] private int _maxVampirePoints = 10;

    public int CurrentVampirePoints { get; private set; }
    public int MaxVampirePoints => _maxVampirePoints;
    public int PendingUseVampirePoints { get; private set; }


    protected override void Start()
    {
        base.Start();
        CurrentVampirePoints = _maxVampirePoints;
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
        CurrentVampirePoints -= vampirePoints;
        return true;
    }
}
