using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PortraitTaker : MonoBehaviour
{
    [SerializeField] Transform _cameraPosition;
    [SerializeField] Transform _playerPosition;
    [SerializeField] MeshRenderer _background;
    [SerializeField, Range(0f, 179f)] float _cameraFOV = 24f;
    [SerializeField] Camera _portraitCamera;
    [SerializeField] List<Texture2D> _backgroundTextures = new();

    public Texture2D TakePicture(CharacterInfo characterInfo)
    {
        var originalPosition = characterInfo.transform.position;
        var originalRotation = characterInfo.transform.rotation;

        characterInfo.transform.position = _playerPosition.position;
        characterInfo.transform.rotation = _playerPosition.rotation;

        var portraitColor = new Color(RandomColorComponent(), RandomColorComponent(), RandomColorComponent());
        _background.material.color = portraitColor;

        var backgroundTexture = _backgroundTextures.Randomize().First();
        _background.material.mainTexture = backgroundTexture;


        _portraitCamera.transform.position = _cameraPosition.position;
        _portraitCamera.transform.rotation = _cameraPosition.rotation;
        _portraitCamera.fieldOfView = _cameraFOV;

        var renderTexture = new RenderTexture(500, 750, 32);
        _portraitCamera.targetTexture = renderTexture;
        var oldRenderTexture = RenderTexture.active;
        RenderTexture.active = renderTexture;

        _portraitCamera.Render();

        Texture2D portrait = new Texture2D(500, 750);
        portrait.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        portrait.Apply();

        RenderTexture.active = oldRenderTexture;

        characterInfo.transform.position = originalPosition;
        characterInfo.transform.rotation = originalRotation;

        return portrait;
    }

    private float RandomColorComponent()
    {
        return UnityEngine.Random.Range(0f, 1f);
    }
}