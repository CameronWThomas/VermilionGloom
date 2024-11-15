using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UIRumorsMenu : MonoBehaviour
{
    private List<SecretCollection> _secretCollections = new List<SecretCollection>();

    private CharacterInfo _activeCharacterInfo;

    public void Initialize(List<SecretCollection> othersSecretCollections)
    {
        _secretCollections.Clear();
        _secretCollections.AddRange(othersSecretCollections);

        SwitchActiveCharacter(othersSecretCollections.First().Character);
    }

    private void SwitchActiveCharacter(CharacterInfo character)
    {
        if (_activeCharacterInfo == character)
            return;

        _activeCharacterInfo = character;
        var secretCollection = _secretCollections.First(x => x.Character == character);

        var secretList = GetComponentInChildren<UISecretList>(true);
        secretList.Initialize(secretCollection);
    }
}