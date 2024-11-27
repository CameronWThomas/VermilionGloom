using System;
using UnityEngine;
using UnityEngine.UI;

public class UI_PowerUnit : MonoBehaviour
{
    private enum PowerUnitState { Unused, PendingUse, Used }

    [SerializeField] private Color _unused = Color.red;
    [SerializeField] private Color _pendingUse = Color.yellow;
    [SerializeField] private Color _used = new Color(0, 0, 0, 0f);

    private PowerUnitState _state = PowerUnitState.Unused;

    private void Start()
    {
        OnStateChange();
    }

    public bool IsPending => _state == PowerUnitState.PendingUse;
    public bool IsUnused => _state == PowerUnitState.Unused;
    public bool IsUsed => _state == PowerUnitState.Used;

    public void SetUnused() => UpdateState(PowerUnitState.Unused);
    public void SetPendingUse() => UpdateState(PowerUnitState.PendingUse);
    public void SetUsed() => UpdateState(PowerUnitState.Used);

    private void UpdateState(PowerUnitState newPowerState)
    {
        if (_state == newPowerState)
            return;

        _state = newPowerState;
        OnStateChange();
    }

    private void OnStateChange()
    {
        var color = _state switch
        {
            PowerUnitState.Unused => _unused,
            PowerUnitState.PendingUse => _pendingUse,
            PowerUnitState.Used => _used,
            _ => throw new NotImplementedException()
        };

        GetComponent<Image>().color = color;
    }
}