using System;
using UnityEngine;
using UnityEngine.AI;

public class MvmntController : MonoBehaviour
{
    // Variables for movement speed and direction
    NavMeshAgent agent;
    Animator anim;
    public Vector3 targetPos;
    public float distanceToTarget;
    public float reachedTargetThreshold = 0.1f;

    [SerializeField]
    private bool dead;
    [SerializeField]
    private bool isRunning = false;
    [SerializeField]
    private bool isCrouching = false;
    [SerializeField]
    private bool isDragging = false;
    [SerializeField]
    private bool inCombat = false;

    private bool speedLimiter => isCrouching || isDragging;


    [Header("Configurations")]
    public float runSpeed = 3.5f;
    public float walkSpeed = 1.75f;

    [Header("Debug")]
    public bool debug = false;
    public Vector3 AttemptedTarget;
    public Vector3 agentVelocity;
    public float agentVelMagnitude;

    private Action _postDestinationArrivalAction = null;


    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        SetRunning(isRunning);
    }

    // Update is called once per frame
    void Update()
    {
        //handle whether agent should move
        distanceToTarget = Vector3.Distance(transform.position, targetPos + transform.position.y * Vector3.up);
        if(!IsAtDestination())
        {
            agent.isStopped = false;
        }
        else
        {
            agent.isStopped = true;

            var action = _postDestinationArrivalAction;
            _postDestinationArrivalAction = null;
            action?.Invoke();
        }

        
        // limit speed in selected states

        if (speedLimiter && anim.speed != walkSpeed)
        {
            agent.speed = walkSpeed;
        }
        else if(isRunning && anim.speed != runSpeed)
        {
            agent.speed = runSpeed;
        }

        //animator calls
        if (anim != null)
        {
            anim.SetFloat("speedPercent", agent.velocity.magnitude / runSpeed);
        }
    }

    public bool IsAtDestination()
    {
        return distanceToTarget <= reachedTargetThreshold;
    }
    public void SetTarget(Vector3 targetPos, Action postDestinationArrivalAction = null)
    {
        AttemptedTarget = targetPos;
        if (!CanReachTarget(targetPos))
        {
            Debug.Log("Cannot reach target");
            return;
        }
        Debug.Log("Setting target to: " + targetPos);

        _postDestinationArrivalAction = postDestinationArrivalAction;

        this.targetPos = targetPos;
        agent.SetDestination(targetPos);
        distanceToTarget = Vector3.Distance(transform.position, targetPos);
        agent.isStopped = false;
    }

    public bool CanReachTarget(Vector3 targetPos)
    {
        NavMeshPath path = new NavMeshPath();
        agent.CalculatePath(targetPos, path);
        return path.status == NavMeshPathStatus.PathComplete;
    }

    public bool IsRunning()
    {
        return isRunning;
    }
    public void SetRunning(bool run)
    {
        isRunning = run;
        agent.speed = isRunning ? runSpeed : walkSpeed;
    }

    public bool IsCrouching()
    {
        return isCrouching;
    }
    public void SetCrouching(bool crouch)
    {
        isCrouching = crouch;
    }
    public bool IsDragging()
    {
        return isDragging;
    }
    public void SetDragging(bool drag)
    {
        isDragging = drag;
    }
    public bool InCombat()
    {
        return inCombat;
    }
    public void SetCombat(bool combat)
    {
        inCombat = combat;
    }
    private void OnDrawGizmos()
    {
        if (debug)
        {
            // draw agent path
            if (agent != null)
            {
                if (agent.path != null)
                {
                    Vector3[] corners = agent.path.corners;
                    for (int i = 0; i < corners.Length - 1; i++)
                    {
                        Debug.DrawLine(corners[i], corners[i + 1], Color.red);
                    }
                }
            }

            // draw attempted target
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(AttemptedTarget, 0.1f);

        }

    }
}
