using UnityEngine.AI;

public class PlayerCharacterInfo : CharacterInfo
{
    public override void Die()
    {
        base.Die();

        GetComponent<MvmntController>().enabled = false;
        GetComponent<NavMeshAgent>().enabled = false;
    }

    public override CharacterType CharacterType => CharacterType.Player;

    protected override CharacterID CreateCharacterID() => new PlayerCharacterID();
}