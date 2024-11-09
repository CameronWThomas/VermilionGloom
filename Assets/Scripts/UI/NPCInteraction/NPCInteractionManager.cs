using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NPCInteractionManager : MonoBehaviour
{
    private enum NPCInteractionContext
    {
        MainScreen, InformationSelectScreen
    }

    [SerializeField] Image _playerImageObject;
    [SerializeField] TMP_Text _playerNameText;
    [SerializeField] KnowledgeBaseUI _knowledgeBaseUI;
    [SerializeField] ActionsUI _actionsUI;

    public NPCCharacterInfo StartingTargetNPC; //TODO this eventually will be set dynamically

    private NPCCharacterInfo _targetNPC; //TODO this eventually will be set dynamically

    NPCInteractionContext _currentContext = NPCInteractionContext.MainScreen;
    bool _initialized = false;

    private void Update()
    {
        if (!_initialized)
        {
            Initialize(StartingTargetNPC);
            _initialized = true;
        }
    }

    private void Initialize(NPCCharacterInfo newCharacterInfo)
    {
        _targetNPC = newCharacterInfo;

        _playerImageObject.color = _targetNPC.NPCColor;
        _playerNameText.text = _targetNPC.Name;

        var knowledgeBase = _targetNPC.KnowledgeBase;
        foreach (var information in knowledgeBase.Knowledge.Where(x => x.PrivacyLevel <= _targetNPC.UnlockedPrivacyLevel))
        {
            _knowledgeBaseUI.AddInformation(information);
        }

        _actionsUI.Initialize();

        OnNewContext();
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
}
