using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SequenceRunner
{
    List<ISequencePart> _sequenceParts = new();

    private bool _hasBeenRun = false;

    public SequenceRunner AddRoutine(Func<IEnumerator> sequencePartRoutine, float maxDuration = 10f)
    {
        _sequenceParts.Add(new RoutineSequencePart(sequencePartRoutine, maxDuration));
        return this;
    }

    public SequenceRunner AddWait(float duration)
    {
        _sequenceParts.Add(new DurationSequencePart(duration));
        return this;
    }

    public void Run(MonoBehaviour callingMonoBehaviour, Action onEndOfSequence)
    {
        if (_hasBeenRun)
        {
            //onEndOfSequence();
            return;
        }
        _hasBeenRun = true;

        callingMonoBehaviour.StartCoroutine(SequenceRoutine(callingMonoBehaviour, onEndOfSequence));
    }

    private IEnumerator SequenceRoutine(MonoBehaviour callingMonoBehaviour, Action onEndOfSequence)
    {
        foreach (var sequencePart in _sequenceParts)
        {
            if (sequencePart is RoutineSequencePart routineSequencePart)
                yield return RoutineSequencePartRoutine(callingMonoBehaviour, routineSequencePart);
            else if (sequencePart is DurationSequencePart durationSequencePart)
                yield return DurationRoutine(durationSequencePart.Duration);
        }

        onEndOfSequence?.Invoke();
    }

    private IEnumerator RoutineSequencePartRoutine(MonoBehaviour callingMonoBehaviour, RoutineSequencePart routineSequencePart)
    {
        var hasSequenceEnded = false;
        var sequenceCoroutine = new CoroutineContainer(callingMonoBehaviour, routineSequencePart.SequencePartGetter, () => hasSequenceEnded = true);
        var durationLimiterCoroutine = new CoroutineContainer(callingMonoBehaviour, () => DurationRoutine(routineSequencePart.MaxDuration), () => hasSequenceEnded = true);

        sequenceCoroutine.Start();
        durationLimiterCoroutine.Start();

        while (!hasSequenceEnded)
            yield return new WaitForSeconds(Time.deltaTime);
    }

    private IEnumerator DurationRoutine(float duration)
    {
        yield return new WaitForSeconds(duration);
    }

    private interface ISequencePart { }

    private class RoutineSequencePart : ISequencePart
    {
        public RoutineSequencePart(Func<IEnumerator> sequencePartGetter, float maxDuration)
        {
            SequencePartGetter = sequencePartGetter;
            MaxDuration = maxDuration;
        }

        public Func<IEnumerator> SequencePartGetter { get; }
        public float MaxDuration { get; }
    }

    private class DurationSequencePart : ISequencePart
    {
        public DurationSequencePart(float duration)
        {
            Duration = duration;
        }

        public float Duration { get; }
    }
}