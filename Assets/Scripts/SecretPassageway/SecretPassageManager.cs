using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SecretPassageManager : GlobalSingleInstanceMonoBehaviour<SecretPassageManager>
{
    [SerializeField] private float _enterSecretPassageTime = 2f;

    private List<SecretPassage> _secretPassages = new();

    public float EnterSecretPassageTime => _enterSecretPassageTime;

    protected override void Start()
    {
        base.Start();

        _secretPassages = FindObjectsByType<SecretPassage>(FindObjectsSortMode.None).ToList();
        if (!_secretPassages.Any())
            Debug.LogWarning($"Unable to find any {nameof(SecretPassage)}s");
        if (_secretPassages.Count() % 2 != 0)
            Debug.LogWarning($"There is an odd number of {nameof(SecretPassage)}s");


        //var secretPassages = new List<SecretPassage>(_secretPassages);
        //SecretPassage lastSecretPassage = null;
        //while (_secretPassages.Any())
        //{
        //    var passage1 = GetRandom(secretPassages);
        //}
    }

    //private T GetRandom<T>(List<T> secretPassages) where T : class
    //{
    //    if (secretPassages.Any())

    //    var count = secretPassages.Count();
    //    var random = new Random()
    //}
}