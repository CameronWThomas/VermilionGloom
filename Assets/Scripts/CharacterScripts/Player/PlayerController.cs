using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public partial class PlayerController : MonoBehaviour
{
    public bool hostile = false;
    public bool sneaking = false;

    InputSystem_Actions inputActions;
    InputAction toggleCrouch;
    InputAction toggleHostile;
    InputAction toggleRun;

    MvmntController mvmntController;
    MouseReceiver mouseReceiver;
    NavMeshAgent agent;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        inputActions = new InputSystem_Actions();
        mvmntController = GetComponent<MvmntController>();
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

    // Event handler for the crouch action
    private void CrouchPerformed(InputAction.CallbackContext context)
    {
        // Implement crouch logic here
        sneaking = !sneaking;
        mvmntController.SetCrouching(sneaking);
    }

    // Event handler for the toggleHostile action
    private void ToggleHostilePerformed(InputAction.CallbackContext context)
    {
        // Implement toggleHostile logic here
        hostile = !hostile;
        mvmntController.SetCombat(hostile);
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
        GetComponent<CharacterInfo>().Die();

        mvmntController.GoToTarget(transform.position);
        mvmntController.enabled = false;
        if(StrangleTarget != null)
        {
            NpcBrain targetBrain = StrangleTarget.GetComponent<NpcBrain>();
            if(targetBrain != null)
            {
                targetBrain.StopBeingStrangled();
            }
        }
        if(DragTarget != null)
        {
            NpcBrain targetBrain = DragTarget.GetComponent<NpcBrain>();
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
