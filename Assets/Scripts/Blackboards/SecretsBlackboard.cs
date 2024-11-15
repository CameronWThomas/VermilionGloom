using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class SecretsBlackboard : GlobalSingleInstanceMonoBehaviour<SecretsBlackboard>
{
    private const int INITIAL_RUMOUR_SECRETS_MAX = 3;

    [SerializeField] private int _initialNPCCount = 4;
    [SerializeField, Range(0f, 1f)] private float _initialMurderChance = .25f;
    [SerializeField, Range(0f, 1f)] private float _randomNameChance = .1f;

    private Dictionary<CharacterInfo, SecretKnowledge> _secretKnowledgeDict = new();

    protected override void Start()
    {
        base.Start();
        UnityEngine.Random.InitState((int)DateTime.UtcNow.Ticks);
    }

    public void Register(SecretKnowledge secretKnowledge)
    {
        var characterInfo = secretKnowledge.GetComponent<CharacterInfo>();
        _secretKnowledgeDict.Add(characterInfo, secretKnowledge);

        if (_secretKnowledgeDict.Count >= _initialNPCCount)
            InitializeSecretsAndRumours();
    }

    private void InitializeSecretsAndRumours()
    {
        // Create secrets
        foreach (var keyPair in _secretKnowledgeDict)
        {
            var character = keyPair.Key;
            var secretKnowledge = keyPair.Value;
            
            var secrets = CreateCharacterSecrets(character);
            secretKnowledge.AddSecrets(secrets);
        }

        // Create rumours
        foreach (var keyPair in _secretKnowledgeDict)
        {
            var character = keyPair.Key;
            var secretKnowledge = keyPair.Value;

            // Choose up to 2 random people that aren't the current character
            var addedSecretsCount = 0;
            var rumours = new List<Rumour>();
            var allowableTargets = _secretKnowledgeDict.Keys.Where(x => x != character).ToList();
            while (true)
            {
                if (!allowableTargets.Any())
                    break;

                // Choose a count between 1 and INITIAL_RUMOUR_SECRETS_MAX minus how many secrets we've already made
                var secretsForPerson = UnityEngine.Random.Range(1, INITIAL_RUMOUR_SECRETS_MAX + 1 - addedSecretsCount);
                var targetCharacter = allowableTargets[UnityEngine.Random.Range(0, allowableTargets.Count)];

                addedSecretsCount += secretsForPerson;
                allowableTargets.Remove(targetCharacter);

                var secrets = CreateRandomTargetSecrets(targetCharacter, secretsForPerson);
                rumours.Add(new Rumour(new SecretCollection(secrets), targetCharacter));

                if (addedSecretsCount >= INITIAL_RUMOUR_SECRETS_MAX)
                    break;
            }

            foreach (var rumour in rumours)
                secretKnowledge.AddRumour(rumour);
        }
    }

    private List<Secret> CreateRandomTargetSecrets(CharacterInfo character, int secretCount)
    {
        var secrets = new List<Secret>();

        var charactersSecretKnowledge = _secretKnowledgeDict[character];

        var secretsRandomOrder = RandomOrder(charactersSecretKnowledge.Secrets.Secrets);
        foreach (var secret in secretsRandomOrder)
        {
            if (secretCount <= 0)
                return secrets;

            if (!RandomChance(secret.Level.Chance()))
                continue;

            secrets.Add(secret);
            secretCount--;
        }

        if (ChanceAddMurderSecret(ref secrets, character))
            secretCount--;
        if (ChanceAddSecretName(ref secrets))
            secretCount--;
        
        if (secretCount > 0)
            secrets.AddRange(GenericSecret.CreateUnique(secretCount));

        return secrets;
    }    

    private SecretCollection CreateCharacterSecrets(CharacterInfo character)
    {
        var secrets = new List<Secret>();

        switch (character.CharacterType)
        {
            case CharacterType.VanHelsing:
                secrets.Add(new NameSecret("Van Helsing", SecretLevel.Confidential));
                secrets.Add(new VampireSecret());
                break;

            case CharacterType.Owner:
                secrets.Add(new RoomSecret("back left room connected to foyer"));
                ChanceAddSecretName(ref secrets);
                break;

            case CharacterType.Generic:
                ChanceAddSecretName(ref secrets);
                break;
        }

        ChanceAddMurderSecret(ref secrets);        

        secrets.AddRange(GenericSecret.CreateUnique(3));

        return new SecretCollection(secrets);
    }

    private bool ChanceAddMurderSecret(ref List<Secret> secrets, CharacterInfo murderer = null)
    {
        if (RandomChance(_initialMurderChance))
        {
            var level = RandomChance(_initialMurderChance) ? SecretLevel.Public : SecretLevel.Confidential;
            var secret = murderer == null
                ? MurderSecret.PersonalMurder(level)
                : MurderSecret.OtherMurder(level, murderer);

            secrets.Add(secret);
            return true;
        }

        return false;
    }

    private bool ChanceAddSecretName(ref List<Secret> secrets)
    {
        if (RandomChance(_randomNameChance))
        {
            var level = RandomChance(_randomNameChance) ? SecretLevel.Public : SecretLevel.Private;

            // TODO have some name list to go to instead
            secrets.Add(new NameSecret(Guid.NewGuid().ToString()));
            return true;
        }

        return false;
    }

    private bool RandomChance(float chance)
    {
        var value = UnityEngine.Random.Range(0f, 1f);
        return value <= chance;
    }

    // TODO Move to helper class
    private IEnumerable<T> RandomOrder<T>(IEnumerable<T> enumerable)
    {
        var originalList = enumerable.ToList();
        var randomList = new List<T>();

        while (originalList.Any())
        {
            var randomItem = originalList[UnityEngine.Random.Range(0, originalList.Count)];
            randomList.Add(randomItem);
            originalList.Remove(randomItem);
        }

        return randomList;
    }
}