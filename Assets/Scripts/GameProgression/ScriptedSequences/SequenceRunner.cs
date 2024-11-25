using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SequenceRunner
{
    List<ISequencePart> _sequenceParts = new();

    private bool _hasBeenRun = false;
    private ParallelSequenceParts _parallelRoutineSequencePart = null;

    public SequenceRunner AddRoutine(Func<IEnumerator> sequencePartRoutine, float maxDuration = 10f)
    {
        var sequencePart = new RoutineSequencePart(sequencePartRoutine, maxDuration);
        if (_parallelRoutineSequencePart == null)
            _sequenceParts.Add(sequencePart);
        else
            _parallelRoutineSequencePart.SequenceParts.Add(sequencePart);

        return this;
    }

    public SequenceRunner AddWait(float duration)
    {
        var sequencePart = new DurationSequencePart(duration);
        if (_parallelRoutineSequencePart == null)
            _sequenceParts.Add(sequencePart);
        else
            _parallelRoutineSequencePart.SequenceParts.Add(sequencePart);

        return this;
    }

    public SequenceRunner StartAddingParallelSequenceRoutines(float maxDuration = 10f)
    {
        _parallelRoutineSequencePart = new ParallelSequenceParts(maxDuration);
        return this;
    }

    public SequenceRunner EndAddParallelRoutines()
    {
        _sequenceParts.Add(_parallelRoutineSequencePart);
        _parallelRoutineSequencePart = null;
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
            if (sequencePart is ParallelSequenceParts parallelSequencePart)
                yield return ParallelSequencePartRoutine(callingMonoBehaviour, parallelSequencePart);
            else
                yield return RunNonParallelSequencePart(callingMonoBehaviour, sequencePart);
        }

        onEndOfSequence?.Invoke();
    }

    private IEnumerator RunNonParallelSequencePart(MonoBehaviour callingMonoBehaviour, ISequencePart sequencePart, Action completeAction = null)
    {
        if (sequencePart is RoutineSequencePart routineSequencePart)
            yield return RoutineSequencePartRoutine(callingMonoBehaviour, routineSequencePart, completeAction);
        else if (sequencePart is DurationSequencePart durationSequencePart)
            yield return DurationRoutine(durationSequencePart.Duration, completeAction);
    }

    private IEnumerator RoutineSequencePartRoutine(MonoBehaviour callingMonoBehaviour, RoutineSequencePart routineSequencePart, Action completeAction)
    {
        var sequenceCoroutine = new CoroutineContainer(callingMonoBehaviour, routineSequencePart.SequencePartGetter);
        var durationLimiterCoroutine = new CoroutineContainer(callingMonoBehaviour, () => DurationRoutine(routineSequencePart.MaxDuration, null));

        sequenceCoroutine.Start();
        durationLimiterCoroutine.Start();

        while (!sequenceCoroutine.HasEnded && !durationLimiterCoroutine.HasEnded)
            yield return new WaitForNextFrameUnit();

        completeAction?.Invoke();
    }

    private IEnumerator DurationRoutine(float duration, Action completeAction)
    {
        yield return new WaitForSeconds(duration);
        completeAction?.Invoke();
    }

    private IEnumerator ParallelSequencePartRoutine(MonoBehaviour callingMonoBehaviour, ParallelSequenceParts parallelSequencePart)
    {
        var endCount = 0;
        var parallelSequenceCount = parallelSequencePart.SequenceParts.Count;
        foreach (var sequencePart in parallelSequencePart.SequenceParts)
            callingMonoBehaviour.StartCoroutine(RunNonParallelSequencePart(callingMonoBehaviour, sequencePart, () => endCount++));

        while (endCount < parallelSequenceCount)
            yield return new WaitForNextFrameUnit();
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

    private class ParallelSequenceParts : ISequencePart
    {
        public ParallelSequenceParts(float maxDuration)
        {
            MaxDuration = maxDuration;
        }

        public List<ISequencePart> SequenceParts { get; } = new();
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