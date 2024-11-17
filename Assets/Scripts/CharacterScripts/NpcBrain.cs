using BehaviorDesigner.Runtime;
using UnityEngine;
using UnityEngine.AI;

public class NpcBrain : MonoBehaviour
{
    MvmntController mvmntController;
    Animator animator;
    BehaviorTree behaviorTree;
    CapsuleCollider capsuleCollider;
    NavMeshAgent navMeshAgent;

    public Transform convoTarget = null;
    //private Vector3 killPosOffset = new Vector3(0, 0, 0);

    public bool dead = false;
    public bool dragged = false;

    public GameObject mvmntLatchTarget = null;
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
        mvmntController = GetComponent<MvmntController>();
        behaviorTree = GetComponent<BehaviorTree>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        navMeshAgent = GetComponent<NavMeshAgent>();
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
            mvmntController.enabled = false;
            ReEvaluateTree();
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
