using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public enum ActionType
{
    Investigate,
    Influence,
    Delete,
    Trance
}


public class ActionsUI : MonoBehaviour
{
    [SerializeField] GameObject _actionPrefab;
    [SerializeField] List<ActionTypeInfo> _actionTypeImages = new();

    [Serializable]
    public class ActionTypeInfo
    {
        public ActionType ActionType;
        public Texture2D Texture;
    }

    private const int STARTING_X_POS = 55;
    private List<ActionUI> _actions = new List<ActionUI>();

    public void Reset()
    {
        foreach (var action in _actions)
        {
            Destroy(action.gameObject);
        }

        _actions.Clear();
    }

    public void Initialize()
    {
        var xPos = STARTING_X_POS;
        foreach (var actionTypeInfo in _actionTypeImages)
        {
            var actionUI = Instantiate(_actionPrefab, transform).GetComponent<ActionUI>();
            actionUI.Initialize(xPos, actionTypeInfo.ActionType, actionTypeInfo.Texture);
            _actions.Add(actionUI);

            xPos += ActionUI.WIDTH_INTERVAL;
        }
    }

    public void ActivateActions(ActionType[] actionTypes)
    {
        var actionUIActivating = _actions.Where(x => actionTypes.Contains(x.ActionType)).ToList();
        var actionUIDeactivating = _actions.Where(x => !actionTypes.Contains(x.ActionType)).ToList();

        foreach (var actionUI in actionUIActivating)
            actionUI.Activate();

        foreach (var actionUI in actionUIDeactivating)
            actionUI.Deactivate();
    }
}
