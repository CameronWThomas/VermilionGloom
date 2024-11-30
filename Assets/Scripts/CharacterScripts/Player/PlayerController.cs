using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

public partial class PlayerController : MonoBehaviour
{
    public bool hostile = false;
    public bool sneaking = false;
    [SerializeField] bool _cutsceneRunning = false;

    InputSystem_Actions inputActions;
    InputAction toggleCrouch;
    InputAction toggleHostile;
    InputAction toggleRun;

    MvmntController mvmntController;
    MouseReceiver mouseReceiver;
    NavMeshAgent agent;


    [Header("VampShit")]
    // sun
    public float dieInSunTime = 5f;
    public float dieInSunCounter = 0f;
    public bool inSun = false;
    public List<SkinnedMeshRenderer> lit_vertex_shaders;
    public string BURNABLE_SHADER = "Shader Graphs/Lit_VertexShader";
    // garlic
    public GameObject garlic = null;
    public Vector3? garlicRunTarget = null;
    private float garlicRunPosRadius = 5f;
    private bool preGarlicRun = false;

    public bool IsRunning => mvmntController.IsRunning();

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

        //get burnable shaders
        SkinnedMeshRenderer[] children = transform.GetComponentsInChildren<SkinnedMeshRenderer>();
        foreach (SkinnedMeshRenderer child in children)
        {
            string mat_name = child.material.shader.name;
            if (mat_name == BURNABLE_SHADER)
            {
                lit_vertex_shaders.Add(child);
            }
        }
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

    //TODO this is a quick way I could do this. Probably better to do what is done in OnEnable and OnDisable
    public void DisableInputForCutscene()
    {
        _cutsceneRunning = false;

        if (sneaking)
            CrouchPerformed(new InputAction.CallbackContext());
        if (hostile)
            ToggleHostilePerformed(new InputAction.CallbackContext());
        if (mvmntController.IsRunning())
            ToggleRunPerformed(new InputAction.CallbackContext());

        _cutsceneRunning = true;
    }

    public void Run(bool isRunning)
    {
        if (isRunning == mvmntController.IsRunning())
            return;

        ToggleRunPerformed(new InputAction.CallbackContext());
    }

    public void GetHostile(bool getHostile)
    {
        if (getHostile == hostile)
            return;

        ToggleHostilePerformed(new InputAction.CallbackContext());
    }

    public void RenableInputAfterCutscene()
    {
        _cutsceneRunning = false;
    }    


    // Event handler for the crouch action
    private void CrouchPerformed(InputAction.CallbackContext context)
    {
        if (_cutsceneRunning)
            return;

        // Implement crouch logic here
        sneaking = !sneaking;
        mvmntController.SetCrouching(sneaking);
    }

    // Event handler for the toggleHostile action
    private void ToggleHostilePerformed(InputAction.CallbackContext context)
    {
        if (_cutsceneRunning)
            return;

        // Implement toggleHostile logic here
        hostile = !hostile;
        mvmntController.SetCombat(hostile);
        //hostile = hostile;
    }
    private void ToggleRunPerformed(InputAction.CallbackContext context)
    {
        if (_cutsceneRunning)
            return;

        Debug.Log("PLAYER TOGGLE RUN");
        // Implement toggleRun logic here
        mvmntController.SetRunning(!mvmntController.IsRunning());
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

    //TODO: move this somwhere else
    public void EnteredRoom(Room room)
    {
        //mvmntController.EnteredRoom(room);
        RoomVisibilityManager.Instance.UpdateRoomVisibility(room);
    }
    private void OnDestroy()
    {
        inputActions.Disable();
    }

    private void Update()
    {
        GarlicStuff();
        SunStuff();
    }

    public void SetGarlic(GameObject garlic)
    {
        this.garlic = garlic;
        if (garlic == null)
        {
            garlicRunTarget = null;

            mvmntController.SetRunning(preGarlicRun);
        }
        else
        {
            preGarlicRun = mvmntController.IsRunning();
            mvmntController.SetRunning(true);
        }
    }
    private void GarlicStuff()
    {
        if (garlic != null)
        {
            if (garlicRunTarget != null)
            {
                mvmntController.GoToTarget((Vector3)garlicRunTarget);
                if (mvmntController.distanceToTarget <= 1.5f)
                {
                    SetRandomPositionAwayFromGarlic();
                }
            }
            else
            {
                //set target to somewhere other than garlic
                SetRandomPositionAwayFromGarlic();
            }
        }
    }
    private void SetRandomPositionAwayFromGarlic()
    {
        Vector3 offset = new Vector3(UnityEngine.Random.Range(-garlicRunPosRadius, garlicRunPosRadius), 0, UnityEngine.Random.Range(-garlicRunPosRadius, garlicRunPosRadius));
        Vector3 antiGarl = garlic.transform.position - transform.position;
        Vector3 randomPos = transform.position + antiGarl + offset;
        garlicRunTarget = randomPos;
    }


    private void SunStuff()
    {

        if (inSun)
        {
            if (dieInSunCounter >= dieInSunTime)
            {
                Die();
            }
            dieInSunCounter += Time.deltaTime;
        }
        else
        {
            if (dieInSunCounter > 0)
            {
                dieInSunCounter -= Time.deltaTime;
            }
        }
        dieInSunCounter = Mathf.Clamp(dieInSunCounter, 0, 500);
        SetBurnValOnShaders(dieInSunCounter * 10);
    }

    private void SetBurnValOnShaders(float burnVal)
    {
        foreach (SkinnedMeshRenderer mesh in lit_vertex_shaders)
        {
            //Debug.Log("Setting burn val on mesh: " + mesh.name);
            foreach (Material mat in mesh.materials)
            {
                //Debug.Log("Setting burn val: "+ burnVal+ ", on material: " + mat.name);
                mat.SetFloat("_BurnVal", burnVal);
            }
        }
    }


}
