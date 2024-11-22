using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

[TaskCategory("Custom")]
public class SetLatchToPlayer : Action
{
    public SharedBool IsLatched;

    public override void OnStart()
    {
        var movementController = transform.GetComponent<MvmntController>();

        Transform latchedTransform = null;
        if (IsLatched.Value)
            latchedTransform = PlayerStats.Instance.transform;

        movementController.LatchTarget = latchedTransform;
    }

    public override string FriendlyName => $"{(IsLatched.Value ? "Latch to" : "Unlatch from" )} Player";
}