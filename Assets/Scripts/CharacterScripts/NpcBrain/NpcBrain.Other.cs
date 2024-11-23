using UnityEngine;
using static PlayerController;

public partial class NpcBrain
{
    
    public void SawCorpse(GameObject corpse)
    {
        if (corpse != null)
            Debug.Log(gameObject.name + " saw " + corpse.name + " dead");
        else
            Debug.Log(gameObject.name + " saw a dead body");
    }
}
