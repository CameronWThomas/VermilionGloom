using System;
using System.Collections;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class MvmntController : MonoBehaviour
{
    [SerializeField, Range(0f, 2f)] private float _checkDestinationInterval = .25f;


    private IEnumerator _movementAction = null;

    // Variables for movement speed and direction
    NavMeshAgent agent;
    Animator anim;
    public float distanceToTarget;
    public float reachedTargetThreshold = 0.1f;

    
    [SerializeField, Range(0f, 5f), Tooltip("Radius to go to when approaching this")]
    private float _approachRadius = 2f;
    public float ApproachRadius => _approachRadius;

    [Header("States")]
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

    public bool IsAtDestination => _movementAction == null && agent.isStopped;


    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        SetRunning(isRunning);
    }

    // Update is called once per frame
    void Update()
    {        
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

    public void GoToTarget(Vector3 targetPos, Action postDestinationArrivalAction = null)
    {
        if (!CanReachTarget(targetPos))
            return;

        StartNewMovementAction(GoToTargetPositionCoroutine(targetPos, postDestinationArrivalAction));
    }

    public void GoToTarget(Transform otherTransform, Action postDestinationArrivalAction = null)
    {
        if (!otherTransform.TryGetComponent<MvmntController>(out var mvmntController))
        {
            GoToTarget(otherTransform.position, postDestinationArrivalAction);
            return;
        }

        if (!CanReachTarget(otherTransform.position))
            return;

        StartNewMovementAction(GoToMvmntControllerCoroutine(mvmntController, postDestinationArrivalAction));
    }

    public void CancelMovementAction()
    {
        agent.isStopped = true;
        agent.ResetPath();

        if (_movementAction != null)
        {
            StopCoroutine(_movementAction);
            _movementAction = null;
        }
    }

    private void StartNewMovementAction(IEnumerator newMovementAction)
    {
        if (_movementAction != null)
        {
            var oldMovementAction = _movementAction;
            _movementAction = null;
            StopCoroutine(oldMovementAction);
        }

        _movementAction = newMovementAction;
        StartCoroutine(_movementAction);
    }

    private IEnumerator GoToTargetPositionCoroutine(Vector3 targetPos, Action postDestinationArrivalAction)
    {
        AttemptedTarget = targetPos;
        targetPos.y = 0f;

        agent.SetDestination(targetPos);
        agent.isStopped = false;

        while (true)
        {
            if (IsAtTargetPosition(targetPos))
                break;

            yield return new WaitForSeconds(_checkDestinationInterval);
        }

        agent.isStopped = true;
        postDestinationArrivalAction?.Invoke();
    }

    private IEnumerator GoToMvmntControllerCoroutine(MvmntController other, Action postDestinationArrivalAction)
    {
        Vector3 getDestination(MvmntController other)
        {
            var othersPosition = other.transform.position;
            othersPosition.y = 0f;

            var path = new NavMeshPath();
            agent.CalculatePath(othersPosition, path);

            var penulitimateCorner = path.corners.Count() >= 2 
                ? path.corners.TakeLast(2).First() 
                : transform.position;
            penulitimateCorner.y = othersPosition.y;

            var othersToPenulitimateCorner = penulitimateCorner - othersPosition;

            var magnitude = Mathf.Min(othersToPenulitimateCorner.magnitude, other.ApproachRadius);
            var destination = othersPosition + othersToPenulitimateCorner.normalized * magnitude;

            return destination;
        }

        agent.isStopped = false;

        var destination = getDestination(other);
        agent.SetDestination(destination);
        AttemptedTarget = destination;

        var lastOtherPosition = other.transform.position;

        while (true)
        {
            if ((lastOtherPosition - other.transform.position).magnitude > .1f)
            {
                lastOtherPosition = other.transform.position;

                destination = getDestination(other);
                agent.SetDestination(destination);
                AttemptedTarget = destination;
            }

            if (IsAtTargetPosition(destination))
                break;

            yield return new WaitForSeconds(_checkDestinationInterval);
        }

        agent.isStopped = true;
        postDestinationArrivalAction?.Invoke();
    }

    private bool IsAtTargetPosition(Vector3 targetPosition)
    {
        targetPosition.y = transform.position.y;
        var distanceToTarget = Vector3.Distance(transform.position, targetPosition);
        return distanceToTarget <= reachedTargetThreshold;
    }

    public bool CanReachTarget(Vector3 targetPos)
    {
        if (agent == null || !agent.enabled)
            return false;

        var path = new NavMeshPath();
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

    public void FaceTarget(Vector3 target)
    {
        Vector3 lookPos = target - transform.position;
        lookPos.y = 0;
        Quaternion rotation = Quaternion.LookRotation(lookPos);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, 1.5f * Time.deltaTime);

    }
    private void OnDrawGizmos()
    {
        if (!debug)
            return;


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


        // Draw _approachDistance
        Handles.color = Color.magenta;
        var center = transform.position;
        center.y = 0f;
        Handles.DrawWireArc(center, Vector3.up, Vector3.right, 360f, _approachRadius);
    }
}
