using System.Collections.Generic;
using Unity.Mathematics;

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
        var random = new Random((uint)System.DateTime.UtcNow.Ticks);

        _level = random.NextBool() ? SecretLevel.Public : SecretLevel.Private;
        
        var secretCount = _secretDescriptions.Count;
        _description = _secretDescriptions[random.NextInt(0, secretCount - 1)];

    }

    public override SecretLevel Level => _level;

    public override SecretIconIdentifier Identifier => SecretIconIdentifier.Generic;

    public override string Description => _description;
}