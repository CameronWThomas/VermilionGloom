using System;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class MouseReceiver : GlobalSingleInstanceMonoBehaviour<MouseReceiver>
{
    public MvmntController playerMvmnt;
    PlayerController playerController;
    public NpcBrain conversationTarget;
    public bool hostile = false;

    private int _deactivatedCounter = 0;
    public bool IsActivated => _deactivatedCounter == 0;


    public LayerMask clickLayerMask;
    protected override void Start()
    {
        base.Start();
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

        if (hit.transform.IsWalkable())
        {
            //if(playerController._strangleTarget != null)
            //{
            //    NpcBrain brain = playerController._strangleTarget.GetComponent<NpcBrain>();
            //    if(brain != null)
            //        brain.StopBeingStrangled();

            //    playerController.CancelStrangling();

            //}
            playerMvmnt.GoToTarget(hit.point);
        }
        else if (hit.transform.IsInteractable())
        {
            HandleInteractableClick(hit);
        }
        else if (hit.transform.IsNpc())
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
                //conversationTarget.ExitConversation(PlayerStats.Instance.transform);
                conversationTarget = null;
            }
        }


        // End dragging if clicking on player or the drag target

        if (hit.transform.IsPlayer() && playerController.IsDragging)
        {
            playerController.StopDragging();
            return true;
        }
        if (playerController.IsDragging && hit.transform == playerController.DragTarget.transform)
        {
            playerController.StopDragging();
            return true;
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
        Debug.Log("Secret Passage Clicked: " + secretPassage.name);
        playerMvmnt.GoToTarget(secretPassage.DestinationPoint, () => secretPassage.UsePassage(PlayerStats.Instance.transform));
    }

    private void HandleNpcClick(RaycastHit hit)
    {
        Debug.Log("Npc Clicked: " + hit.transform.name);

        NpcBrain brain = hit.transform.GetComponent<NpcBrain>();
        if (brain == null)
        {
            Debug.LogError("NpcBrain not found on " + hit.transform.name);
            return;
        }
        else
        {
            //Drag, strangle, or talk
            if (brain.IsDead)
            {
                //drag that mofo
                playerController.Drag(brain);
            }
            else if (hostile)
            {
                playerController.Strangle(brain);
            }
            else
            {
                playerMvmnt.GoToTarget(brain.transform, () => EnterConversationWithNpc(brain));
                conversationTarget = brain;
            }
        }
    }

    private void EnterConversationWithNpc(NpcBrain brain)
    {
        playerMvmnt.FaceTarget(brain.transform.position);
        UI_CharacterInteractionMenu.Instance.Activate(brain.GetNPCHumanCharacterID());
    }
}
