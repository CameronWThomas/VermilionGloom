using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(CharacterInfo))]
public class CharacterSecrets : MonoBehaviour
{
    private Secrets _secrets;
        private List<Secrets> _rumours = new();

    //TODO remove later
    private bool _rumoirsInitialized = false;

    public Secrets PersonalSecrets => _secrets;
    public List<Secrets> Rumours => _rumours;
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

        _secrets = Secrets.CreatePersonal(secrets);
    }

    // Update is called once per frame
    void Update()
    {
        if (!_rumoirsInitialized)
        {
            _rumoirsInitialized = true;

            foreach (var characterInfo in FindObjectsByType<CharacterInfo>(FindObjectsSortMode.None))
            {
                if (characterInfo == GetComponent<CharacterInfo>())
                    continue;

                var secrets = new List<Secret>
                {
                    new GenericSecret(),
                    new GenericSecret(),
                    new GenericSecret(),
                    MurderSecret.OtherMurder(SecretLevel.Confidential, characterInfo),                    
                };

                _rumours.Add(Secrets.Create(characterInfo, secrets));
            }
        }
    }
}
