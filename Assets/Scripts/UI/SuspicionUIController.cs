using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//TODO move to general place
public enum SuspicionLevel
{
    None = 0,
    One = 1,
    Two = 2,
    Max = 3
}

public class SuspicionUIController : MonoBehaviour
{
    [SerializeField] private RawImage _suspicionDial;

    //TODO I don't like having these here, should be some other thing in case other places need it.
    [SerializeField] private Texture2D _noSuspicionTexture;
    [SerializeField] private Texture2D _oneSuspicionTexture;
    [SerializeField] private Texture2D _twoSuspicionTexture;
    [SerializeField] private Texture2D _maxSuspicionTexture;

    private Dictionary<SuspicionLevel, Texture2D> _suspicionTextures = new();
    private SuspicionLevel _suspicionLevel = SuspicionLevel.None;

    private void Start()
    {
        _suspicionTextures.Add(SuspicionLevel.None, _noSuspicionTexture);
        _suspicionTextures.Add(SuspicionLevel.One, _oneSuspicionTexture);
        _suspicionTextures.Add(SuspicionLevel.Two, _twoSuspicionTexture);
        _suspicionTextures.Add(SuspicionLevel.Max, _maxSuspicionTexture);
    }

    public bool IncreaseSuspicion()
    {
        if (_suspicionLevel == SuspicionLevel.Max)
            return false;

        _suspicionLevel++;
        UpdateSuspicionImage();
        return true;
    }

    public bool DecreaseSuspicion()
    {
        if (_suspicionLevel == SuspicionLevel.None)
            return false;

        _suspicionLevel--;
        UpdateSuspicionImage();
        return true;
    }

    private void UpdateSuspicionImage()
    {
        var newSuspicionTexture = _suspicionTextures[_suspicionLevel];
        _suspicionDial.texture = newSuspicionTexture;
    }
}
