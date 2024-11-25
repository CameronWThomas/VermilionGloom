using UnityEngine;

public class UsefulTransforms : GlobalSingleInstanceMonoBehaviour<UsefulTransforms>
{
    [Header("Vampire")]
    public Transform V_Default;
    public Transform V_FloatingAboveCoffin;
    public Transform V_InCoffin;
    public Transform V_InFrontOfCoffin;

    [Header("Player")]
    public Transform P_PreAddressingVampire;
    public Transform P_AddressingVampire;
    public Transform P_DragBodyToLocation;
    public Transform P_WatchSuck;
}