using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterInfo))]
public class NpcBrain : MonoBehaviour
{
    private List<Secret> _personalSecrets = new();
    private List<CharacterReputation> _characterReputations = new();

    //TODO remove later
    private bool _reputationInitialized = false;

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

                var secrets = new List<Secret>();
                secrets.Add(new GenericSecret());
                secrets.Add(new GenericSecret());

                _characterReputations.Add(new CharacterReputation(characterInfo, secrets));
            }
        }
    }
}
