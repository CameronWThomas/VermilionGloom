using UnityEngine;
using static PlayerController;

public partial class NpcBrain
{
    //private Vector3 killPosOffset = new Vector3(0, 0, 0);
    
    public GameObject mvmntLatchTarget = null;


    [Header("Interpersonal")]
    public GameObject combatTarget;
    public float crunchDistance = 2.0f;

    // Update is called once per frame
    //void OtherUpdate()
    //{
    //    //if dead and not being dragged, do nothing
    //    if (dead && !dragged)
    //        return;

    //    //if being dragged or strangled update position
    //    if(
    //        (mvmntLatchTarget != null && !dead)
    //        || 
    //        dragged
    //        )
    //    {
    //        transform.forward = mvmntLatchTarget.transform.forward;
    //        Vector3 placementMod = mvmntLatchTarget.transform.forward;
    //        if(dragged)
    //        {
    //            placementMod = placementMod * -1;
    //        }
    //        transform.position = mvmntLatchTarget.transform.position + placementMod;
    //    }
    //}

    // STRANGLE
    public void BeStrangled(GameObject killer)
    {
        Strangler = killer;
    }

    public void StopBeingStrangled()
    {
        BeStrangled(null);
    }

    // DRAG
    public void BeDraged(GameObject dragger) 
    {
        Dragger = dragger;
    }

    public void StopBeingDragged()
    {
        BeDraged(null);
    }

    public void Die(bool setAnimParam = true)
    {
        StopBeingStrangled();
        GetComponent<CharacterInfo>().Die();
    }

    private void ParseCombatTarget(GameObject attacker, GameObject attacked)
    {
        if (attacker == null && attacked == null)
        {
            return;
        }
        if (attacker != null)
        {
            combatTarget = attacker;
        }
        if (attacked != null)
        {
            NpcBrain strBrain = attacked.GetComponent<NpcBrain>();

            attacker = strBrain.mvmntLatchTarget;
            if (attacker != null)
            {
                combatTarget = attacker;
            }
        }
    }
    public void SawCorpseDragging(GameObject attacker, GameObject corpse = null)
    {
        if (corpse != null)
            Debug.Log(gameObject.name + " saw " + attacker.name + " dragging " + corpse.name);
        else if (attacker == null)
            Debug.Log(gameObject.name + " saw " + corpse + " being dragged");
        else
            Debug.Log(gameObject.name + " saw someone dragging someone");
        ParseCombatTarget(attacker, corpse);
    }
    public void SawStrangling(GameObject attacker, GameObject strangled =null)
    {
        if(strangled != null && attacker.name != null)
            Debug.Log(gameObject.name + " saw " + attacker.name + " strangling " + strangled.name);
        else if( attacker == null)
            Debug.Log(gameObject.name + " saw " + strangled + " being strangled");
        else 
            Debug.Log(gameObject.name + " saw someone strangling someone");

        ParseCombatTarget(attacker, strangled);
    }
    public void SawCorpse(GameObject corpse)
    {
        if (corpse != null)
            Debug.Log(gameObject.name + " saw " + corpse.name + " dead");
        else
            Debug.Log(gameObject.name + " saw a dead body");
    }

    public void Crunch()
    {
        //TODO set once you can get crunched again
        //animator.SetTrigger("crunch");


        // a little tolerance for the player moving away further
        if(mvmntController.distanceToTarget <= crunchDistance + .3f)
        {
            if(combatTarget == null)
            {
                return;
            }
            // if player 
            PlayerController pc = combatTarget.GetComponent<PlayerController>();
            if(pc != null)
            {
                pc.Die();
                combatTarget = null;
                return;
            }    
            // if npc
            NpcBrain targetBrain = combatTarget.GetComponent<NpcBrain>();
            if(targetBrain != null)
            {
                targetBrain.Die(true);
                combatTarget = null;
                return;
            }
        }
    }
   
}
