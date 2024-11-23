using UnityEngine;

public enum CharacterType { Generic, VanHelsing, Owner, Player }

public abstract class CharacterInfo : MonoBehaviour
{
    [SerializeField] private bool _isDead;
    [SerializeField] private string _name;
    [SerializeField] private int _maxHealth = 3;
    [SerializeField] private float _healthCooldownSec = 5;

    private CharacterID _id = null;

    private float? _lastDamageTime = null;
    private int _currentHealth;

    public CharacterID ID => _id ??= CreateCharacterID();    

    public virtual CharacterType CharacterType { get; }
    public string Name => _name;
    public bool IsDead => _isDead;



    protected virtual void Start()
    {
        _currentHealth = _maxHealth;
        CharacterInfoBB.Instance.Register(this);
        CharacterPortraitContentBB.Instance.Register(ID);
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

    public void Die()
    {
        _isDead = true;
        
        // will be updated next frame, but we need to force it now
        GetComponent<CharacterAnimator>().HasDied();
    }

    /// <summary>
    /// Damages the target. Will return whether they are dead
    /// </summary>
    public virtual bool Damage()
    {
        if (IsDead)
            return false;

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