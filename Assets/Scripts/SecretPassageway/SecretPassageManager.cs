using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SecretPassageManager : GlobalSingleInstanceMonoBehaviour<SecretPassageManager>
{
    [SerializeField] private float _enterSecretPassageTime = 2f;
    [SerializeField] private List<SecretPassageConnection> _forcedConnections;
    [SerializeField] private List<SecretPassageConnection> _forbiddenConnections;

    private List<SecretPassage> _secretPassages = new();

    public float EnterSecretPassageTime => _enterSecretPassageTime;

    protected override void Start()
    {
        base.Start();

        _secretPassages = FindObjectsByType<SecretPassage>(FindObjectsSortMode.None).ToList();
        if (!_secretPassages.Any())
            Debug.LogWarning($"Unable to find any {nameof(SecretPassage)}s");

        InitializeSecretPassageWays();
    }

    private void InitializeSecretPassageWays()
    {
        var secretPassages = new List<SecretPassage>(_secretPassages);
        
        // Add forced connections
        foreach (var passageConnection in _forcedConnections)
        {
            if (passageConnection.Passage1 == passageConnection.Passage2)
            {
                Debug.LogWarning($"Unable to create a forced connection through the same passage ({passageConnection.Passage1.name})");
                continue;
            }

            var passage1 = secretPassages.FirstOrDefault(x => x == passageConnection.Passage1);
            var passage2 = secretPassages.FirstOrDefault(x => x == passageConnection.Passage2);

            if (passage1 == null || passage2 == null)
            {
                Debug.LogWarning($"Unable to create a forced connection for {passageConnection.Passage1.name} and {passageConnection.Passage2.name} because one was used in an earlier forced connection");
                continue;
            }

            SecretPassage.ExchangeEndPoints(passage1, passage2);
            
            secretPassages.Remove(passageConnection.Passage1);
            secretPassages.Remove(passageConnection.Passage2);
        }

        // Make special passages only connect to normal ones
        var secretPassagewaysOfSpecialType = secretPassages.Where(x => x.SecretPassageType != SecretPassageType.Normal).ToList();
        foreach (var specialPassage in secretPassagewaysOfSpecialType)
        {
            var normalPassage = GetRandom(secretPassages.Where(x => x.SecretPassageType == SecretPassageType.Normal).ToList());
            secretPassages.Remove(normalPassage);
            secretPassages.Remove(specialPassage);

            SecretPassage.ExchangeEndPoints(normalPassage, specialPassage);
        }

         
        // Sort secret passages to start with forbidden paths
        List<SecretPassage> allWithForbiddenPartners = new List<SecretPassage>();
        allWithForbiddenPartners.AddRange(_forbiddenConnections.Select(x => x.Passage1));
        allWithForbiddenPartners.AddRange(_forbiddenConnections.Select(x => x.Passage2));
        allWithForbiddenPartners = allWithForbiddenPartners.Distinct().ToList();

        secretPassages.Sort((x, y) =>
        {
            if (allWithForbiddenPartners.Contains(x) && !allWithForbiddenPartners.Contains(y))
                return 1;
            if (!allWithForbiddenPartners.Contains(x) && allWithForbiddenPartners.Contains(y))
                return -1;
            return 0;
        });


        
        // Handle rest of secret passages
        while (secretPassages.Any())
        {
            var passage1 = GetRandom(secretPassages);
            secretPassages.Remove(passage1);
            
            // get forbidden connections for passage 1
            var verboten = _forbiddenConnections.Where(x => x.Passage1 == passage1).Select(x => x.Passage2).ToList();
            verboten.AddRange(_forbiddenConnections.Where(x => x.Passage2 == passage1).Select(x => x.Passage1).ToList());

            Debug.Log("root passage: " + passage1.name);
            Debug.Log("Forbidden connections: " + string.Join(", ", verboten.Select(x => x.name)));

            var passage2 = GetRandom(secretPassages, verboten);

            // If odd number, we will return
            if (passage2 == null)
            {
                Debug.LogWarning($"{passage1.name} has no secret passage connection (odd number of {nameof(SecretPassage)}s)");
                break;
            }

            secretPassages.Remove(passage2);

            SecretPassage.ExchangeEndPoints(passage1, passage2);
        }
    }

    private static T GetRandom<T>(List<T> secretPassages, List<T> forbidden = null) where T : class
    {
        if (!secretPassages.Any())
            return null;
            
        var count = secretPassages.Count();
        var index = UnityEngine.Random.Range(0, count);
        
        var targetPassage = secretPassages[index];
        if (forbidden != null && forbidden.Contains(targetPassage))
        {
            var newSecretPassages = new List<T>(secretPassages);
            newSecretPassages.Remove(targetPassage);

            // if this stack overflows ill eat my hat
            return GetRandom(newSecretPassages, forbidden);
        }
        return targetPassage;
        
    }

    [Serializable]
    private class SecretPassageConnection
    {
        [SerializeField] public SecretPassage Passage1;
        [SerializeField] public SecretPassage Passage2;
    }
}