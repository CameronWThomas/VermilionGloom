using UnityEngine;

public class CharacterInfo : MonoBehaviour
{
    [SerializeField] private string _name = "Unassigned";

    public string Name => _name;
}