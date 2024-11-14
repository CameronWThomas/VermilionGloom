using System;
using UnityEngine;

public class MouseReceiver : GlobalSingleInstanceMonoBehaviour<MouseReceiver>
{
    public MvmntController playerMvmnt;

    private int _deactivatedCounter = 0;
    public bool IsActivated => _deactivatedCounter == 0;

    //public LayerMask movementLayerMask;

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
        Debug.Log("Mouse Clicked");
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        // Debug draw the ray
        //Debug.DrawRay(ray.origin, ray.direction, Color.red);

        if (!Physics.Raycast(ray, out var hit, Mathf.Infinity))
            return;
        
        Debug.Log("Mouse Hit:");
        Debug.Log("     Transform Name: " + hit.transform.name);
        Debug.Log("     Point: " + hit.point);


        if(hit.transform.CompareTag(GlobalConstants.WALKABLE_TAG_NAME))
        {
            playerMvmnt.SetTarget(hit.point);
        }
        else if (hit.transform.CompareTag(GlobalConstants.INTERACTABLE_TAG_NAME))
        {
            HandleInteractableClick(hit);
        }
    }

    private void HandleInteractableClick(RaycastHit hit)
    {
        if (hit.transform.TryGetComponent<SecretPassage>(out var secretPassage))
            HandleInteractableClick(secretPassage);
        else if (hit.transform.TryGetComponent<NpcBrain>(out var brain))
            HandleInteractableClick(brain);
    }

    private void HandleInteractableClick(SecretPassage secretPassage)
    {
        playerMvmnt.SetTarget(secretPassage.DestinationPoint, () => secretPassage.UsePassage(playerMvmnt.transform));
    }

    private void HandleInteractableClick(NpcBrain brain)
    {
        //TODO move to them too
        UI_MenuController.Instance.TalkToNPC(brain);
    }
}
