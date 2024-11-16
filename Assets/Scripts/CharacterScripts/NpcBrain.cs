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

        if (dead)
            return;

        if(
            (mvmntLatchTarget != null && !dead)
            || 
            dragged
            )
        {
            transform.forward = mvmntLatchTarget.transform.forward;
            transform.position = mvmntLatchTarget.transform.position + mvmntLatchTarget.transform.forward;
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
            capsuleCollider.enabled = false;

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
    public void StrangleDie()
    {
        animator.SetTrigger("chokeKill");
        animator.SetBool("choked", false);
        Die();
    }

    public void Die()
    {
        dead = true;
        mvmntLatchTarget = null;
        
        //this should be handled in an animation.
        //cuz right now they die too quick
        animator.SetBool("dead", true);

        mvmntController.enabled = false;
        //other stuff for the dyin
        ReEvaluateTree();
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
