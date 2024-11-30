using System;
using UnityEngine;

public enum Objective
{
    GoToDen,
    CollectBodies,
    KillEveryone
}

public class GameState : GlobalSingleInstanceMonoBehaviour<GameState>
{
    private const int MAX_BODIES = 10;

    [Header("Progression")]
    public bool VampireLordVisited = false;
    [Range(0, MAX_BODIES)]public int BodyDeliverCount = 0;

    [Header("Player Abilities")]
    public bool LongRangeInteracting = false;
    public bool PauseOnInteract = false;

    [Header("Progression Conditions")]
    [SerializeField, Range(0, MAX_BODIES)]public int WinGameBodyCount = 5;
    [SerializeField, Range(0, MAX_BODIES)]public bool GameWon = false;

    [Header("Needed to help run sequences")]
    public VampireController Vampire;
    public CoffinController CoffinController;

    public Objective CurrentObjective => GetCurrentObjective();
    public string ObjectiveMessage => CurrentObjective switch
    {
        Objective.GoToDen => "An unnatural urge compells you to look in the fireplace, in the northwest corner of the foyer...",
        Objective.CollectBodies => "You feel empowered by the vampires blessing, bring " + (5 - BodyDeliverCount) + " bodies to him...",
        Objective.KillEveryone => "KILL",
        _ => throw new NotImplementedException()
    };    

    protected override void Start()
    {
        base.Start();
     
        PutVampireLordInDefaultPosition();        
    }

    public void Progress()
    {
        BodyDeliverCount++;

        if (BodyDeliverCount >= 2)
        {
            LongRangeInteracting = true;
        }
        if(BodyDeliverCount >= 3)
        {
            PauseOnInteract = true;
        }
    }
    public void PutVampireLordInDefaultPosition()
    {
        GetVampireLordDefaultPositionAndRotation(out var position, out var rotation);
        Vampire.transform.SetPositionAndRotation(position, rotation);
    }

    public void GetVampireLordDefaultPositionAndRotation(out Vector3 position, out Quaternion rotation)
    {
        var defaultVampireTransform = UsefulTransforms.Instance.V_Default;
        position = defaultVampireTransform.position;
        rotation = defaultVampireTransform.rotation;
    }

    private Objective GetCurrentObjective()
    {
        if (!VampireLordVisited)
            return Objective.GoToDen;
        else if (!GameWon)
            return Objective.CollectBodies;
        else
            return Objective.KillEveryone;
    }
}