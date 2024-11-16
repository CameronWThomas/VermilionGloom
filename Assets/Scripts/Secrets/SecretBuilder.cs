using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;

public static class SecretBuilder
{
    private static float MurderChance => CharacterSecretKnowledgeBB.Instance._initialMurderChance;
    private static float NameChance => CharacterSecretKnowledgeBB.Instance._randomNameChance;

    public static SecretBuilderHelper Initialize(this CharacterInfo secretOwner) => new(secretOwner);

    public static List<Secret> BuildSecretList(this SecretBuilderHelper secretBuilderHelper) => secretBuilderHelper.Secrets.ToList();

    public static SecretBuilderHelper TryCreateNameSecrets(this SecretBuilderHelper helper)
    {
        var nameSecret = helper.CharacterType switch
        {
            CharacterType.VanHelsing => new NameSecret("Van Helsing", SecretLevel.Confidential, helper.ID),
            _ => RandomChance(MurderChance)
                ? new NameSecret(Guid.NewGuid().ToString(), RandomSecretLevel(MurderChance, SecretLevel.Private, SecretLevel.Public), helper.ID)
                : null
        };

        if (nameSecret != null)
            helper.Secrets.Add(nameSecret);

        return helper;
    }


    public static SecretBuilderHelper TryCreateVampreSecrets(this SecretBuilderHelper helper)
    {
        if (helper.CharacterType is CharacterType.VanHelsing)
            helper.Secrets.Add(new VampireSecret(helper.ID));

        return helper;
    }

    public static SecretBuilderHelper TryCreateMurderSecrets(this SecretBuilderHelper helper)
    {
        if (!RandomChance(MurderChance))
            return helper;

        var level = RandomSecretLevel(MurderChance, SecretLevel.Confidential, SecretLevel.Public);
        helper.Secrets.Add(new MurderSecret(level, helper.ID));

        return helper;
    }    

    public static SecretBuilderHelper TryCreateRoomSecrets(this SecretBuilderHelper helper)
    {
        if (helper.CharacterType is CharacterType.Owner)
            helper.Secrets.Add(new RoomSecret("Back left room of the foyer", SecretLevel.Public, helper.ID));

        return helper;
    }

    public static SecretBuilderHelper CreateGenericSecrets(this SecretBuilderHelper helper, int count)
    {
        helper.Secrets.AddRange(GenericSecret.CreateUnique(count, helper.ID));
        return helper;
    }

    private static bool RandomChance(float chance)
    {
        var value = UnityEngine.Random.Range(0f, 1f);
        return value <= chance;
    }

    private static SecretLevel RandomSecretLevel(float chance, SecretLevel option1, SecretLevel option2)
        => RandomChance(chance) ? option1 : option2;

    public class SecretBuilderHelper
    {
        public SecretBuilderHelper(CharacterInfo secretOwner)
        {
            SecretOwner = secretOwner;
        }

        public List<Secret> Secrets { get; } = new();
        public CharacterInfo SecretOwner { get; }

        public CharacterID ID => SecretOwner.ID;
        public CharacterType CharacterType => SecretOwner.CharacterType;
    }
}