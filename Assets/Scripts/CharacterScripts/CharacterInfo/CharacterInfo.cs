using UnityEngine;

public enum CharacterType { Generic, VanHelsing, Owner, Player }

public abstract class CharacterInfo : MonoBehaviour
{
    [SerializeField] private bool _isDead;
    [SerializeField] private string _name;

    private CharacterID _id = null;

    public CharacterID ID => _id ??= CreateCharacterID();    

    public virtual CharacterType CharacterType { get; }
    public string Name => _name;
    public bool IsDead => _isDead;



    protected virtual void Start()
    {
        CharacterInfoBB.Instance.Register(this);
        CharacterPortraitContentBB.Instance.Register(ID);
    }

    public void CreateName()
    {
        _name = NameHelper.GetRandomName();
    }

    public void Die()
    {
        _isDead = true;
    }

    protected abstract CharacterID CreateCharacterID();
}