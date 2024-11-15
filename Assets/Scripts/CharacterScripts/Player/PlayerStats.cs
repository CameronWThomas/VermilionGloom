using UnityEngine;

public class PlayerStats : GlobalSingleInstanceMonoBehaviour<PlayerStats>
{
    [SerializeField] private int _maxVampirePoints = 10;

    public int CurrentVampirePoints { get; private set; }
    public int MaxVampirePoints => _maxVampirePoints;


    protected override void Start()
    {
        base.Start();
        CurrentVampirePoints = _maxVampirePoints;
    }
}
