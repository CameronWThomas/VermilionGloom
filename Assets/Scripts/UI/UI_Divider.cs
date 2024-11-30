using System;
using UnityEngine;
using UnityEngine.UI;

public class UI_Divider : MonoBehaviour
{
    GameObject _neighborOne;
    GameObject _neighborTwo;

    int _ourIndex = -1;
    float _defaultSize = 0f;
    Action<float> _hiddenSizeUpdater;

    private void Start()
    {
        _ourIndex = -1;
        for (int i = 0; i < transform.parent.childCount; i++)
        {
            if (transform.parent.GetChild(i) == transform)
            {
                _ourIndex = i;
                break;
            }
        }

        var layoutElement = GetComponent<LayoutElement>();
        if (layoutElement.preferredHeight > 0)
        {
            _defaultSize = layoutElement.preferredHeight;
            _hiddenSizeUpdater = x => layoutElement.preferredHeight = x;
        }
        else if (layoutElement.preferredWidth > 0)
        {
            _defaultSize = layoutElement.preferredWidth;
            _hiddenSizeUpdater = x => layoutElement.preferredWidth = x;
        }
    }

    private void Update()
    {
        var hasNeighborOne = HasActiveNeighbor(true);
        var hasNeighborTwo = HasActiveNeighbor(false);

        UpdateHidden(!hasNeighborOne || !hasNeighborTwo);
    }

    private void UpdateHidden(bool hide)
    {
        var size = hide ? 0f : _defaultSize;
        _hiddenSizeUpdater?.Invoke(size);
    }

    private bool HasActiveNeighbor(bool leftNeighbor)
    {
        var index = _ourIndex + (leftNeighbor ? -1 : 1);

        if (index < 0 || index >= transform.parent.childCount)
            return false;

        var diff = leftNeighbor ? -1 : 1;
        var end = leftNeighbor ? -1 : transform.parent.childCount;
        for (int i = index; i != end; i += diff)
        {
            if (transform.parent.GetChild(i).gameObject.activeInHierarchy)
                return true;
        }

        return false;
    }
}