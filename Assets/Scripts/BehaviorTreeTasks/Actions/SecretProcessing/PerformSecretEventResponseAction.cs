using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

[TaskCategory("Custom/Secret Processing")]
public class PerformSecretEventResponseAction : Action
{
    NpcBrain _brain;

    float _timerStart = 0f;
    float _duration = 0f;

    public override void OnStart()
    {
        _timerStart = Time.time;
        _duration = float.MinValue;

        _brain = GetComponent<NpcBrain>();

        var secretEventResponse = _brain.LastSecretEventResponse;
        if (secretEventResponse == null)
            return;

        var npcAnimator = transform.GetComponent<NpcCharacterAnimator>();
        if (npcAnimator.TryPlayAnimationForReponseType(secretEventResponse.ResponseType, out var animationDuration))
            _duration = animationDuration;
    }    

    public override TaskStatus OnUpdate()
    {
        if (Time.time - _timerStart <= _duration)
            return TaskStatus.Running;

        if (_brain.LastSecretEventResponse?.NewSecretEvent != null)
            NpcBehaviorBB.Instance.BroadcastSecretEvent(_brain.LastSecretEventResponse.NewSecretEvent);

        return TaskStatus.Success;
    }
}