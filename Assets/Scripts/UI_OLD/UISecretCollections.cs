using System.Collections.Generic;
using UnityEngine;

public class UISecretCollections : MonoBehaviour
{
    [SerializeField] private RectTransform _secretCollectionSection;
    [SerializeField] private GameObject _uISecretCollectionPrefab;

    private List<UISecretCollection> _uISecretCollections = new();

    public void Initialize(List<SecretCollection> secretCollections)
    {
        _uISecretCollections.ForEach(x => Destroy(x));
        _uISecretCollections.Clear();        

        foreach (var secretCollection in secretCollections)
        {
            var uiSecretCollection = Instantiate(_uISecretCollectionPrefab, _secretCollectionSection).GetComponent<UISecretCollection>();
            _uISecretCollections.Add(uiSecretCollection);
            uiSecretCollection.Initialize(secretCollection);
        }
    }
}