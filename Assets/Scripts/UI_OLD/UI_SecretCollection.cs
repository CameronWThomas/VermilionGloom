using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UI_SecretCollection : MonoBehaviour
{
    public const float BUFFER = 10f;

    private const float TILE_POSITION_MODIFIER = UI_SecretTile.SIDE_LENGTH / 2f + UI_SecretTile.BUFFER;

    [SerializeField] private GameObject _secretTilePrefab;

    private bool _respaceContent = false;
    private bool _contentHasBeenRespaced = false;
    private List<UI_SecretTile> _secretTiles = new();
    private SecretCollection _secretCollection;

    private RectTransform RectTransform => GetComponent<RectTransform>();
    private float MinHeight => IsPortraitActive ? UI_Portrait.HEIGHT : UI_SecretTile.TOTAL + UI_SecretTile.BUFFER;
    private UI_Portrait Portrait => transform.GetChild(0).GetComponent<UI_Portrait>();
    private bool IsPortraitActive => Portrait.isActiveAndEnabled;

    public bool ContentHasBeenRespaced
    {
        get
        {
            if (_contentHasBeenRespaced)
            {
                _contentHasBeenRespaced = false;
                return true;
            }

            return false;
        }
    }
    

    private void Update()
    {
        if (_respaceContent)
        {
            SpaceContent();
            _respaceContent = false;
        }
    }

    public void Initialize(SecretCollection secretCollection)
    {
        _secretCollection = secretCollection;
        Portrait.gameObject.SetActive(!_secretCollection.IsPersonalSecrets);

        _secretTiles.ForEach(secret => Destroy(secret.gameObject));
        _secretTiles.Clear();

        foreach (var revealedSecret in secretCollection.Secrets.Where(x => !x.IsRevealed))
        {
            var secretTile = Instantiate(_secretTilePrefab, transform).GetComponent<UI_SecretTile>();
            secretTile.Initialize(revealedSecret);
            _secretTiles.Add(secretTile);
        }

        // Needed because the first time it opens, the RectTransform won't be initialized with enough info we need (I think it needs an Update to be called and that may not happen yet)
        _respaceContent = true;
    }

    private void SpaceContent()
    {
        var totalWidth = RectTransform.rect.width;
        var minHeight = MinHeight;




        var isPortraitActive = IsPortraitActive;
        var xIndex = -1;
        var yIndex = -1;
        var tilesPerRow = int.MinValue;
        foreach (var secretTile in _secretTiles)
        {
            if (xIndex + 1 > tilesPerRow)
            {
                yIndex++;
                GetTileRowVariables(yIndex, totalWidth, isPortraitActive, out tilesPerRow, out xIndex);
            }

            var x = GetTilePosition(xIndex++);
            var y = GetTilePosition(yIndex);

            var rectTransform = secretTile.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = new Vector2(x, -y);
        }

        var height = Mathf.Max(minHeight, UI_SecretTile.TOTAL * (yIndex + 1) + UI_SecretTile.BUFFER);
        RectTransform.sizeDelta = new Vector2(RectTransform.sizeDelta.x, height);

        var yPosition = GetYPosition(height);
        RectTransform.anchoredPosition = new Vector2(0f, yPosition);

        _contentHasBeenRespaced = true;
    }

    private float GetTilePosition(int index) => index * UI_SecretTile.TOTAL + TILE_POSITION_MODIFIER;

    private void GetTileRowVariables(int yIndex, float totalWidth, bool isPortraitActive,
        out int tilesPerRow, out int startingXIndex)
    {
        startingXIndex = 0;
        if (isPortraitActive && IsPortraitRow(yIndex))
            startingXIndex = GetPortraitRowStartingXIndex();

        var width = totalWidth - startingXIndex * UI_SecretTile.TOTAL;

        // Get tiles before extra buffer at end
        tilesPerRow = Mathf.FloorToInt(width / UI_SecretTile.TOTAL);

        // Check if there is enough space for buffer at end. If not, remove one tile per row
        if (width - tilesPerRow * UI_SecretTile.TOTAL < UI_SecretTile.BUFFER)
            tilesPerRow--;
    }

    private bool IsPortraitRow(int yIndex)
    {
        var yPosition = GetTilePosition(yIndex);
        return yPosition < UI_Portrait.HEIGHT + UI_SecretTile.BUFFER;
    }

    private int GetPortraitRowStartingXIndex()
    {
        // This is an incredibly lazy and inefficient way to figure this out, but screw it
        for (var i = 0; i < 500; i++)
        {
            if (GetTilePosition(i) > UI_Portrait.WIDTH)
                return i;
        }

        return 0;
    }

    private float GetYPosition(float height)
    {
        // Find the last child before us to figure out our spot
        var lastChildIndex = -1;
        for (var i = 0; i < transform.parent.childCount; i++)
        {
            if (transform.parent.GetChild(i) == transform)
                break;

            lastChildIndex = i;
        }

        // If we are the first child, we will treat our y_zero as 0
        var y_zero = 0f;
        if (lastChildIndex >= 0)
        {
            var lastChildRectTransform = transform.parent.GetChild(lastChildIndex).GetComponent<RectTransform>();
            y_zero = lastChildRectTransform.anchoredPosition.y - lastChildRectTransform.rect.height / 2f - BUFFER;
        }

        return y_zero - height / 2f;
    }
}
