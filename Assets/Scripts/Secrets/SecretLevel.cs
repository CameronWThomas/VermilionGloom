using System.Collections.Generic;
using Unity.VisualScripting;

public enum SecretLevel
{
    /// <summary>
    /// Always succeed in looking for public secrets. Other levels are chance based or depend on v-bucks
    /// </summary>
    Public = 0,

    Private = 1,
    Confidential = 2,
    Vampiric = 3
}

public static class SecretLevelExtensions
{
    //TODO put in some monobehaviour
    private static Dictionary<SecretLevel, float> _chanceDictionary = new()
    {
        { SecretLevel.Public, 1f },
        { SecretLevel.Private, .5f },
        { SecretLevel.Confidential, 1f },
        { SecretLevel.Vampiric, .01f },
    };


    public static float Chance(this SecretLevel secretLevel)
        => _chanceDictionary[secretLevel];

    public static bool RandomChance(this SecretLevel secretLevel)
    {
        var chance = _chanceDictionary[secretLevel];
        var value = UnityEngine.Random.Range(0f, 1f);
        return value <= chance;
    }
}