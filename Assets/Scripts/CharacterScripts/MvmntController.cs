using System;
using UnityEngine;
using UnityEngine.AI;

public class MvmntController : MonoBehaviour
{
    // Variables for movement speed and direction
    NavMeshAgent agent;
    public Vector3 targetPos;
    public float distanceToTarget;
    public float reachedTargetThreshold = 0.1f;


    [Header("Debug")]
    public bool debug = false;
    public Vector3 AttemptedTarget;

    private Action _postDestinationArrivalAction = null;


    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
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
