using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(CharacterInfo))]
public class NpcBrain : MonoBehaviour
{
    private List<Secret> _personalSecrets = new();
    private List<OthersSecrets> _othersSecretsCollection = new();

    //TODO remove later
    private bool _reputationInitialized = false;

    public List<Secret> Secrets => _personalSecrets;
    public List<OthersSecrets> OthersSecretsCollection => _othersSecretsCollection;
    public bool IsAnySecretRevealed => Secrets.Any(x => x.IsRevealed);

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _personalSecrets.Clear();

        // TODO all the same secrets for now. Will be randomly populated and this will be done somewhere else

        _personalSecrets.Add(new GenericSecret());
        _personalSecrets.Add(new GenericSecret());
        _personalSecrets.Add(new GenericSecret());
        _personalSecrets.Add(new GenericSecret());
        _personalSecrets.Add(new RoomSecret("Back left room connected to foyer", SecretLevel.Private));
        _personalSecrets.Add(MurderSecret.PersonalMurder(SecretLevel.Confidential));
        _personalSecrets.Add(new VampireSecret());
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
                    new GenericSecret()
                };

                _othersSecretsCollection.Add(new OthersSecrets(characterInfo, secrets));
            }
        }
    }
}
