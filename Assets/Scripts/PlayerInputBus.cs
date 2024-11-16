using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputBus : MonoBehaviour
{
    public bool hostile = false;
    public bool sneaking = false;

    InputSystem_Actions inputActions;
    InputAction toggleCrouch;
    InputAction toggleHostile;
    InputAction toggleRun;

    MvmntController mvmntController;
    Animator animator;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {

        inputActions = new InputSystem_Actions();
        mvmntController = GetComponent<MvmntController>();
        animator = GetComponent<Animator>();
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
