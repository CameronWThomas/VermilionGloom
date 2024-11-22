using UnityEngine;

public enum CharacterType { Generic, VanHelsing, Owner, Player }

public abstract class CharacterInfo : MonoBehaviour
{
    [SerializeField] private bool _isDead;

    private CharacterID _id = null;

    public CharacterID ID => _id ??= CreateCharacterID();    

    public virtual CharacterType CharacterType { get; }
    public string Name { get; private set; }
    public bool IsDead => _isDead;



    protected virtual void Start()
    {
        CharacterInfoBB.Instance.Register(this);
        CharacterPortraitContentBB.Instance.Register(ID);
    }

    public void CreateName()
    {
        Name = NameHelper.GetRandomName();
    }

    public void Die()
    {
        _isDead = true;
    }

    protected abstract CharacterID CreateCharacterID();
}