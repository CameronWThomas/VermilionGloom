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
        if (dead && !dragged)
            return;

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
        //ReEvaluateTree();
    }

    public void BeStrangled(GameObject killer)
    {

        mvmntLatchTarget = killer;
        if(killer != null)
        {
            mvmntController.enabled = false;
            //capsuleCollider.enabled = false;

            //remove navmeshagent
            //Destroy(navMeshAgent);
            navMeshAgent.enabled = false;

            animator.SetBool("choked", true);

        }
        else
        {
            animator.SetBool("choked", false);
            navMeshAgent.enabled = true;
            //if (navMeshAgent == null)
            //{
            //    navMeshAgent = gameObject.AddComponent<NavMeshAgent>();
            //}
            //mvmntController.agent = navMeshAgent;
            mvmntController.enabled = true;

        }
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

    public void BeDraged(GameObject dragger) 
    { 
        mvmntLatchTarget = dragger;
        if (dragger != null)
        {
            mvmntController.enabled = false;

            //remove navmeshagent
            //Destroy(navMeshAgent);
            navMeshAgent.enabled = false;

            animator.SetBool("dragged", true);
            dragged = true;

        }
        else
        {
            animator.SetBool("dragged", false);
            navMeshAgent.enabled = true;
            //if (navMeshAgent == null)
            //{
            //    navMeshAgent = gameObject.AddComponent<NavMeshAgent>();
            //}
            //mvmntController.agent = navMeshAgent;
            mvmntController.enabled = true;
            dragged = false;

        }
        ReEvaluateTree();
    }
    public void StopBeingDragged()
    {
        BeDraged(null);
    }
    public void Die(bool setAnimParam = true)
    {
        if (setAnimParam)
            animator.SetBool("dead", true);

        if (!dead)
        {
            dead = true;

            //this should be handled in an animation.
            //cuz right now they die too quick
            mvmntController.enabled = false;
            //other stuff for the dyin
            ReEvaluateTree();
        }

    }


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
