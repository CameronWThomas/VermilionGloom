using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UI_Secrets : MonoBehaviour
{
    [SerializeField] private GameObject _secretPrefab;

    private List<Rumour> _rumours = new();
    private SecretCollection _secretCollection;
    private List<UI_Secret> _secrets = new();

    private int _revealedSecretsCount = 0;

    private ScrollRect ScrollRect => GetComponent<ScrollRect>();

    private void Update()
    {
        //if (GetRevealedSecretsCount() != _revealedSecretsCount)
        //{
        //    var secretCollection = _secretCollection;
        //    var rumours = _rumours.ToList();
        //    ResetSecrets();

        //    AddSecrets(secretCollection);
        //    AddRumours(rumours);
        //}
    }

    public void ResetSecrets()
    {
        _secrets.ForEach(x => Destroy(x.gameObject));
        _secrets.Clear();

        _secretCollection = null;
        _secrets.Clear();

        _revealedSecretsCount = 0;
    }

    public void AddSecrets(SecretCollection secrets)
    {
        _secretCollection = secrets;
        AddSecrets(secrets, false);
    }

    public void AddRumours(IEnumerable<Rumour> rumours)
    {
        _rumours.AddRange(rumours);

        // TODO more will be involved later
        foreach (var rumour in rumours)
        {
            AddSecrets(rumour.Secrets, true);
        }
    }

    private void AddSecrets(SecretCollection secrets, bool isRumour)
    {
        var content = ScrollRect.content;

        // TODO only show revealed secrets
        //foreach (var secret in secrets.Secrets.Where(x => x.IsRevealed))
        foreach (var secret in secrets.Secrets)
        {
            //if (secret is GenericSecret)
            //    continue;

            var uISecret = Instantiate(_secretPrefab, content).GetComponent<UI_Secret>();
            _secrets.Add(uISecret);

            uISecret.Initialize(secret, isRumour);
            _revealedSecretsCount++;
        }
    }

    private int GetRevealedSecretsCount()
    {
        var secretCollectionCount = _secretCollection?.Secrets.Count(x => x.IsRevealed) ?? 0;
        var rumourCount = _rumours.SelectMany(x => x.Secrets.Secrets).Count(x => x.IsRevealed);

        return secretCollectionCount + rumourCount;
    }
}