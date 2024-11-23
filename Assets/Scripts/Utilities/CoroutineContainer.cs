using System;
using System.Collections;
using UnityEngine;

public class CoroutineContainer
{
    readonly MonoBehaviour _callingMonoBehaviour;
    readonly Func<IEnumerator> _getRoutine;
    readonly Action _onCoroutineEnd;
    readonly Action _onPrematureEnd;

    Coroutine _coroutine = null;

    public CoroutineContainer(MonoBehaviour callingMonoBehaviour, Func<IEnumerator> getRoutine, Action onCoroutineEnd = null, Action onPrematureEnd = null)
    {
        _callingMonoBehaviour = callingMonoBehaviour;
        _getRoutine = getRoutine;
        _onCoroutineEnd = onCoroutineEnd;
        _onPrematureEnd = onPrematureEnd;
    }

    public bool HasEnded { get; private set; } = false;

    public void Start()
    {
        if (_coroutine != null)
            return;

        var routine = Routine();
        _coroutine = _callingMonoBehaviour.StartCoroutine(routine);
    }

    public void Stop()
    {
        if (_coroutine == null || HasEnded)
            return;

        _callingMonoBehaviour.StopCoroutine(_coroutine);
        HasEnded = true;
        _onPrematureEnd?.Invoke();
    }

    private IEnumerator Routine()
    {
        yield return _getRoutine();
        HasEnded = true;
        _onCoroutineEnd?.Invoke();
    }
}