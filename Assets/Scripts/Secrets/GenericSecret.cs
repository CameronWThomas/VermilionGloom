using System.Collections.Generic;
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

    private readonly int _secretDescriptionIndex = 0;

    private GenericSecret(int secretDescriptionIndex, SecretLevel level, CharacterID secretOwner)
        : base(level, secretOwner)
    {
        _secretDescriptionIndex = secretDescriptionIndex;
    }

    private GenericSecret(GenericSecret secret) : base(secret)
    {
        _secretDescriptionIndex = secret._secretDescriptionIndex;
    }

    public static List<GenericSecret> CreateUnique(int count, CharacterID secretOwner)
    {
        var secrets = new List<GenericSecret>();

        var usedIndexes = new List<int>();
        var max = Mathf.Min(count, _secretDescriptions.Count);
        while (secrets.Count < max)
        {
            var secretIndex = UnityEngine.Random.Range(0, _secretDescriptions.Count);
            if (usedIndexes.Contains(secretIndex))
                continue;
            usedIndexes.Add(secretIndex);

            var level = UnityEngine.Random.Range(0f, 1f) >= .5f ? SecretLevel.Public : SecretLevel.Private;

            secrets.Add(new GenericSecret(secretIndex, level, secretOwner));
        }

        return secrets;
    }

    public override SecretIconIdentifier Identifier => SecretIconIdentifier.Generic;

    public override Secret Copy() => new GenericSecret(this);

    public override string CreateDescription() => _secretDescriptions[_secretDescriptionIndex];
}