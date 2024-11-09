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

    public NPCCharacterInfo TargetNPC; //TODO this eventually will be set dynamically

    NPCInteractionContext _currentContext = NPCInteractionContext.MainScreen;
    bool _initialized = false;

    private void Update()
    {
        if (!_initialized)
        {
            Initialize();
            _initialized = true;
        }
    }

    private void Initialize()
    {
        _playerImageObject.color = TargetNPC.NPCColor;
        _playerNameText.text = TargetNPC.Name;

        var knowledgeBase = TargetNPC.KnowledgeBase;
        foreach (var information in knowledgeBase.Knowledge)
        {
            _knowledgeBaseUI.AddInformation(information);
        }

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

        foreach (var actionType in actionTypes)
        {
            _actionsUI.AddAction(actionType);
        }
    }
}
