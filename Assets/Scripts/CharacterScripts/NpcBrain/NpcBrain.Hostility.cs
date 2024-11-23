using System;
using System.Linq;
using UnityEngine;

public partial class NpcBrain
{
    [Header("Hostility")]
    [SerializeField] private bool _isHostile = false;
    [SerializeField] private bool _isCrunching = false;
    [SerializeField] private Transform _hostileTowardsTarget = null;
    [SerializeField] private float _crunchDistance = 2.0f;

    private Action<bool> _onCrunchEnd = null;

    public bool IsHostile => _isHostile;
    public bool IsCrunching => _isCrunching;
    public Transform HostileTowardsTarget => _hostileTowardsTarget;

    public void GetHostile()
    {
        _isHostile = true;
    }

    public void EndHostility()
    {
        _isHostile = false;
        _hostileTowardsTarget = null;
    }

    public bool SetHostileTowardsTarget()
    {
        if (SeesAHostileTowardsTarget(out var hostileTarget))
        {
            _hostileTowardsTarget = CharacterInfoBB.Instance.GetCharacterInfo(hostileTarget).transform;
            return true;
        }

        return false;
    }

    public bool SeesAHostileTowardsTarget(out CharacterID hostileTarget)
    {
        hostileTarget = null;

        if (!FindCharactersInSight(out var characters))
            return false;

        var relationShips = characters.Select(x => GetRelationship(x.ID));
        
        var hostileRelationShip = relationShips.FirstOrDefault(x => x.IsHostileTowards);
        if (hostileRelationShip == null)
            return false;

        hostileTarget = hostileRelationShip.RelationshipTarget;
        return true;
    }

    public void Crunch(Action<bool> onCrunchEnd)
    {
        if (_isCrunching)
        {
            onCrunchEnd?.Invoke(false);
            return;
        }

        //TODO should add timer to make sure we turn off crunching eventually if somethign goes wrong
        
        _onCrunchEnd = onCrunchEnd;
        _isCrunching = true;
    }

    public void OnCrunchEnd()
    {
        _isCrunching = false;

        if (_hostileTowardsTarget == null)
        {
            _onCrunchEnd?.Invoke(false);
            return;
        }

        var crunchTargetPosition = _hostileTowardsTarget.position;
        var ourPosition = transform.position;

        crunchTargetPosition.y = 0f;
        ourPosition.y = 0f;

        var crunched = Vector3.Distance(crunchTargetPosition, ourPosition) <= _crunchDistance + .3f;
        _onCrunchEnd?.Invoke(crunched);
    }
}