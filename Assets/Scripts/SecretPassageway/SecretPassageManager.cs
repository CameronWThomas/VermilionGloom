using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SecretPassageManager : GlobalSingleInstanceMonoBehaviour<SecretPassageManager>
{
    [SerializeField] private float _enterSecretPassageTime = 2f;

    [SerializeField] private List<SecretPassageConnection> _forcedConnections;
    [SerializeField] private List<SecretPassageConnection> _blacklistedConnections;

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

            passage1.EndPoint = passage2;
            passage2.EndPoint = passage1;
            
            secretPassages.Remove(passageConnection.Passage1);
            secretPassages.Remove(passageConnection.Passage2);
        }

        // Make sure all blacklisted connections are enforced
        foreach (var passageConnection in _blacklistedConnections)
        {
            var passage1 = secretPassages.FirstOrDefault(x => x == passageConnection.Passage1);

            // Already had a forced connection or was used in handling an earlier blacklist, we can ignore.
            if (passage1 == null)
                continue;

            var relevantBlacklistedConnections = _blacklistedConnections.Where(x => x.Passage1 == passage1 || x.Passage2 == passage1);
            var acceptablePassages = secretPassages
                .Where(x => relevantBlacklistedConnections.All(blackListedConnection => blackListedConnection.Passage1 != x && blackListedConnection.Passage2 != x))
                .ToList();

            if (!acceptablePassages.Any())
            {
                Debug.LogWarning($"No acceptable connections were found for {passage1.name} based on the blacklisted ones and ones remaining after handling forced connections");
                continue;
            }

            var passage2 = GetRandom(acceptablePassages);
            passage1.EndPoint = passage2;
            passage2.EndPoint = passage1;

            secretPassages.Remove(passage1);
            secretPassages.Remove(passage2);
        }

        // Handle rest of secret passages
        while (secretPassages.Any())
        {
            var passage1 = GetRandom(secretPassages);
            secretPassages.Remove(passage1);

            var passage2 = GetRandom(secretPassages);

            // If odd number, we will return
            if (passage2 == null)
            {
                Debug.LogWarning($"{passage1.name} has no secret passage connection (odd number of {nameof(SecretPassage)}s)");
                break;
            }

            secretPassages.Remove(passage2);

            passage1.EndPoint = passage2;
            passage2.EndPoint = passage1;
        }
    }

    private static T GetRandom<T>(List<T> secretPassages) where T : class
    {
        if (!secretPassages.Any())
            return null;

            
        var count = secretPassages.Count();
        var random = new Unity.Mathematics.Random((uint)DateTime.UtcNow.Ticks);
        
        return secretPassages[random.NextInt(count - 1)];
    }

    [Serializable]
    private class SecretPassageConnection
    {
        [SerializeField] public SecretPassage Passage1;
        [SerializeField] public SecretPassage Passage2;
    }
}