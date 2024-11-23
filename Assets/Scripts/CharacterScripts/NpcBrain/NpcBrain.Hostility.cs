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
    [SerializeField] float _crunchMaxDuration = 2f;

    float _crunchStartTime = 0f;

    private Action _onCrunchEnd = null;
    private Action _onCrunchInterrupted = null;

    public bool IsHostile => GetIsHostile();
    public bool IsCrunching => _isCrunching;
    public Transform HostileTowardsTarget => _hostileTowardsTarget;

    public void GetHostile(CharacterID hostileTarget = null)
    {
        if (!HasAnyHostileRelationships() && _isHostile)
        {
            EndHostility();
            return;
        }

        _isHostile = true;

        if (hostileTarget != null)
            _hostileTowardsTarget = CharacterInfoBB.Instance.GetCharacterInfo(hostileTarget).transform;
    }

    public void EndHostility()
    {
        _isHostile = false;
        _hostileTowardsTarget = null;
    }

    public bool HasAnyHostileRelationships()
    {
        return _relationships.Any(x => x.IsHostileTowards);
    }

    public bool SetHostileTowardsTarget()
    {
        if (SeesAHostileTowardsTarget(out var hostileTarget))
        {
            _hostileTowardsTarget = CharacterInfoBB.Instance.GetCharacterInfo(hostileTarget).transform;
            return true;
        }

        _hostileTowardsTarget = null;

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
        return !CharacterInfoBB.Instance.GetCharacterInfo(hostileTarget).IsDead;
    }

    public void Crunch(Action onCrunchEnd, Action onCrunchInterrupted)
    {
        if (_isCrunching)
        {
            onCrunchInterrupted?.Invoke();
            return;
        }

        _crunchStartTime = Time.time;
        _onCrunchEnd = onCrunchEnd;
        _onCrunchInterrupted = onCrunchInterrupted;
        _isCrunching = true;

        if (_hostileTowardsTarget != null)
        {
            // Let everyone whos sees you know that you are attacking someone
            var secretEvent = new SecretEvent(SecretEventType.AttackingSomeone, this.GetCharacterID(), _hostileTowardsTarget.GetCharacterID(), SecretNoticability.Sight, SecretDuration.Instant);
            NpcBehaviorBB.Instance.BroadcastSecretEvent(secretEvent);
        }
    }

    public void OnCrunchDamagePoint()
    {
        if (!IsSuccessfulCrunch())
            return;

        var hostileTargetID = _hostileTowardsTarget.GetCharacterID();
        if (CharacterInfoBB.Instance.GetCharacterInfo(hostileTargetID).Damage())
        {
            // Target was killed. Create a secret and broadcast to the room
            var murderSecret = CreatePersonalMurderSecret(hostileTargetID, out var wasExistingSecret);
            if (wasExistingSecret)
                murderSecret.UpdateJustificationOrAttempt(true, false);
            else
                GetComponent<CharacterSecretKnowledge>().AddSecret(murderSecret);

            GetRelationship(hostileTargetID).Reevaluate();

            var secretEvent = new SecretEvent(SecretEventType.KilledSomeone, ID, hostileTargetID, SecretNoticability.Room, SecretDuration.Instant);
            NpcBehaviorBB.Instance.BroadcastSecretEvent(secretEvent);
        }
    }

    public void OnCrunchEnd(bool isInterrupt)
    {
        if (!_isCrunching)
            return;

        _isCrunching = false;

        if (isInterrupt)
            _onCrunchInterrupted?.Invoke();
        else
            _onCrunchEnd?.Invoke();

        _onCrunchEnd = null;
        _onCrunchInterrupted = null;
    }

    private void HostilityUpdate()
    {
        if (!_isCrunching)
            return;

        if (Time.time - _crunchStartTime >= _crunchMaxDuration)
        {
            OnCrunchEnd(true);
        }
    }

    private bool IsSuccessfulCrunch()
    {
        if (_hostileTowardsTarget == null)
        {
            return false;
        }

        var crunchTargetPosition = _hostileTowardsTarget.position;
        var ourPosition = transform.position;

        crunchTargetPosition.y = 0f;
        ourPosition.y = 0f;

        var crunched = Vector3.Distance(crunchTargetPosition, ourPosition) <= _crunchDistance + .3f;
        return crunched;
    }

    private bool GetIsHostile()
    {
        if (!HasAnyHostileRelationships() && _isHostile)
        {
            EndHostility();
        }

        return _isHostile;
    }
}