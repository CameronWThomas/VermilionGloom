using BehaviorDesigner.Runtime;
using Unity.VisualScripting;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using static PlayerController;

[RequireComponent(typeof(CharacterInfo))]
public class NpcBrain : MonoBehaviour
{
    MvmntController mvmntController;
    Animator animator;
    BehaviorTree behaviorTree;
    CapsuleCollider capsuleCollider;
    NavMeshAgent navMeshAgent;
    Looker looker;

    public Transform convoTarget = null;
    //private Vector3 killPosOffset = new Vector3(0, 0, 0);

    public bool dead = false;
    public bool dragged = false;
    public bool strangled = false;

    public GameObject mvmntLatchTarget = null;


    [Header("Interpersonal")]
    public GameObject combatTarget;
    public float crunchDistance = 2.0f;

    public Room activeRoom;
    public Room[] allRooms; // all possible rooms, sorted by social score


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
        mvmntController = GetComponent<MvmntController>();
        behaviorTree = GetComponent<BehaviorTree>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        looker = GetComponentInChildren<Looker>();

        allRooms = GameObject.FindObjectsByType<Room>(FindObjectsSortMode.None);
        allRooms = allRooms.OrderBy(r => r.socialScore).Reverse().ToArray();

        // set active room to the room the npc is in
        foreach(Room room in allRooms)
        {
            if(room.PointIsInRoom(transform.position))
            {
                activeRoom = room;
                break;
            }
        }
        if(activeRoom == null)
        {
            activeRoom = allRooms[0];
        }



        // lazy solution (:
        if (!TryGetComponent<CharacterSecretKnowledge>(out _))
            transform.AddComponent<CharacterSecretKnowledge>();

    }

    // Update is called once per frame
    void Update()
    {
        //if dead and not being dragged, do nothing
        if (dead && !dragged)
            return;

        //if being dragged or strangled update position
        if(
            (mvmntLatchTarget != null && !dead)
            || 
            dragged
            )
        {
            transform.forward = mvmntLatchTarget.transform.forward;
            Vector3 placementMod = mvmntLatchTarget.transform.forward;
            if(dragged)
            {
                placementMod = placementMod * -1;
            }
            transform.position = mvmntLatchTarget.transform.position + placementMod;
        }
    }

    // CONVERSATIONS
    public void EnterConversation(Transform target)
    {
        animator.SetBool("conversing", true);
        convoTarget = target;
        ReEvaluateTree();

        UI_CharacterInteractionMenu.Instance.Activate(GetComponent<CharacterInfo>().ID);
    }
    public void ExitConversation()
    {
        convoTarget = null;
        animator.SetBool("conversing", false);
        ReEvaluateTree();
    }
    // MOVEMENT LATCHING
    private void SetMvmntLatchTarget(GameObject target, string animParam)
    {
        mvmntLatchTarget = target;
        if (target != null)
        {
            // TODO: instead of disabling movement stuff,
            // maybe handle latching in mvmnt controller. 

            mvmntController.enabled = false;
            navMeshAgent.enabled = false;

            if(!string.IsNullOrEmpty(animParam))
                animator.SetBool(animParam, true);

        }
        else
        {
            if (!string.IsNullOrEmpty(animParam))
                animator.SetBool(animParam, false);
            navMeshAgent.enabled = true;
            mvmntController.enabled = true;

        }
    }

    // STRANGLE
    public void BeStrangled(GameObject killer)
    {
        SetMvmntLatchTarget(killer, "choked");
        strangled = killer != null;
        ReEvaluateTree();
    }
    public void StopBeingStrangled()
    {
        BeStrangled(null);
    }
    public void StrangleDie()
    {
        animator.SetTrigger("chokeKill");
        animator.SetBool("choked", false);

        mvmntLatchTarget = null;
        strangled = false;
        Die(false);
    }

    // DRAG
    public void BeDraged(GameObject dragger) 
    {

        SetMvmntLatchTarget(dragger, "dragged");
        dragged = dragger != null;
        ReEvaluateTree();
    }
    public void StopBeingDragged()
    {
        BeDraged(null);
    }

    // DEATH
    //      setAnimParam can be called after death to trigger the death animation after a cool animation.
    //      see the DieOnExitAnim script, used in the choke kill animation
    public void Die(bool setAnimParam = true)
    {
        if (setAnimParam)
            animator.SetBool("dead", true);

        if (!dead)
        {
            dead = true;
            strangled = false;
            mvmntController.enabled = false;
            ReEvaluateTree();
        }

    }

    public void ReceiveBroadcast(BroadcastType type, GameObject shouldSee, GameObject extraObject )
    {
        if (
            (looker.CanSeeTarget(shouldSee) || looker.CanSeeTarget(extraObject))
            && !dead && !strangled)
        {

            switch (type)
            {
                case BroadcastType.Drag:
                    // Handle drag broadcast
                    //Debug.Log(gameObject.name + " saw " + shouldSee.name + " dragging someone");
                    SawCorpseDragging(shouldSee, extraObject);
                    break;
                case BroadcastType.Strangle:
                    // Handle strangle broadcast
                    SawStrangling(shouldSee, extraObject);
                    break;
                default:
                    // Handle other types of broadcasts
                    break;
            }
        }
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


        ReEvaluateTree();
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

        ReEvaluateTree();
    }
    public void SawCorpse(GameObject corpse)
    {
        if (corpse != null)
            Debug.Log(gameObject.name + " saw " + corpse.name + " dead");
        else
            Debug.Log(gameObject.name + " saw a dead body");


        ReEvaluateTree();
    }

    public void Crunch()
    {
        animator.SetTrigger("crunch");
        // a little tolerance for the player moving away further
        if(mvmntController.distanceToTarget <= crunchDistance + .3f)
        {
            if(combatTarget == null)
            {
                ReEvaluateTree();
                return;
            }
            // if player 
            PlayerController pc = combatTarget.GetComponent<PlayerController>();
            if(pc != null)
            {
                pc.Die();
                combatTarget = null;
                ReEvaluateTree();
                return;
            }    
            // if npc
            NpcBrain targetBrain = combatTarget.GetComponent<NpcBrain>();
            if(targetBrain != null)
            {
                targetBrain.Die(true);
                combatTarget = null;
                ReEvaluateTree();
                return;
            }
        }
    }
    // Triggers a re-calculation of current behaviour tree. 
    // Nice for when you expect some conditionals to change
    //      This could probably be written better, but it works.
    public void ReEvaluateTree()
    {
        behaviorTree.StopAllCoroutines();
        behaviorTree.StopAllTaskCoroutines();
        behaviorTree.ExternalBehavior = behaviorTree.ExternalBehavior;
        behaviorTree.enabled = false;
        behaviorTree.enabled = true;
        behaviorTree.Start();
    }
}
