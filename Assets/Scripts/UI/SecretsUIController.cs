using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using UnityEngine;

public class SecretsUIController : MonoBehaviour
{
    public const float SECRET_TILE_WIDTH = 100F;
    public const float SECRET_TILE_BUFFER = 10F;

    private List<Secret> _secrets = new List<Secret>();

    private RectTransform _thisRectTransform;

    public void Initialize(List<Secret> secrets)
    {
        _secrets = secrets;
        _thisRectTransform = GetComponent<RectTransform>();

        foreach (var secret in _secrets.Where(x => x.IsRevealed))
        {
            //TODO create a secret ui part
        }
    }

    public Vector2 GetPosition(int index)
    {
        var width = _thisRectTransform.rect.width;
        var tileSpace = SECRET_TILE_WIDTH + SECRET_TILE_BUFFER;
        var tilesPerRow = Mathf.FloorToInt(width / tileSpace);

        var y = Mathf.FloorToInt(index / tilesPerRow);

        var xIndex = index - y * tilesPerRow;
        var x = tileSpace * xIndex + SECRET_TILE_WIDTH / 2f + SECRET_TILE_BUFFER;

        return new Vector2(x, y);
    }
}