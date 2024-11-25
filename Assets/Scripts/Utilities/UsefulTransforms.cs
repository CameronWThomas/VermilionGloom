using UnityEngine;

public class UsefulTransforms : GlobalSingleInstanceMonoBehaviour<UsefulTransforms>
{
    [Header("Vampire")]
    public Transform V_Default;
    public Transform V_FloatingAboveCoffin;
    public Transform V_InCoffin;

    [Header("Player")]
    public Transform P_PreAddressingVampire;
    public Transform P_AddressingVampire;
}