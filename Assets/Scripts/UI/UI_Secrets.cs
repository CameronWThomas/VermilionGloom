using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UI_Secrets : MonoBehaviour
{
    [SerializeField] private GameObject _secretPrefab;

    private List<UI_Secret> _secrets = new();

    private ScrollRect ScrollRect => GetComponent<ScrollRect>();

    public void ResetSecrets()
    {
        _secrets.ForEach(x => Destroy(x.gameObject));
        _secrets.Clear();
    }

    public void AddSecrets(Secrets secrets)
    {
        var content = ScrollRect.content;

        // TODO only show revealed secrets
        foreach (var secret in secrets.SecretCollection)
        {
            if (secret is GenericSecret)
                continue;

            var uISecret = Instantiate(_secretPrefab, content).GetComponent<UI_Secret>();
            _secrets.Add(uISecret);

            uISecret.Initialize(secret);
        }
            }
}