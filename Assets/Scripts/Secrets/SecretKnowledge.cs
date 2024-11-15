using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(CharacterInfo))]
public class SecretKnowledge : MonoBehaviour
{
    private List<Rumour> _rumours = new();

    public SecretCollection Secrets { get; private set; }
    public IReadOnlyList<Rumour> Rumours => _rumours.ToList();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SecretsBlackboard.Instance.Register(this);
    }

    public void AddSecrets(SecretCollection secrets)
    {
        Secrets = secrets;
    }

    public void AddRumour(Rumour rumour)
    {
        if (Rumours.Any(x => x.RumourTarget == rumour.RumourTarget))
        {
            Debug.LogWarning($"This character already has rumours");
            return;
        }    

        _rumours.Add(rumour);
    }

    //void Start()
    //{
    //    // TODO all the same secrets for now. Will be randomly populated and this will be done somewhere else
    //    var secrets = new List<Secret>
    //    {
    //        new GenericSecret(),
    //        new GenericSecret(),
    //        new GenericSecret(),
    //        new RoomSecret("Back left room connected to foyer", SecretLevel.Private),
    //        MurderSecret.PersonalMurder(SecretLevel.Confidential),
    //        new VampireSecret()
    //    };

    //    _secrets = Secrets.CreatePersonal(secrets);
    //}

    //void Update()
    //{
    //    if (!_rumoirsInitialized)
    //    {
    //        _rumoirsInitialized = true;

    //        foreach (var characterInfo in FindObjectsByType<CharacterInfo>(FindObjectsSortMode.None))
    //        {
    //            if (characterInfo == GetComponent<CharacterInfo>())
    //                continue;

    //            var secrets = new List<Secret>
    //            {
    //                new GenericSecret(),
    //                new GenericSecret(),
    //                new GenericSecret(),
    //                MurderSecret.OtherMurder(SecretLevel.Confidential, characterInfo),
    //            };

    //            _rumours.Add(Secrets.Create(characterInfo, secrets));
    //        }
    //    }
    //}
}
