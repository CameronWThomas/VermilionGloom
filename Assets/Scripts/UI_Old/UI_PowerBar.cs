using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UI_PowerBar : MonoBehaviour
{
    private enum PowerBarType { Detective, Vampire }

    [SerializeField] PowerBarType _powerBarType = PowerBarType.Detective;
    [SerializeField] private Transform _powerBarContent;

    //TODO I would like to go to a global prefab class or something for this
    [SerializeField] private GameObject _powerUnitPrefab;
    [SerializeField] private GameObject _powerUnitDividerPrefab;

    private List<UI_PowerUnit> _orderedPowerUnits = new();
    private List<GameObject> _dividers = new();

    private NPCHumanCharacterID _characterID;

    private void Update()
    {
        if (_powerBarType is PowerBarType.Vampire)
            CheckAndPerformVampirePointUpdate();
        else if (_powerBarType is PowerBarType.Detective)
            CheckAndPerformDetectivePointUpdate();
    }    

    public void Initialize(NPCHumanCharacterID characterID)
    {
        _characterID = characterID;
        Initialize(NPCHumanCharacterInfo.MAX_DETECTIVE_POINTS);
        CheckAndPerformDetectivePointUpdate();
    }

    private void Initialize(int maxUnits)
    {
        ResetPowerUnitsAndDividers();

        for (var i = 0; i < maxUnits; i++)
        {
            var powerUnit = Instantiate(_powerUnitPrefab, _powerBarContent).GetComponent<UI_PowerUnit>();
            _orderedPowerUnits.Add(powerUnit);

            if (i + 1 != maxUnits)
                AddDivider();
        }
    }

    private void AddDivider()
    {
        var divider = Instantiate(_powerUnitDividerPrefab, _powerBarContent);
        _dividers.Add(divider);
    }

    private void ResetPowerUnitsAndDividers()
    {
        _dividers.ForEach(x => Destroy(x));
        _orderedPowerUnits.ForEach(x => Destroy(x.gameObject));

        _dividers.Clear();
        _orderedPowerUnits.Clear();
    }

    private void CheckAndPerformVampirePointUpdate()
    {
        CheckAndPerformPointUpdate(
            PlayerStats.Instance.MaxVampirePoints,
            PlayerStats.Instance.PendingUseVampirePoints,
            PlayerStats.Instance.CurrentVampirePoints);
        
        //var currentMaxVampirePoints = _orderedPowerUnits.Count;
        //if (currentMaxVampirePoints != actualMaxVampirePoints)
        //    Initialize(PlayerStats.Instance.MaxVampirePoints);

        //var actualPending = PlayerStats.Instance.PendingUseVampirePoints;
        //var actualUnused = PlayerStats.Instance.CurrentVampirePoints;
        //var actualUsed = actualMaxVampirePoints - actualUnused;

        //for (var i = 0; i < _orderedPowerUnits.Count; i++)
        //{
        //    if (i < actualUnused - actualPending)
        //        _orderedPowerUnits[i].SetUnused();
        //    else if (i < actualUnused)
        //        _orderedPowerUnits[i].SetPendingUse();
        //    else
        //        _orderedPowerUnits[i].SetUsed();
        //}
    }

    private void CheckAndPerformDetectivePointUpdate()
    {
        CheckAndPerformPointUpdate(NPCHumanCharacterInfo.MAX_DETECTIVE_POINTS,
            _characterID.PendingDetectivePoints,
            _characterID.CurrentDetectivePoints);
    }

    private void CheckAndPerformPointUpdate(int max, int pending, int unused)
    {
        var currentMaxVampirePoints = _orderedPowerUnits.Count;
        if (currentMaxVampirePoints != max)
            Initialize(PlayerStats.Instance.MaxVampirePoints);
        
        var used = max - unused;

        for (var i = 0; i < _orderedPowerUnits.Count; i++)
        {
            if (i < unused - pending)
                _orderedPowerUnits[i].SetUnused();
            else if (i < unused)
                _orderedPowerUnits[i].SetPendingUse();
            else
                _orderedPowerUnits[i].SetUsed();
        }
    }

    private void UpdateUsedUnits(int actualUsed)
    {
        _orderedPowerUnits
            .TakeLast(actualUsed)
            .ToList()
            .ForEach(x => x.SetUnused());
    }

    private void UpdateUnusedUnits(int actualUnused)
    {
        _orderedPowerUnits
            .Take(actualUnused)
            .ToList()
            .ForEach(x => x.SetUnused());
    }

    private void UpdatePendingUnits(int pendingUnits)
    {
        _orderedPowerUnits
            .Where(x => x.IsUnused || x.IsPending)
            .TakeLast(pendingUnits)
            .ToList()
            .ForEach(x => x.SetPendingUse());
    }
}