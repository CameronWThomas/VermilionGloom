using UnityEngine;
using static PlayerController;

public partial class NpcBrain
{
    

    public void SawCorpseDragging(GameObject attacker, GameObject corpse = null)
    {
        if (corpse != null)
            Debug.Log(gameObject.name + " saw " + attacker.name + " dragging " + corpse.name);
        else if (attacker == null)
            Debug.Log(gameObject.name + " saw " + corpse + " being dragged");
        else
            Debug.Log(gameObject.name + " saw someone dragging someone");
        //ParseCombatTarget(attacker, corpse);
    }
    
    public void SawCorpse(GameObject corpse)
    {
        if (corpse != null)
            Debug.Log(gameObject.name + " saw " + corpse.name + " dead");
        else
            Debug.Log(gameObject.name + " saw a dead body");
    }
}
