using UnityEngine;

public enum CharacterType { Generic, VanHelsing, Owner, Player }

public abstract class CharacterInfo : MonoBehaviour
{
    [SerializeField] private bool _isDead;
    [SerializeField] private string _name;
    [SerializeField] private int _maxHealth = 3;
    [SerializeField] private float _healthCooldownSec = 5;

    [SerializeReference] private CharacterID _id = null;

    private float? _lastDamageTime = null;
    private int _currentHealth;

    public CharacterID ID => _id ??= CreateCharacterID();    

    public virtual CharacterType CharacterType { get; }
    public string Name => _name;
    public bool IsDead => _isDead;

    VoiceBox voiceBox;


    protected virtual void Start()
    {
        voiceBox = GetComponent<VoiceBox>();
        _currentHealth = _maxHealth;
        CharacterInfoBB.Instance.Register(this);
    }

    protected virtual void Update()
    {
        if (!IsDead && _lastDamageTime.HasValue && Time.time - _lastDamageTime.Value >= _healthCooldownSec)
        {
            _currentHealth = _maxHealth;
            _lastDamageTime = null;
        }
    }

    public void CreateName()
    {
        _name = NameHelper.GetRandomName();
    }

    /// <summary>
    /// Sound vampiry, but its actually for loading checkpoints...
    /// </summary>
    public virtual void ReturnToLife() { }

    public virtual void Die()
    {
        _isDead = true;
        
        // will be updated next frame, but we need to force it now
        GetComponent<CharacterAnimator>().HasDied();
        GetComponent<MvmntController>().CancelMovementAction();
    }

    /// <summary>
    /// Damages the target. Will return whether they are dead
    /// </summary>
    public virtual bool Damage()
    {
        if (IsDead)
            return false;

        if (voiceBox != null)
        {
            voiceBox.PlayHurt();
        }

        _currentHealth--;
        if (_currentHealth <= 0)
        {
            Die();
            return true;
        }

        _lastDamageTime = Time.time;
        return false;
    }

    protected abstract CharacterID CreateCharacterID();
}