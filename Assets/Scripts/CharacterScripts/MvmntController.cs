using System;
using System.Collections;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class MvmntController : MonoBehaviour
{
    [SerializeField, Range(0f, 2f)] private float _checkDestinationInterval = .25f;

    // Variables for movement speed and direction
    NavMeshAgent agent;
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

    private CoroutineContainer _movementAction = null;

    public bool IsAtDestination => _movementAction == null && agent.isStopped;


    private Action<bool> _onMovementEndResult;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        SetRunning(isRunning);
    }

    // Update is called once per frame
    void Update()
    {
        SetupLatchTarget();

        // limit speed in selected states

        if (speedLimiter)
        {
            agent.speed = walkSpeed;
        }
        else if(isRunning)
        {
            agent.speed = runSpeed;
        }
    }    

    public void GoToTarget(Vector3 targetPos, Action onSuccess = null, Action onFailure = null)
    {
        if (!CanReachTarget(targetPos))
            return;

        var coroutine = new CoroutineContainer(this, () => GoToTargetPositionCoroutine(targetPos), onSuccess, onFailure);
        StartNewMovementAction(coroutine);
    }

    public void GoToTarget(Transform otherTransform, Action onSuccess = null, Action onFailure = null)
    {
        if (!otherTransform.TryGetComponent<MvmntController>(out var mvmntController))
        {
            GoToTarget(otherTransform.position, onSuccess, onFailure);
            return;
        }

        if (!CanReachTarget(otherTransform.position))
            return;

        var coroutine = new CoroutineContainer(this, () => GoToMvmntControllerCoroutine(mvmntController), onSuccess, onFailure);
        StartNewMovementAction(coroutine);
    }

    public void CancelMovementAction()
    {
        CancelMovementActionCoroutine();

        if (agent.isActiveAndEnabled)
        {
            agent.isStopped = true;
            agent.ResetPath();
        }
    }

    private void SetupLatchTarget()
    {
        // So far, only npc can be latched
        if (!TryGetComponent<NpcBrain>(out var brain) || (!brain.IsBeingDragged && !brain.IsBeingStrangled))
        {
            agent.enabled = true;
            return;
        }

        agent.enabled = false;
        CancelMovementActionCoroutine();

        var latchTarget = brain.IsBeingDragged ? brain.Dragger.transform : brain.Strangler.transform;
        var modifier =  brain.IsBeingDragged ? -latchTarget.forward : latchTarget.forward;

        transform.forward = latchTarget.transform.forward;
        transform.position = latchTarget.transform.position + modifier;
    }

    private void StartNewMovementAction(CoroutineContainer newMovementAction)
    {
        CancelMovementActionCoroutine();

        _movementAction = newMovementAction;
        _movementAction.Start();
    }

    private void CancelMovementActionCoroutine()
    {
        if (_movementAction == null)
            return;

        if (!_movementAction.HasEnded)
            _movementAction.Stop();

        _movementAction = null;
    }

    private IEnumerator GoToTargetPositionCoroutine(Vector3 targetPos)
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
        agent.ResetPath();
    }

    private IEnumerator GoToMvmntControllerCoroutine(MvmntController other)
    {
        Vector3 getDestination(MvmntController other)
        {
            var otherPosition = other.transform.position;
            otherPosition.y = 0f;

            return otherPosition;
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

            if (IsAtTargetPosition(destination, other.ApproachRadius))
                break;

            yield return new WaitForSeconds(_checkDestinationInterval);
        }

        agent.isStopped = true;
        agent.ResetPath();
    }

    private bool IsAtTargetPosition(Vector3 targetPosition, float buffer = 0f)
    {
        targetPosition.y = transform.position.y;
        var distanceToTarget = Vector3.Distance(transform.position, targetPosition);
        return distanceToTarget <= reachedTargetThreshold + buffer;
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
        
        // TODO setup as coroutine later and use this
        //transform.rotation = Quaternion.Slerp(transform.rotation, rotation, 1.5f * Time.deltaTime);
        transform.rotation = rotation;

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
