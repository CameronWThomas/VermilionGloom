using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UI_SecretCollections : MonoBehaviour
{
    public const float BUFFER = 10F;

    [SerializeField] private ScrollRect _secretCollectionsScrollRect;
    [SerializeField] private GameObject _secretCollectionPrefab;

    private UI_SecretCollection _personalSecretCollection;

    private List<UI_SecretCollection> _secretCollections = new();

    private void Update()
    {
        var fixScrollContentHeight = false;
        foreach (var secretCollection in _secretCollections)
            fixScrollContentHeight |= secretCollection.ContentHasBeenRespaced;

        if (fixScrollContentHeight)
            FixScrollContentHeight();
        
    }

    public void Initialize(IEnumerable<SecretCollection> secretCollections)
    {
        _secretCollections.ForEach(secret => Destroy(secret.gameObject));
        _secretCollections.Clear();

        var personalSecretCollection = secretCollections.Single(x => x.IsPersonalSecrets);
        AddNewUISecretCollection(personalSecretCollection);

        foreach (var secretCollection in secretCollections.Where(x => !x.IsPersonalSecrets))
        {
            AddNewUISecretCollection(secretCollection);
        }
    }    

    private void AddNewUISecretCollection(SecretCollection secretCollection)
    {
        var uISecretCollection = Instantiate(_secretCollectionPrefab, _secretCollectionsScrollRect.content.transform)
            .GetComponent<UI_SecretCollection>();
        uISecretCollection.Initialize(secretCollection);
        _secretCollections.Add(uISecretCollection);
    }

    private void FixScrollContentHeight()
    {
        var contentRect = _secretCollectionsScrollRect.content.GetComponent<RectTransform>();

        // Set to center top align. Width should be fine, so only need to figure out the new height
        var height = 0f;
        var furthestRectTransform = _secretCollections
            .Select(x => x.GetComponent<RectTransform>())
            .OrderBy(x => x.anchoredPosition.y) // will be a negative value
            .FirstOrDefault();

        if (furthestRectTransform != null)
        {
            height = -furthestRectTransform.anchoredPosition.y
                + furthestRectTransform.rect.height / 2f
                + UI_SecretCollection.BUFFER + BUFFER;
        }

        var width = contentRect.rect.width;
        contentRect.sizeDelta = new Vector2(width, height);
    }
}
