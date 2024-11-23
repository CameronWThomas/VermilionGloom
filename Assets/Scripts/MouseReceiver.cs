using System;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class MouseReceiver : GlobalSingleInstanceMonoBehaviour<MouseReceiver>
{
    public MvmntController playerMvmnt;
    CamsPlayerController playerController;
    public CamsNpcBrain conversationTarget;
    public bool hostile = false;

    private int _deactivatedCounter = 0;
    public bool IsActivated => _deactivatedCounter == 0;


    public LayerMask clickLayerMask;
    protected override void Start()
    {
        base.Start();
        playerController = playerMvmnt.GetComponent<CamsPlayerController>();
    }

    private void Update()
    {
        if (!IsActivated)
            return;

        // TODO: use input actions instead
        // could also maybe use OnMouseDown?
        if (Input.GetMouseButtonDown(0))
        {
            MouseInteraction();


        }
    }

    public void Deactivate() => _deactivatedCounter++;
    public void Activate() => _deactivatedCounter = Mathf.Clamp(_deactivatedCounter - 1, 0, int.MaxValue);

    private void MouseInteraction()
    {
        //Debug.Log("Mouse Clicked");
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        // Debug draw the ray
        //Debug.DrawRay(ray.origin, ray.direction, Color.red);

        if (!Physics.Raycast(ray, out var hit, Mathf.Infinity, clickLayerMask))
            return;

        //Debug.Log("Mouse Hit:");
        //Debug.Log("     Transform Name: " + hit.transform.name);
        //Debug.Log("     Point: " + hit.point);

        // if this returns true, dont do anything else on this click
        if (ClickCancelActions(hit))
        {
            return;
        }

        if(hit.transform.CompareTag(GlobalConstants.WALKABLE_TAG_NAME))
        {
            if(playerController.strangleTarget != null)
            {
                NpcBrain brain = playerController.strangleTarget.GetComponent<NpcBrain>();
                if(brain != null)
                    brain.StopBeingStrangled();

                playerController.CancelStrangling();

            }
            playerMvmnt.GoToTarget(hit.point);
        }
        else if (hit.transform.CompareTag(GlobalConstants.INTERACTABLE_TAG_NAME))
        {
            HandleInteractableClick(hit);
        }
        else if(hit.transform.CompareTag(GlobalConstants.NPC_TAG_NAME))
        {
            HandleNpcClick(hit);
        }
    }

    private bool ClickCancelActions(RaycastHit hit)
    {
        // Exit any ongoing conversation if clicking elsewhere
        //TODO: reassess this when we implement an interaction screen for npcs
        if (hit.transform != null && conversationTarget != null)
        {
            if (hit.transform.name != conversationTarget.name)
            {
                conversationTarget.ExitConversation();
                conversationTarget = null;
            }
        }


        // End dragging if clicking on player or the drag target

        if(hit.transform.CompareTag(GlobalConstants.PLAYER_TAG_NAME) && playerController.dragging)
        {
            playerController.EndDragging();
            return true;
        }
        if (playerController.dragTarget != null)
        {
            if (hit.transform == playerController.dragTarget.transform)
            {
                playerController.EndDragging();
                return true;
            }
        }
        return false;
    }
    private void HandleInteractableClick(RaycastHit hit)
    {
        if (hit.transform.TryGetComponent<SecretPassage>(out var secretPassage))
            HandleInteractableClick(secretPassage);
    }

    private void HandleInteractableClick(SecretPassage secretPassage)
    {
        playerMvmnt.GoToTarget(secretPassage.DestinationPoint, () => secretPassage.UsePassage(playerMvmnt.transform));
    }

    private void HandleNpcClick(RaycastHit hit)
    {
        Debug.Log("Npc Clicked: " + hit.transform.name);
        
        CamsNpcBrain brain = hit.transform.GetComponent<CamsNpcBrain>();
        if (brain == null) {
            Debug.LogError("NpcBrain not found on " + hit.transform.name);
            return;
        }
        else
        {
            //Drag, strangle, or talk
            if (brain.dead)
            {
                //drag that mofo
                playerController.InitiateDragging(brain.gameObject);
            }
            else if (hostile)
            {
                playerController.InitiateStrangling(brain.gameObject);
            }
            else
            {

                brain.EnterConversation(playerMvmnt.transform);
                if (playerController.garlicRunTarget == null)
                {
                    playerMvmnt.GoToTarget(hit.point);
                }
                conversationTarget = brain;
            }
        }
    }
}
