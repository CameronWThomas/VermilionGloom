using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class UI_MiniGame : MonoBehaviour
{
    [SerializeField] GameObject _miniGameZoneArea;
    [SerializeField] RectTransform _miniGameIndicator;
    [SerializeField, Range(0f, 1f)] float _timeForIndicatorToCrossBar = .3f;
    [SerializeField, Range(0f, 1f)] float _delayAfterGame = 1f;

    private Dictionary<int, List<UI_MiniGameZone>> _zones = null;

    private bool _miniGamePlaying = false;
    private bool _playerEndedMiniGame = false;

    private Action<bool> _onGameFinish;

    private void Update()
    {
        if (_miniGamePlaying && Input.GetKeyDown(KeyCode.Space))
        {
            _playerEndedMiniGame = true;
        }
    }

    public void Initialize(Action<bool> onGameFinish)
    {
        _onGameFinish = onGameFinish;

        _zones ??= InitializeMiniGameZoneDict();
        SetupRandomMiniGameLevel();

        _miniGamePlaying = false;
        _playerEndedMiniGame = false;
        _miniGameIndicator.gameObject.SetActive(false);
    }

    public void PlayMiniGame()
    {
        if (_miniGamePlaying)
        {
            _playerEndedMiniGame = true;
            return;
        }

        _miniGamePlaying = true;
        StartCoroutine(MoveIndicator());
    }

    public void StopMiniGame()
    {
        StopAllCoroutines();
    }    

    private Dictionary<int, List<UI_MiniGameZone>> InitializeMiniGameZoneDict()
    {
        return _miniGameZoneArea.GetComponentsInChildren<UI_MiniGameZone>(true)
            .Where(x => !x.IsEmpty)
            .GroupBy(x => x.Level)
            .ToDictionary(x => x.Key, x => x.ToList());
    }

    private void SetupRandomMiniGameLevel()
    {
        var minLevel = GetRandomLevel();

        foreach (var keyValue in _zones)
        {
            var level = keyValue.Key;
            var zones = keyValue.Value;

            if (level >= minLevel)
                zones.ForEach(x => x.SetLevel(minLevel));
            else
                zones.ForEach(x => x.SetEmpty());
        }
    }

    private int GetRandomLevel()
    {
        var value = UnityEngine.Random.Range(0f, 1f);

        if (value > .6f)
            return 1;
        else if (value > .3f)
            return 2;
        else if (value > .1f)
            return 3;
        else
            return 4;
    }

    private IEnumerator MoveIndicator()
    {

        var indicatorWidth = _miniGameIndicator.rect.width;
        var indicatorParentWidth = _miniGameIndicator.parent.GetComponent<RectTransform>().rect.width;

        var anchoredPositionStart = new Vector2(indicatorWidth / 2f, 0f);
        var anchoredPositionEnd = new Vector2(indicatorParentWidth - indicatorWidth / 2f, 0f);

        _miniGameIndicator.gameObject.SetActive(true);

        var timeDelta = Time.deltaTime;

        var t = 0f;
        var timeCount = Time.time;
        while (true)
        {
            if (_playerEndedMiniGame)
                break;

            if (t > 1f)
            {
                timeCount = Time.time;
                var startTemp = anchoredPositionStart;
                anchoredPositionStart = anchoredPositionEnd;
                anchoredPositionEnd = startTemp;
            }

            t = (Time.time - timeCount) / _timeForIndicatorToCrossBar;
            _miniGameIndicator.anchoredPosition = Vector2.Lerp(anchoredPositionStart, anchoredPositionEnd, t);

            yield return new WaitForSeconds(timeDelta);
        }

        bool success = false;
        foreach (var zone in _zones.Values.SelectMany(x => x))
        {
            if (!zone.IsEmpty && zone.IsPointInZone(_miniGameIndicator.anchoredPosition.x))
            {
                success = true;
                break;
            }
        }

        yield return new WaitForSeconds(_delayAfterGame);
        
        _onGameFinish(success);
    }

}