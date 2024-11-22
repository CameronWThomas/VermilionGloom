using System.Linq;
using UnityEngine;

public partial class NpcBrain
{
    [Header("Hostility")]
    [SerializeField] private bool _isHostile = false;
    [SerializeField] private Transform _hostileTarget = null;

    public bool IsHostile => _isHostile;
    public Transform HostileTarget => _hostileTarget;

    public void GetHostile()
    {
        _isHostile = true;
    }

    public void EndHostility()
    {
        _isHostile = false;
        _hostileTarget = null;
    }

    public bool SeesHostileTowardsTarget()
    {
        if (!FindCharactersInSight(out var characters))
            return false;

        var relationShips = characters.Select(x => GetRelationship(x.ID));
        return relationShips.Any(x => x.IsHostileTowards);
    }
}