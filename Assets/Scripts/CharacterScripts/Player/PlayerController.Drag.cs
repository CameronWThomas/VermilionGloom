using UnityEngine;

public partial class PlayerController
{
    [Header("Drag Stuff")]
    public NpcBrain DragTarget;
    [SerializeField] private bool _isDragging;

    public bool IsDragging => _isDragging;

    CoroutineContainer _draggingCoroutine;

    private SecretEvent _dragSecretEvent = null;


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

        NpcBehaviorBB.Instance.EndSecretEventBroadcast(_dragSecretEvent);
        _dragSecretEvent = null;
    }

    private void BeginDraggingTarget()
    {
        _isDragging = true;
        DragTarget.BeDraged(gameObject);

        _dragSecretEvent = new SecretEvent(SecretEventType.DraggingABody, this.GetCharacterID(), DragTarget.GetCharacterID(), SecretNoticability.Sight, SecretDuration.UntilCancel);
        NpcBehaviorBB.Instance.BroadcastSecretEvent(_dragSecretEvent);
    }
}