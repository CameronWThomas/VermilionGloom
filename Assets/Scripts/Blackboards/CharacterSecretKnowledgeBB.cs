using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class CharacterSecretKnowledgeBB : GlobalSingleInstanceMonoBehaviour<CharacterSecretKnowledgeBB>
{
    private const int INITIAL_RUMOUR_SECRETS_MAX = 3;

    [SerializeField] private int _initialNPCCount = 4;
    [SerializeField, Range(0f, 1f)] public float _initialMurderChance = .25f;
    [SerializeField, Range(0f, 1f)] public float _randomNameChance = .1f;

    private Dictionary<CharacterID, CharacterSecretKnowledge> _secretKnowledgeDict = new();
    private bool _allInitialNPCsRegistered = false;
    private bool _initializedInitialSecrets = false;

    protected override void Start()
    {
        base.Start();
        UnityEngine.Random.InitState((int)DateTime.UtcNow.Ticks);
    }

    private void Update()
    {
        if (!_initializedInitialSecrets && _allInitialNPCsRegistered && _secretKnowledgeDict.Count == CharacterInfoBB.Instance.CharacterCount)
        {
            _initializedInitialSecrets = true;
            CharacterInfoBB.Instance.Initialize();
            InitializeSecretsAndRumours();
        }
    }

    public void Register(CharacterID characterID, CharacterSecretKnowledge secretKnowledge)
    {
        _secretKnowledgeDict.Add(characterID, secretKnowledge);

        if (_secretKnowledgeDict.Count >= _initialNPCCount)
            _allInitialNPCsRegistered = true;
    }

    public IReadOnlyList<Secret> GetSecrets(CharacterID characterId) => _secretKnowledgeDict[characterId].Secrets.ToList();

    /// <summary>
    /// Chooses a random secret for <paramref name="characterId"/> of level <paramref name="level"/> to reveal
    /// </summary>
    public void UnlockSecret(CharacterID characterId, SecretLevel level)
    {
        var secretKnowledge = _secretKnowledgeDict[characterId];
        var unlockedSecret = secretKnowledge.Secrets
            .Where(x => x.Level == level)
            .Randomize()
            .FirstOrDefault();

        unlockedSecret?.Reveal();
    }

    private void InitializeSecretsAndRumours()
    {
        // Create personal secrets (as in secrets where the character is the owner)
        foreach (var keyPair in _secretKnowledgeDict)
        {
            var secretKnowledge = keyPair.Value;
            var characterInfo = CharacterInfoBB.Instance.GetCharacterInfo(keyPair.Key);

            var secrets = CreatePersonalCharacterSecrets(characterInfo);
            secretKnowledge.AddSecrets(secrets);
        }

        SpreadSecretsAcrossAllCharacters(3);
    }

    private List<Secret> CreatePersonalCharacterSecrets(CharacterInfo characterInfo) => characterInfo.Initialize()
        .TryCreateNameSecrets()
        .TryCreateVampreSecrets()
        .TryCreateMurderSecrets()
        .TryCreateRoomSecrets()
        .CreateGenericSecrets(3)
        .BuildSecretList();

    private void SpreadSecretsAcrossAllCharacters(int otherCharactersSecretsPerCharacter)
    {
        foreach (var keyPair in _secretKnowledgeDict)
        {
            var characterID = keyPair.Key;
            var secrets = GetOtherCharactersSecrets(characterID, otherCharactersSecretsPerCharacter);

            keyPair.Value.AddSecrets(secrets);
        }
    }

    private IEnumerable<Secret> GetOtherCharactersSecrets(CharacterID characterID, int otherCharactersSecretsPerCharacter)
    {
        // Select only others secrets that don't have us as the owner and randomize the order
        var secretKnowledges = _secretKnowledgeDict
            .Where(x => x.Key != characterID)
            .Select(x => x.Value)
            .SelectMany(x => x.Secrets)
            .Where(x => x.SecretOwner != characterID)
            .Randomize()
            .ToList();

        var returnCount = 0;
        var returnMax = Mathf.Min(otherCharactersSecretsPerCharacter, secretKnowledges.Count);

        // Keep looping through the secrets selecting by chance
        while (true)
            foreach (var secret in secretKnowledges)
            {
                if (secret.Level.RandomChance())
                {
                    var secretCopy = secret.Copy();
                    yield return secretCopy;
                    returnCount++;
                }

                if (returnCount >= returnMax)
                    yield break;
            }
    }    
}