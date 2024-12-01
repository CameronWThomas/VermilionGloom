using System;
using System.Collections.Generic;
using UnityEngine;

public enum Objective
{
    GoToDen,
    CollectBodies,
    KillEveryone
}

public enum Tutorial
{
    FButton,
    HostileMode,
    InteractingMode,
    FirstStrangle,
    BaseMenu,
    Forget,
    Trance,
    LongRangeAbility,
    PauseOnInteract
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

    [Header("Tutorial")]
    private List<Tutorial> _completedTutorialStages = new();

    public Objective CurrentObjective => GetCurrentObjective();
    public string ObjectiveMessage => CurrentObjective switch
    {
        Objective.GoToDen => "The strange note told me to look in the fireplace...",
        Objective.CollectBodies => "You feel empowered by the creature's blessing, bring " + (5 - BodyDeliverCount) + " bodies to him...",
        Objective.KillEveryone => "KILL",
        _ => throw new NotImplementedException()
    };    

    protected override void Start()
    {
        base.Start();

        if (VampireLordVisited)
            VampireLordHasBeenVisited();

        PutVampireLordInDefaultPosition();        
    }

    public void VampireLordHasBeenVisited()
    {
        VampireLordVisited = true;
        PlayerController pc = FindObjectOfType<PlayerController>();
        pc.vampTurned = true;
    }

    public void AddCompletedTutorial(Tutorial completedTutorial)
    {
        if (_completedTutorialStages == null)
            _completedTutorialStages = new();

        if (_completedTutorialStages.Contains(completedTutorial))
            return;

        _completedTutorialStages.Add(completedTutorial);
    }

    public bool IsTutorialCompleted(Tutorial tutorial)
    {
        if (_completedTutorialStages == null)
            _completedTutorialStages = new();

        return _completedTutorialStages.Contains(tutorial);
    }

    public void ClearTutorials()
    {
        if (_completedTutorialStages == null)
            _completedTutorialStages = new();

        _completedTutorialStages.Clear();
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
        PlayerController pc  = FindObjectOfType<PlayerController>();
        pc.ModifyVampynessBasedOnGameState();
    }
    public void PutVampireLordInDefaultPosition()
    {
        GetVampireLordDefaultPositionAndRotation(out var position, out var rotation);
        Vampire.transform.SetPositionAndRotation(position, rotation);
    }

    public string TutorialMessage(Tutorial stage) => stage switch
    {
        Tutorial.FButton => "Press 'F' or click the icons to switch between hostile mode and interacting mode",
        Tutorial.HostileMode => "If you click on a character while in hostile mode, you will attack them",
        Tutorial.InteractingMode => "Interacting mode will only work if the character is not hostile",
        Tutorial.FirstStrangle => "Be careful! If you get spotted strangling someone, they won't forget it. Or can they...",
        Tutorial.BaseMenu => "When you interact with someone, you may probe their mind to read their thoughts. These thoughts influence their behaviour.",
        Tutorial.Forget => "The 'Forget' button lets you to erase the selected thought",
        Tutorial.Trance => "The 'Trance' button lets you create new thoughts that incite the character to murder",
        Tutorial.LongRangeAbility => "Your mind probe has been enhanced. It can be performed from a distance and while the character is hostile.",
        Tutorial.PauseOnInteract => "Your mind probe has been enhanced. Time will freeze during a probing sesh.",
        _ => string.Empty,
    };

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