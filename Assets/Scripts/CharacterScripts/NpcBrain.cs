using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(CharacterInfo))]
public class NpcBrain : MonoBehaviour
{
    private SecretCollection _personalSecrets;
        private List<SecretCollection> _othersSecretsCollection = new();

    //TODO remove later
    private bool _reputationInitialized = false;

    public SecretCollection PersonalSecrets => _personalSecrets;
    public List<SecretCollection> OthersSecretCollections => _othersSecretsCollection;
    public bool IsAnySecretRevealed => PersonalSecrets.IsAnySecretsRevealed;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // TODO all the same secrets for now. Will be randomly populated and this will be done somewhere else
        var secrets = new List<Secret>
        {
            new GenericSecret(),
            new GenericSecret(),
            new GenericSecret(),
            new RoomSecret("Back left room connected to foyer", SecretLevel.Private),
            MurderSecret.PersonalMurder(SecretLevel.Confidential),
            new VampireSecret()
        };

        _personalSecrets = SecretCollection.CreatePersonal(secrets);
    }

    // Update is called once per frame
    void Update()
    {
        if (!_reputationInitialized)
        {
            _reputationInitialized = true;

            foreach (var characterInfo in FindObjectsByType<CharacterInfo>(FindObjectsSortMode.None))
            {
                if (characterInfo == GetComponent<CharacterInfo>())
                    continue;

                var secrets = new List<Secret>
                {
                    new GenericSecret(),
                    new GenericSecret(),
                    new GenericSecret()
                };

                _othersSecretsCollection.Add(SecretCollection.Create(characterInfo, secrets));
            }
        }
    }
}
