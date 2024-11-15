using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class UISecretList : MonoBehaviour
{
    [SerializeField] private GameObject _secretTilePrefab;

    private List<UISecretTile> _secretTiles = new();

    public void Initialize(SecretCollection secretCollection)
    {
        _secretTiles.ForEach(x => Destroy(x));
        _secretTiles.Clear();

        foreach (var secret in secretCollection.Secrets)
        {
            var secretTile = Instantiate(_secretTilePrefab, transform).GetComponent<UISecretTile>();
            _secretTiles.Add(secretTile);
            secretTile.Initialize(secret);
        }
    }
}