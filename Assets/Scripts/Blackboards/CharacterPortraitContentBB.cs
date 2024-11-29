using System;
using System.Collections.Generic;
using UnityEngine;

public class CharacterPortraitContentBB : GlobalSingleInstanceMonoBehaviour<CharacterPortraitContentBB>
{
    private Dictionary<CharacterID, Texture2D> _portraitDict = new();

    public void AddPortrait(CharacterID id, Texture2D portrait)
    {
        if (_portraitDict.ContainsKey(id))
            _portraitDict[id] = portrait;
        else
            _portraitDict.Add(id, portrait);
    }

    public Texture2D GetPortrait(CharacterID characterID) => _portraitDict.ContainsKey(characterID) ? _portraitDict[characterID] : null;
}