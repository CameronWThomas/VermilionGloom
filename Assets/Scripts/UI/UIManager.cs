using System;
using Unity.VisualScripting;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField, Tooltip("TODO remove")] NPCCharacterInfo _startingTargetNPC;

    UIContext _context = UIContext.Create();
    NPCInteractionManager _nPCInteractionManager;
    BottomBarManager _bottomBarManager;

    private void Start()
    {
        _nPCInteractionManager = GetComponentInChildren<NPCInteractionManager>();
        _bottomBarManager = GetComponentInChildren<BottomBarManager>();

        _nPCInteractionManager.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (_startingTargetNPC != null)
        {
            InteractWithNPC(_startingTargetNPC);
            _startingTargetNPC = null;
        }
    }

    private void InteractWithNPC(NPCCharacterInfo npcCharacterInfo)
    {
        _context.CoreContext = CoreContext.InNPCInteraction;
        _nPCInteractionManager.gameObject.SetActive(true);

        _nPCInteractionManager.Initialize(npcCharacterInfo);
        _nPCInteractionManager.OnAction += OnStartAction;
    }

    private void OnStartAction(ActionType type)
    {
        if (_context.CoreContext is not CoreContext.InNPCInteraction)
            return;

        _nPCInteractionManager.Lock();

        RunMiniGame();
    }

    private void RunMiniGame()
    {
        if (_context.SubContext is SubContext.InMiniGame)
            return;

        _context.SubContext = SubContext.InMiniGame;

        _bottomBarManager.OnMiniGameFinish += OnMiniGameFinish;
        _bottomBarManager.RunMiniGame();
    }

    private void OnMiniGameFinish(bool successful)
    {
        _context.SubContext = SubContext.Default;
        _bottomBarManager.OnMiniGameFinish -= OnMiniGameFinish;

        _nPCInteractionManager.UpdateActionState(successful);
        _nPCInteractionManager.Unlock();
    }

    private enum CoreContext
    {
        Default,
        InNPCInteraction,
    }

    private enum SubContext
    {
        Default,
        InMiniGame
    }

    private class UIContext
    {
        public CoreContext CoreContext { get; set; }
        public SubContext SubContext { get; set; }

        public static UIContext Create()
            => new UIContext() { CoreContext = CoreContext.Default, SubContext = SubContext.Default };
    }
}
