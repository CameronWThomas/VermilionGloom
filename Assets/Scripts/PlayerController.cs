using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public bool hostile = false;
    public bool sneaking = false;

    InputSystem_Actions inputActions;
    InputAction toggleCrouch;
    InputAction toggleHostile;
    InputAction toggleRun;

    MvmntController mvmntController;
    Animator animator;
    MouseReceiver mouseReceiver;
    NavMeshAgent agent;

    [Header("Strangle Stuff")]
    public GameObject strangleTarget;
    [SerializeField]
    float strangleDist = 1f;
    [SerializeField]
    bool strangling = false;
    [SerializeField]
    float strangleTime = 5f;
    [SerializeField]
    float strangleCounter = 0f;

    [Header("Drag Stuff")]
    public GameObject dragTarget;
    public bool dragging = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {

        inputActions = new InputSystem_Actions();
        mvmntController = GetComponent<MvmntController>();
        animator = GetComponent<Animator>();
        mouseReceiver = MouseReceiver.Instance;
        agent = GetComponent<NavMeshAgent>();
    }
    void Start()
    {
        // Subscribe to the crouch and toggleHostile events
        inputActions.Player.ToggleCrouch.performed += CrouchPerformed;
        inputActions.Player.ToggleHostile.performed += ToggleHostilePerformed;
        inputActions.Player.ToggleRun.performed += ToggleRunPerformed;
    }
    private void OnEnable()
    {
        // Enable input actions
        inputActions.Enable();
        toggleCrouch= inputActions.Player.ToggleCrouch;
        toggleCrouch.Enable();
        toggleHostile = inputActions.Player.ToggleHostile;
        toggleHostile.Enable();
        toggleRun = inputActions.Player.ToggleRun;
        toggleRun.Enable();
    }
    private void OnDisable()
    {
        // Disable input actions
        toggleCrouch.Disable();
        toggleHostile.Disable();
        toggleRun.Disable();
        inputActions.Disable();
    }
    // Update is called once per frame
    void Update()
    {
        StrangleUpdate();

        DraggingUpdate();
    }

    // Initiate dragging & stringling are called from the MouseReceiver
    public void InitiateDragging(GameObject target)
    {
        Debug.Log("Dragging initiated");
        dragTarget = target;
    }
    public void InitiateStrangling(GameObject target)
    {
        strangleTarget = target;
    }

    // End Dragging is called from the MouseReceiver when player or npc is clicked on again
    public void EndDragging()
    {
        dragging = false;
        animator.SetBool("dragging", false);

        NpcBrain targetBrain = dragTarget.GetComponent<NpcBrain>();
        if (targetBrain == null)
        {
            return;
        }
        targetBrain.StopBeingDragged();
        mvmntController.SetDragging(false);
        dragTarget = null;
    }

    // NOT IMPLEMENTED
    // StrangleInterupt would either be called by the MouseReceiver
    // or when the player is hit by an NPC mid strangle
    private void StrangleInterupt()
    {
        //todo
        NpcBrain targetBrain = strangleTarget.GetComponent<NpcBrain>();
        if (targetBrain != null)
        {
            targetBrain.StopBeingStrangled();
        }
        animator.SetBool("choking", false);
        strangleTarget = null;
        strangling = false;
    }

    // DraggingUpdate and StrangleUpdate are called from the Update function
    private void DraggingUpdate()
    {
        if (dragTarget != null && !dragging)
        {
            mvmntController.GoToTarget(dragTarget.transform.position);
            if (mvmntController.distanceToTarget <= strangleDist)
            {
                DragSetup();
            }
        }
    }
    private void StrangleUpdate()
    {
        if (strangleTarget != null && !strangling)
        {
            mvmntController.GoToTarget(strangleTarget.transform.position);
            if (mvmntController.distanceToTarget <= strangleDist)
            {
                StrangleSetup();
            }
        }
        if (strangling)
        {
            //choking begins
            if (strangleCounter >= strangleTime)
            {
                StrangleKill();
            }

            strangleCounter += Time.deltaTime;

        }
    }
    // Drag & Strangle Setup functions are called from the Update functions
    //
    //      They are seperate from initate because they are first primed by clicking on the NPC,
    //      then the player must move into position before the setup may occur
    //      
    //      PS: seeing this now, im unsure if the player clicking elsewhere would cancel this. Might be worth testing
    private void DragSetup()
    {
        Debug.Log("Started the dragging process");
        if (dragTarget == null)
        {
            return;
        }
        else
        {
            NpcBrain targetBrain = dragTarget.GetComponent<NpcBrain>();
            if (targetBrain == null)
            {
                return;
            }
            //strangleCounter = 0f;
            dragging = true;
            animator.SetBool("dragging", true);
            targetBrain.BeDraged(gameObject);
            mvmntController.GoToTarget(transform.position);
            mvmntController.SetDragging(true);
            Broadcast(BroadcastType.Drag, dragTarget);

        }
    }
    private void StrangleSetup()
    {
        Debug.Log("Started the strangling process");
        if(strangleTarget == null)
        {
            return;
        }
        else
        {
            NpcBrain targetBrain = strangleTarget.GetComponent<NpcBrain>();
            if(targetBrain == null)
            {
                return;
            }
            strangleCounter = 0f;
            strangling = true;
            animator.SetBool("choking", true);
            targetBrain.BeStrangled(gameObject);
            mvmntController.GoToTarget(transform.position);
            Broadcast(BroadcastType.Strangle, strangleTarget);
        }
    }

    // Strangle kill is called when the strangleCounter reaches the strangleTime
    private void StrangleKill()
    {
        //todo
        NpcBrain targetBrain = strangleTarget.GetComponent<NpcBrain>();
        if (targetBrain != null)
        {
            targetBrain.StrangleDie();
        }
        animator.SetTrigger("chokeKill");
        animator.SetBool("choking", false);
        strangleTarget = null;
        strangling = false;
    }
    public void CancelStrangling()
    {
        animator.SetBool("choking", false);
        strangleTarget = null;
        strangling = false;
        strangleCounter = 0f;
    }


    // Event handler for the crouch action
    private void CrouchPerformed(InputAction.CallbackContext context)
    {
        // Implement crouch logic here
        sneaking = !sneaking;
        mvmntController.SetCrouching(sneaking);
        animator.SetBool("sneaking", sneaking);
    }

    // Event handler for the toggleHostile action
    private void ToggleHostilePerformed(InputAction.CallbackContext context)
    {
        // Implement toggleHostile logic here
        hostile = !hostile;
        mvmntController.SetCombat(hostile);
        animator.SetBool("combat", hostile);
        mouseReceiver.hostile = hostile;
    }
    private void ToggleRunPerformed(InputAction.CallbackContext context)
    {
        Debug.Log("PLAYER TOGGLE RUN");
        // Implement toggleRun logic here
        mvmntController.SetRunning(!mvmntController.IsRunning());
    }
    // Disable input actions when the script is destroyed
    public enum BroadcastType
    {
        Drag,
        Strangle
    }
    private void Broadcast(BroadcastType bcType, GameObject extraObj = null)
    {
        NpcBrain[] brains = GameObject.FindObjectsByType<NpcBrain>(sortMode: FindObjectsSortMode.None);
        foreach(NpcBrain brain in brains)
        {
            brain.ReceiveBroadcast(bcType, gameObject, extraObj);
        }
    }
    public void Die()
    {
        animator.SetBool("dead", true);
        mvmntController.GoToTarget(transform.position);
        mvmntController.enabled = false;
        if(strangleTarget != null)
        {
            NpcBrain targetBrain = strangleTarget.GetComponent<NpcBrain>();
            if(targetBrain != null)
            {
                targetBrain.StopBeingStrangled();
            }
        }
        if(dragTarget != null)
        {
            NpcBrain targetBrain = dragTarget.GetComponent<NpcBrain>();
            if(targetBrain != null)
            {
                targetBrain.StopBeingDragged();
            }
        }
        agent.enabled = false;
    }
    private void OnDestroy()
    {
        inputActions.Disable();
    }
}
