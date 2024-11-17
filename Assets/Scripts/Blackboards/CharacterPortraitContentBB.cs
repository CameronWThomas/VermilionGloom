using System;
using System.Collections.Generic;
using UnityEngine;

public class CharacterPortraitContentBB : GlobalSingleInstanceMonoBehaviour<CharacterPortraitContentBB>
{
    private Dictionary<CharacterID, Color> _portraitColorDict = new();

    public void Register(CharacterID id)
    {
        // TODO I don't think we will always do this, but for now we are just going to choose random colors
        _portraitColorDict.Add(id, new Color(RandomColorComponent(), RandomColorComponent(), RandomColorComponent()));
    }   

    public Texture2D GetPortrait(CharacterID characterID)
        => null;

    public Color GetPortraitColor(CharacterID characterID) => _portraitColorDict[characterID];

    private float RandomColorComponent()
    {
        return UnityEngine.Random.Range(0f, 1f);
    }
}