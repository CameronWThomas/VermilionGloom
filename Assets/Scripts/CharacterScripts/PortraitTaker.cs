using System;
using UnityEngine;

public class PortraitTaker : MonoBehaviour
{
    [SerializeField] Transform _cameraPosition;
    [SerializeField] Transform _playerPosition;
    [SerializeField] MeshRenderer _background;
    [SerializeField, Range(0f, 179f)] float _cameraFOV = 24f;

    [Header("Remove")]
    public Camera _portraitCamera;
    public UI_Portrait _portrait;
    public CharacterInfo _characterInfo;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
            UpdatePicture(_portraitCamera, _background.material, _characterInfo);
    }

    private void UpdatePicture(Camera portraitCamera, Material bgMaterial, CharacterInfo characterInfo)
    {
        characterInfo.transform.position = _playerPosition.position;
        characterInfo.transform.rotation = _playerPosition.rotation;


        var portraitColor = new Color(RandomColorComponent(), RandomColorComponent(), RandomColorComponent());
        bgMaterial.color = portraitColor;


        portraitCamera.transform.position = _cameraPosition.position;
        portraitCamera.transform.rotation = _cameraPosition.rotation;
        _portraitCamera.fieldOfView = _cameraFOV;

        var renderTexture = new RenderTexture(500, 750, 32);
        portraitCamera.targetTexture = renderTexture;
        var oldRenderTexture = RenderTexture.active;
        RenderTexture.active = renderTexture;

        portraitCamera.Render();

        Texture2D portrait = new Texture2D(500, 750);
        portrait.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        portrait.Apply();

        CharacterPortraitContentBB.Instance.AddPortrait(characterInfo.ID, portrait);

        _portrait.SetCharacter(characterInfo.ID);
        RenderTexture.active = oldRenderTexture;
    }

    private float RandomColorComponent()
    {
        return UnityEngine.Random.Range(0f, 1f);
    }
}