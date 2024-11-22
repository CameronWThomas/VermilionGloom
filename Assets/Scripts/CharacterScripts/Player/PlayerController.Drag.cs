using UnityEngine;

public partial class PlayerController
{
    [Header("Drag Stuff")]
    public NpcBrain DragTarget;
    [SerializeField] private bool _isDragging;

    public bool IsDragging => _isDragging;

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

        DragTarget.StopBeingDragged();
        _isDragging = false;
        DragTarget = null;
    }

    private void BeginDraggingTarget()
    {
        _isDragging = true;
        DragTarget.BeDraged(gameObject);
        Broadcast(BroadcastType.Drag, DragTarget.gameObject);
    }
}