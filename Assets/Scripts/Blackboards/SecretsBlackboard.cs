using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class SecretsBlackboard : GlobalSingleInstanceMonoBehaviour<SecretsBlackboard>
{
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
            InitializeSecrets();
    }

    private void InitializeSecrets()
    {
        foreach (var keyPair in _secretKnowledgeDict)
        {
            var character = keyPair.Key;
            var secretKnowledge = keyPair.Value;
            
            var secrets = CreateCharacterSecrets(character);
            secretKnowledge.AddSecrets(secrets);
        }
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

    private void ChanceAddMurderSecret(ref List<Secret> secrets)
    {
        if (RandomChance(_initialMurderChance))
        {
            var level = RandomChance(_initialMurderChance) ? SecretLevel.Public : SecretLevel.Confidential;
            secrets.Add(MurderSecret.PersonalMurder(level));
        }
    }

    private void ChanceAddSecretName(ref List<Secret> secrets)
    {
        if (RandomChance(_randomNameChance))
        {
            var level = RandomChance(_randomNameChance) ? SecretLevel.Public : SecretLevel.Private;

            // TODO have some name list to go to instead
            secrets.Add(new NameSecret(Guid.NewGuid().ToString()));
        }
    }

    private bool RandomChance(float chance)
    {
        var value = UnityEngine.Random.Range(0f, 1f);
        return value <= chance;
    }
}