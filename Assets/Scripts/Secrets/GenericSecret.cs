using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Secret with nothing of value
/// </summary>
public class GenericSecret : Secret
{   
    private const int SECRET_DESCRIPTION_COUNT = 4;

    private readonly int _secretDescriptionIndex = 0;

    private GenericSecret(int secretDescriptionIndex)
    {
        _secretDescriptionIndex = secretDescriptionIndex;
    }

    private GenericSecret(GenericSecret secret) : base(secret)
    {
        _secretDescriptionIndex = secret._secretDescriptionIndex;
    }

    protected override Secret Copy() => new GenericSecret(this);

    public override SecretIconIdentifier Identifier => SecretIconIdentifier.Generic;

    protected override string CreateDescription()
    {
        return _secretDescriptionIndex switch
        {
            0 => $"{SecretTarget.Name} never learned how to read",
            1 => $"{SecretTarget.Name} likes to eat dirt",
            2 => $"{SecretTarget.Name} regularly forgets their name",
            3 => $"{SecretTarget.Name} is only here in the hopes of being murdered",
            _ => "Uh oh... You shouldn't be seeing this..."
        };
    }

    public class Builder : SecretTypeBuilder<GenericSecret>
    {
        public Builder(CharacterID secretOwner) : base(secretOwner, null) { }

        public override GenericSecret Build() => BuildManyUnique(1).First();

        public List<GenericSecret> BuildManyUnique(int count)
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

                _secretLevel = UnityEngine.Random.Range(0f, 1f) >= .5f ? SecretLevel.Public : SecretLevel.Private;

                var genericSecret = new GenericSecret(secretIndex);
                Init(genericSecret, target: _secretOwner);

                secrets.Add(genericSecret);
            }

            return secrets;
        }
    }
}