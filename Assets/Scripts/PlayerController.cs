using UnityEngine;
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
    public void InitiateDragging(GameObject target)
    {
        Debug.Log("Dragging initiated");
        dragTarget = target;
    }
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
        dragTarget = null;
    }
    public void DraggingUpdate()
    {
        if (dragTarget != null && !dragging)
        {
            mvmntController.SetTarget(dragTarget.transform.position);
            if (mvmntController.distanceToTarget <= strangleDist)
            {
                DragSetup();
            }
        }
    }
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
            mvmntController.SetTarget(transform.position);
        }
    }

    public void InitiateStrangling(GameObject target)
    {
        strangleTarget = target;
    }
    private void StrangleUpdate()
    {
        if (strangleTarget != null && !strangling)
        {
            mvmntController.SetTarget(strangleTarget.transform.position);
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
            mvmntController.SetTarget(transform.position);
        }
    }
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
    private void OnDestroy()
    {
        inputActions.Disable();
    }
}
