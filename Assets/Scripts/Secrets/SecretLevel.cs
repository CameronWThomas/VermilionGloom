using System.Collections.Generic;
using Unity.VisualScripting;

public enum SecretLevel
{
    /// <summary>
    /// Always succeed in looking for public secrets. Other levels are chance based or depend on v-bucks
    /// </summary>
    Public,

    Private,
    Confidential,
    Vampiric
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
}