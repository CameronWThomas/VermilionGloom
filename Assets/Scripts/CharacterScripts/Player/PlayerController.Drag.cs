using System;
using System.Collections;
using UnityEngine;

public partial class PlayerController
{
    [Header("Drag Stuff")]
    public NpcBrain DragTarget;

    public bool IsDragging => DragTarget != null;

    CoroutineContainer _draggingCoroutine;


    public void Drag(NpcBrain target)
    {
        if (DragTarget != null)
            return;

        DragTarget = target;
        mvmntController.GoToTarget(DragTarget.transform, BeginDraggingTarget);
    }

    public void StopDragging()
    {
        if (DragTarget == null)
            return;

        animator.SetBool("dragging", false);
        DragTarget.StopBeingDragged();

        DragTarget = null;
    }

    private void BeginDraggingTarget()
    {
        DragTarget.BeDraged(gameObject);
        animator.SetBool("dragging", true);
        Broadcast(BroadcastType.Drag, DragTarget.gameObject);
    }
}