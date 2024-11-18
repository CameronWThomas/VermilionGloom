using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Secret with nothing of value
/// </summary>
public class GenericSecret : Secret
{   
    private const int SECRET_DESCRIPTION_COUNT = 4;

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
        var max = Mathf.Min(count, SECRET_DESCRIPTION_COUNT);
        while (secrets.Count < max)
        {
            var secretIndex = UnityEngine.Random.Range(0, SECRET_DESCRIPTION_COUNT);
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

    public override string CreateDescription()
    {
        return _secretDescriptionIndex switch
        {
            0 => $"{SecretOwner.Name} never learned how to read",
            1 => $"{SecretOwner.Name} likes to eat dirt",
            2 => $"{SecretOwner.Name} regularly forgets their name",
            3 => $"{SecretOwner.Name} is only here in the hopes of being murdered",
            _ => "Uh oh... You shouldn't be seeing this..."
        };
    }
}