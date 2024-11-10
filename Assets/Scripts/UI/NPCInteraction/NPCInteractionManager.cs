using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NPCInteractionManager : MonoBehaviour
{
    enum NPCInteractionContext
    {
        MainScreen, InformationSelectScreen
    }

    [SerializeField] Image _playerImageObject;
    [SerializeField] TMP_Text _playerNameText;
    [SerializeField] KnowledgeBaseUI _knowledgeBaseUI;
    [SerializeField] ActionsUI _actionsUI;

    [SerializeField] GameObject _lock;

    public event Action<ActionType> OnAction;

    NPCCharacterInfo _targetNPC;
    ActionType? _currentActionType = null;


    NPCInteractionContext _currentContext = NPCInteractionContext.MainScreen;

    bool IsLocked => _lock.activeInHierarchy;

    private void Start()
    {
        _actionsUI.OnAction += OnActionPerformed;
    }    

    public void Initialize(NPCCharacterInfo newCharacterInfo)
    {
        Unlock();

        _targetNPC = newCharacterInfo;

        _playerImageObject.color = _targetNPC.NPCColor;
        _playerNameText.text = _targetNPC.Name;

        UpdateKnowledgeBaseUI();

        _actionsUI.Initialize();

        OnNewContext();
    }

    public void Lock() => UpdateLock(true);
    public void Unlock() => UpdateLock(false);

    public void UpdateActionState(bool successful)
    {
        var requireKnowledgeBaseUpdate = true;
        if (!successful)
            requireKnowledgeBaseUpdate = false;
        else if (_currentActionType is ActionType.Investigate)
            _targetNPC.IncreasePrivacyLevel();
        else
            requireKnowledgeBaseUpdate = false;

        if (requireKnowledgeBaseUpdate)
        {
            UpdateKnowledgeBaseUI();
        }

        _currentActionType = null;
    }

    private void UpdateKnowledgeBaseUI()
    {
        _knowledgeBaseUI.ResetKnowledge();

        var knowledgeBase = _targetNPC.KnowledgeBase;
        foreach (var information in knowledgeBase.Knowledge.Where(x => x.PrivacyLevel <= _targetNPC.UnlockedPrivacyLevel))
        {
            _knowledgeBaseUI.AddInformation(information);
        }
    }

    private void UpdateLock(bool isLocked)
    {
        _lock.SetActive(isLocked);
    }

    private void OnNewContext()
    {
        var newContext = _currentContext;

        var actionTypes = newContext switch
        {
            NPCInteractionContext.MainScreen => new[] { ActionType.Investigate, ActionType.Influence, ActionType.Trance },
            NPCInteractionContext.InformationSelectScreen => new[] { ActionType.Influence, ActionType.Delete },
            _ => new ActionType[] { }
        };

        _actionsUI.ActivateActions(actionTypes);
    }

    private void OnActionPerformed(ActionType actionType)
    {
        _currentActionType = actionType;
        OnAction?.Invoke(actionType);
    }
}
