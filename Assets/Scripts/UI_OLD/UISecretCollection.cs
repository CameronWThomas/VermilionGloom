using NUnit.Framework;
using System.Collections.Generic;
using System.Security;
using UnityEngine;

public class UISecretCollection : MonoBehaviour
{
    [SerializeField] private GameObject _secretTilesSection;
    [SerializeField] private GameObject _secretTilePrefab;

    private List<UI_SecretTile> _secretTiles = new();

    public void Initialize(SecretCollection secretCollection)
    {
        _secretTiles.ForEach(x => Destroy(x.gameObject));
        _secretTiles.Clear();

        foreach (var secret in secretCollection.Secrets)
        {
            var secretTile = Instantiate(_secretTilePrefab, _secretTilesSection.transform).GetComponent<UI_SecretTile>();
            _secretTiles.Add(secretTile);
            secretTile.Initialize(secret);
        }
    }
}