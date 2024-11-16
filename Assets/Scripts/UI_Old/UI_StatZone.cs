using System;
using TMPro;
using UnityEngine;

public class UI_StatZone : MonoBehaviour
{
    private enum StatType
    {
        Vampire,
        Detective
    }

    [SerializeField] StatType _statType = StatType.Vampire;

    private int _lastPointValue = -1;
    private int _lastMaxPointValue = -1;

    private CharacterInfo _characterInfo = null;

    private void Update()
    {
        GetActualPointValues(out var currentPoints, out var maxPoints);

        if (currentPoints != _lastPointValue ||
            maxPoints != _lastMaxPointValue)
            UpdatePoints(currentPoints, maxPoints);
    }

    public void SetCharacter(CharacterInfo characterInfo)
    {
        _characterInfo = characterInfo;
        _lastMaxPointValue = -1;
        _lastPointValue = -1;
    }


    private void UpdatePoints(int currentPoints, int maxPoints)
    {
        _lastPointValue = currentPoints;
        _lastMaxPointValue = maxPoints;

        var pointText = GetComponentInChildren<TMP_Text>(true);
        pointText.text = $"{_lastPointValue}/{_lastMaxPointValue}";
    }

    private void GetActualPointValues(out int currentPoints, out int maxPoints)
    {
        if (_statType is StatType.Vampire)
        {
            var playerStats = PlayerStats.Instance;
            currentPoints = playerStats.CurrentVampirePoints;
            maxPoints = playerStats.MaxVampirePoints;
        }
        else if (_statType is StatType.Detective && _characterInfo != null)
        {
            maxPoints = CharacterInfo.MAX_DETECTIVE_POINTS;
            currentPoints = _characterInfo.RemainingDetectivePoints;
        }
        else
        {
            currentPoints = _lastPointValue;
            maxPoints = _lastMaxPointValue;
        }
    }    
}