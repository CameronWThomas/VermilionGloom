using System;
using UnityEngine;

public class MouseReceiver : GlobalSingleInstanceMonoBehaviour<MouseReceiver>
{
    public MvmntController playerMvmnt;
    PlayerController playerController;
    public NpcBrain conversationTarget;
    public bool hostile = false;

    private int _deactivatedCounter = 0;
    public bool IsActivated => _deactivatedCounter == 0;

    //public LayerMask movementLayerMask;
    private void Start()
    {
        playerController = playerMvmnt.GetComponent<PlayerController>();
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

        if (!Physics.Raycast(ray, out var hit, Mathf.Infinity))
            return;

        //Debug.Log("Mouse Hit:");
        //Debug.Log("     Transform Name: " + hit.transform.name);
        //Debug.Log("     Point: " + hit.point);

        //TODO: reassess this when we implement an interaction screen for npcs
        if (hit.transform != null && conversationTarget != null) 
        {
            if (hit.transform.name != conversationTarget.name)
            {
                conversationTarget.ExitConversation();
                conversationTarget = null;
            }
        }

        if(hit.transform.CompareTag(GlobalConstants.WALKABLE_TAG_NAME))
        {
            playerMvmnt.SetTarget(hit.point);
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

    private void HandleInteractableClick(RaycastHit hit)
    {
        if (hit.transform.TryGetComponent<SecretPassage>(out var secretPassage))
            HandleInteractableClick(secretPassage);

    }

    private void HandleInteractableClick(SecretPassage secretPassage)
    {

        playerMvmnt.SetTarget(secretPassage.DestinationPoint, () => secretPassage.UsePassage(playerMvmnt.transform));
    }

    private void HandleNpcClick(RaycastHit hit)
    {
        Debug.Log("Npc Clicked: " + hit.transform.name);
        
        NpcBrain brain = hit.transform.GetComponent<NpcBrain>();
        if (brain == null) {
            Debug.LogError("NpcBrain not found on " + hit.transform.name);
            return;
        }
        else if (hostile)
        {
            //brain.BeStrangled(playerMvmnt.gameObject);
            playerController.strangleTarget = brain.gameObject;
        }
        else
        {
            
            brain.EnterConversation(playerMvmnt.transform);
            playerMvmnt.SetTarget(hit.point);
            conversationTarget = brain;
        }
    }
}
