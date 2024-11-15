using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Secret with nothing of value
/// </summary>
public class GenericSecret : Secret
{
    private static readonly List<string> _secretDescriptions = new List<string>
    {
        "Never learned how to read",
        "Likes to eat dirt",
        "Regularlly forgets their name",
        "Is only here in the hopes of being murdered"
    };

    private SecretLevel _level;
    private string _description;

    public GenericSecret()
    {
        Func<bool> random = () => UnityEngine.Random.Range(0f, 1f) > .5f;

        _level = random() ? SecretLevel.Public : SecretLevel.Private;
        
        var secretCount = _secretDescriptions.Count;
        UnityEngine.Random.Range(0, secretCount);
        _description = _secretDescriptions[UnityEngine.Random.Range(0, secretCount)];

    }

    private GenericSecret(SecretLevel level, string description)
    {
        _level = level;
        _description = description;
    }

    public static List<GenericSecret> CreateUnique(int count)
    {
        var secrets = new List<GenericSecret>();

        var max = Mathf.Min(count, _secretDescriptions.Count);
        while (secrets.Count < max)
        {
            var newSecret = new GenericSecret();
            if (secrets.Any(x => x.Description == newSecret.Description))
                continue;

            secrets.Add(newSecret);
        }

        return secrets;
    }

    public override Secret Copy() => new GenericSecret(_level, _description);

    public override SecretLevel Level => _level;

    public override SecretIconIdentifier Identifier => SecretIconIdentifier.Generic;

    public override string Description => _description;
}