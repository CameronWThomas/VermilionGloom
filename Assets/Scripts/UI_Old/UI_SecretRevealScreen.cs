using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class UI_SecretRevealScreen : MonoBehaviour
{
    [SerializeField] private GameObject _miniGameZoneArea;
    [SerializeField] private RectTransform _miniGameIndicator;
    [SerializeField, Range(0f, 1f)] private float _timeForIndicatorToCrossBar = 1f;
    [SerializeField, Range(0f, 1f)] private float _delayAfterGame = 1f;

    private int _usedVPoints = 0;

    private Dictionary<SecretLevel, List<UI_MiniGameZone_Old>> _zones = null;

    private List<Secret> _unrevealedSecrets = new();
    private SecretLevel _allowableSecretLevel = SecretLevel.Public;
    private bool _allowVampiricSecrets = false;
    
    private bool _miniGamePlaying = false;
    private bool _playerEndedMiniGame = false;

    private Action<SecretLevel?, bool> _onFinish;

    private void Update()
    {
        if (_miniGamePlaying && Input.GetKeyDown(KeyCode.Space))
        {
            _playerEndedMiniGame = true;
        }
    }

    public void Initialize(NPCHumanCharacterID characterID, Action<SecretLevel?, bool> onFinish)
    {
        _onFinish = onFinish;

        _zones ??= InitializeMiniGameZoneDict();

        _usedVPoints = 0;
        _allowVampiricSecrets = false;
        _allowableSecretLevel = SecretLevel.Public;
        _unrevealedSecrets.Clear();
        _miniGamePlaying = false;
        _playerEndedMiniGame = false;
        _miniGameIndicator.gameObject.SetActive(false);

        var secrets = CharacterSecretKnowledgeBB.Instance.GetSecrets(characterID);
        //_unrevealedSecrets = secrets.Where(x => !x.IsRevealed).ToList();

        UpdateAllowableSecretLevel();
    }   

    public void IncreaseAllowedSecretLevel()
    {
        if (_miniGamePlaying)
            return;

        if (_allowableSecretLevel == SecretLevel.Confidential || !TryUpdateUsedVPoints(1))
            return;

        _allowableSecretLevel++;
        UpdateAllowableSecretLevel();
    }

    public void DecreaseAllowedSecretLevel()
    {
        if (_miniGamePlaying)
            return;

        if (_allowableSecretLevel == SecretLevel.Public || !TryUpdateUsedVPoints(-1))
            return;

        _allowableSecretLevel--;
        UpdateAllowableSecretLevel();
    }

    public void ToggleAllowVampiricSecrets()
    {
        if (_miniGamePlaying)
            return;

        if (!_unrevealedSecrets.Any(x => x.Level == SecretLevel.Vampiric))
            return;

        var vampirePoints = _allowVampiricSecrets ? -1 : 1;
        if (!TryUpdateUsedVPoints(vampirePoints))
            return;

        _allowVampiricSecrets = !_allowVampiricSecrets;
        UpdateAllowableSecretLevel();
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

    public void CloseScreen()
    {
        StopAllCoroutines();
        PlayerStats.Instance.TrySetPendingVampirePoints(0);
        _onFinish(null, false);
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

        SecretLevel? unlockedSecretLevel = null;
        foreach (var zone in _zones.Values.SelectMany(x => x))
        {
            if (!zone.IsEmpty && zone.IsPointInZone(_miniGameIndicator.anchoredPosition.x))
            {
                unlockedSecretLevel = zone.Level;
                break;
            }
        }

        yield return new WaitForSeconds(_delayAfterGame);

        PlayerStats.Instance.TryUseVampirePoints(_usedVPoints);
        _onFinish(unlockedSecretLevel, true);
    }

    private bool TryUpdateUsedVPoints(int diff)
    {
        var usedVPoints = Mathf.Max(0, _usedVPoints + diff);
        if (!PlayerStats.Instance.TrySetPendingVampirePoints(usedVPoints))
            return false;

        _usedVPoints = usedVPoints;
        return true;
    }

    private Dictionary<SecretLevel, List<UI_MiniGameZone_Old>> InitializeMiniGameZoneDict()
    {
        return _miniGameZoneArea.GetComponentsInChildren<UI_MiniGameZone_Old>(true)
            .Where(x => !x.IsEmpty)
            .GroupBy(x => x.Level)
            .ToDictionary(x => x.Key, x => x.ToList());
    }

    private void UpdateAllowableSecretLevel()
    {
        SecretLevel? lastAllowedSecretLevel = null;

        foreach (var keyValue in _zones)
        {
            var secretLevel = keyValue.Key;
            var zones = keyValue.Value;

            if (IsAllowableMiniGameZone(secretLevel))
                lastAllowedSecretLevel = secretLevel;

            if (lastAllowedSecretLevel.HasValue)
                zones.ForEach(x => x.SetLevel(lastAllowedSecretLevel.Value));
            else
                zones.ForEach(x => x.SetEmpty());
        }
    }

    private bool IsAllowableMiniGameZone(SecretLevel zoneLevel)
    {
        // No secrets of this level to reveal
        if (!_unrevealedSecrets.Any(x => x.Level == zoneLevel))
            return false;

        if (zoneLevel <= _allowableSecretLevel)
            return true;

        if (zoneLevel == SecretLevel.Vampiric && _allowVampiricSecrets)
            return true;

        return false;
    }
        
}