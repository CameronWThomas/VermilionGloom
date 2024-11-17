using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_PowerBar : MonoBehaviour
{
    [SerializeField] private Transform _powerBarContent;

    //TODO I would like to go to a global prefab class or something for this
    [SerializeField] private GameObject _powerUnitPrefab;
    [SerializeField] private GameObject _powerUnitDividerPrefab;

    private List<UI_PowerUnit> _orderedPowernUnit = new();
    private List<GameObject> _dividers = new();

    public void Initialize(int maxUnits)
    {
        ResetPowerUnitsAndDividers();

        for (var i = 0; i < maxUnits; i++)
        {
            var powerUnit = Instantiate(_powerUnitPrefab, _powerBarContent).GetComponent<UI_PowerUnit>();
            _orderedPowernUnit.Add(powerUnit);

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
        _orderedPowernUnit.ForEach(x => Destroy(x.gameObject));

        _dividers.Clear();
        _orderedPowernUnit.Clear();
    }


}