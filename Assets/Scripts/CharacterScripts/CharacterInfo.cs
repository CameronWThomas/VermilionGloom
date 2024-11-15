using UnityEngine;

public enum CharacterType { Generic, VanHelsing, Owner }

public class CharacterInfo : MonoBehaviour
{
    [SerializeField] private string _name = "Unassigned";
    [SerializeField] private CharacterType _characterType = global::CharacterType.Generic;

    public string Name => _name;

    public CharacterType CharacterType => _characterType;
}