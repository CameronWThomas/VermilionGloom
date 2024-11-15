using System;
using UnityEngine;

public enum CharacterType { Generic, VanHelsing, Owner }

public class CharacterInfo : MonoBehaviour
{
    public const int MAX_DETECTIVE_POINTS = 10;

    [SerializeField] private string _name = "Unassigned";
    [SerializeField] private CharacterType _characterType = global::CharacterType.Generic;

    private void Start()
    {
        RemainingDetectivePoints = MAX_DETECTIVE_POINTS;
    }

    public bool UseDetectivePoint(int points = 1)
    {
        if (points < 0 || RemainingDetectivePoints < points)
            return false;

        RemainingDetectivePoints -= points;
        return true;
    }

    public string Name => _name;
    public CharacterType CharacterType => _characterType;
    public int RemainingDetectivePoints { get; private set; }
}