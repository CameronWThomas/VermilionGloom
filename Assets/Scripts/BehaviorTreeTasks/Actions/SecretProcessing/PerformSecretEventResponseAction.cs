using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

[TaskCategory("Custom/Secret Processing")]
public class PerformSecretEventResponseAction : Action
{
    public SharedFloat Duration = new() { Value = 2f };

    NpcBrain _brain;
    System.Action _endDurationEvent = null;

    float _timerStart = 0f;

    public override void OnStart()
    {
        _endDurationEvent = null;
        _timerStart = 0f;

        _brain = GetComponent<NpcBrain>();

        var secretEvent = _brain.SecretFromLastProcessedSecretEvent;
        if (secretEvent == null)
            return;

        if (secretEvent is MurderSecret murderSecret)
            InitializeSecret(murderSecret);
        else if (secretEvent is DragSecret dragSecret)
            InitializeSecret(dragSecret);
    }    

    public override TaskStatus OnUpdate()
    {
        if (Time.time - _timerStart <= Duration.Value)
            return TaskStatus.Running;

        _brain.SecretEventResponse = SecretEventResponse.NA;
        _endDurationEvent?.Invoke();

        if (_brain.SecretEventToBroadcast != null)
            NpcBehaviorBB.Instance.BroadcastSecretEvent(_brain.SecretEventToBroadcast);

        return TaskStatus.Success;
    }

    private void InitializeSecret(MurderSecret murderSecret)
    {
        _timerStart = Time.time;

        if (murderSecret.IsJustified)
            _brain.SecretEventResponse = SecretEventResponse.ThumbsUp;
        else
        {
            _brain.SecretEventResponse = SecretEventResponse.Point;
            _endDurationEvent = () => _brain.GetHostile(_brain.SecretFromLastProcessedSecretEvent.SecretTarget);
        }
    }

    private void InitializeSecret(DragSecret dragSecret)
    {
        _timerStart = Time.time;
        _brain.SecretEventResponse = SecretEventResponse.Point;
        _endDurationEvent = () => _brain.GetHostile(_brain.SecretFromLastProcessedSecretEvent.SecretTarget);
    }
}