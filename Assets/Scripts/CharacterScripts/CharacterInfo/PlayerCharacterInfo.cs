using UnityEngine.AI;

public class PlayerCharacterInfo : CharacterInfo
{
    public override void Die()
    {
        if (GameState.Instance.GameWon)
            return;

        base.Die();

        GetComponent<MvmntController>().enabled = false;
        GetComponent<NavMeshAgent>().enabled = false;
    }

    public override bool Damage()
    {
        if (GameState.Instance.GameWon)
            return false;

        return base.Damage();
    }

    public override void ReturnToLife()
    {
        GetComponent<MvmntController>().enabled = true;
        GetComponent<NavMeshAgent>().enabled = true;
    }

    public override CharacterType CharacterType => CharacterType.Player;

    protected override CharacterID CreateCharacterID() => new PlayerCharacterID();
}