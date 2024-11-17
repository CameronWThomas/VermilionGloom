using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UI_SecretRevealScreen : MonoBehaviour
{
    [SerializeField] private GameObject _miniGameZoneArea;
    [SerializeField] private GameObject _miniGameIndicator;

    private int _usedVPoints = 0;

    private Dictionary<SecretLevel, List<UI_MiniGameZone>> _zones = null;

    private List<Secret> _unrevealedSecrets = new();
    private SecretLevel _allowableSecretLevel = SecretLevel.Public;
    private bool _allowVampiricSecrets = false;

    public void Initialize(CharacterID characterID)
    {
        _zones ??= InitializeMiniGameZoneDict();

        ResetScreen();

        var secrets = CharacterSecretKnowledgeBB.Instance.GetSecrets(characterID);
        _unrevealedSecrets = secrets.Where(x => !x.IsRevealed).ToList();

        UpdateAllowableSecretLevel();
    }   

    public void IncreaseAllowedSecretLevel()
    {
        if (_allowableSecretLevel == SecretLevel.Confidential || !TryUpdateUsedVPoints(1))
            return;

        _allowableSecretLevel++;
        UpdateAllowableSecretLevel();
    }

    public void DecreaseAllowedSecretLevel()
    {
        if (_allowableSecretLevel == SecretLevel.Public || !TryUpdateUsedVPoints(-1))
            return;

        _allowableSecretLevel--;
        UpdateAllowableSecretLevel();
    }

    public void ToggleAllowVampiricSecrets()
    {
        if (!_unrevealedSecrets.Any(x => x.Level == SecretLevel.Vampiric))
            return;

        var vampirePoints = _allowVampiricSecrets ? -1 : 1;
        if (!TryUpdateUsedVPoints(vampirePoints))
            return;

        _allowVampiricSecrets = !_allowVampiricSecrets;
        UpdateAllowableSecretLevel();
    }

    private bool TryUpdateUsedVPoints(int diff)
    {
        var usedVPoints = Mathf.Max(0, _usedVPoints + diff);
        if (!PlayerStats.Instance.TrySetPendingVampirePoints(usedVPoints))
            return false;

        _usedVPoints = usedVPoints;
        return true;
    }

    private Dictionary<SecretLevel, List<UI_MiniGameZone>> InitializeMiniGameZoneDict()
    {
        return _miniGameZoneArea.GetComponentsInChildren<UI_MiniGameZone>(true)
            .Where(x => !x.IsEmpty)
            .GroupBy(x => x.Level)
            .ToDictionary(x => x.Key, x => x.ToList());
    }

    private void ResetScreen()
    {
        _usedVPoints = 0;
        _allowVampiricSecrets = false;
        _allowableSecretLevel = SecretLevel.Public;
        _unrevealedSecrets.Clear();
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
                zones.ForEach(x => x.SetSecretLevel(lastAllowedSecretLevel.Value));
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