using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
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

    private int _xPos = 55;
    private List<ActionUI> _actions = new List<ActionUI>();

    public void Reset()
    {
        foreach (var action in _actions)
        {
            Destroy(action.gameObject);
        }

        _actions.Clear();
    }

    public void AddAction(ActionType actionType)
    {
        if (_actions.Any(x => x.ActionType == actionType))
            return;

        var texture = _actionTypeImages.First(x => x.ActionType == actionType).Texture;

        var actionUI = Instantiate(_actionPrefab, transform).GetComponent<ActionUI>();
        actionUI.Initialize(_xPos, actionType, texture);

        _xPos += ActionUI.WIDTH_INTERVAL;
    }
}
