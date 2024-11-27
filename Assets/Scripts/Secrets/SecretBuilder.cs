using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;

public static class SecretBuilder
{
    private static float MurderChance => CharacterSecretKnowledgeBB.Instance._initialMurderChance;
    private static float NameChance => CharacterSecretKnowledgeBB.Instance._randomNameChance;

    public static SecretBuilderHelper Initialize(this NPCHumanCharacterInfo secretOwner) => new(secretOwner);

    public static List<Secret> BuildSecretList(this SecretBuilderHelper secretBuilderHelper) => secretBuilderHelper.Secrets.ToList();

    public static SecretBuilderHelper TryCreateNameSecrets(this SecretBuilderHelper helper)
    {
        NameSecret nameSecret = null;
        if (helper.CharacterType is CharacterType.VanHelsing)
        {
            nameSecret = new NameSecret.Builder(helper.ID, SecretLevel.Confidential)
                .SetName("Van Helsing")
                .Build();
        }
        else if (RandomChance(MurderChance))
        {
            var level = RandomSecretLevel(MurderChance, SecretLevel.Private, SecretLevel.Public);
            nameSecret = new NameSecret.Builder(helper.ID, level)
                .SetName(NameHelper.GetRandomName())
                .Build();
        }

        if (nameSecret != null)
            helper.Secrets.Add(nameSecret);

        return helper;
    }


    public static SecretBuilderHelper TryCreateVampreSecrets(this SecretBuilderHelper helper)
    {
        if (helper.CharacterType is CharacterType.VanHelsing)
            helper.Secrets.Add(new VampireSecret.Builder(helper.ID).Build());

        return helper;
    }

    public static SecretBuilderHelper TryCreateMurderSecrets(this SecretBuilderHelper helper)
    {
        if (!RandomChance(MurderChance))
            return helper;

        var level = RandomSecretLevel(MurderChance, SecretLevel.Confidential, SecretLevel.Public);

        helper.Secrets.Add(new MurderSecret.Builder(helper.ID, level)
            .SetMurderer(helper.ID)
            .WasSuccessfulMuder()
            .Build());

        return helper;
    }    

    public static SecretBuilderHelper TryCreateRoomSecrets(this SecretBuilderHelper helper)
    {
        if (helper.CharacterType is CharacterType.Owner)
            helper.Secrets.Add(new RoomSecret.Builder(helper.ID, SecretLevel.Public)
                .SetRoomId(RoomID.Den) //TODO will need to look somewhere for this info
                .Build());

        return helper;
    }

    public static SecretBuilderHelper CreateGenericSecrets(this SecretBuilderHelper helper, int count)
    {
        helper.Secrets.AddRange(new GenericSecret.Builder(helper.ID)
            .BuildManyUnique(count));

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
        public SecretBuilderHelper(NPCHumanCharacterInfo secretOwner)
        {
            SecretOwner = secretOwner;
        }

        public List<Secret> Secrets { get; } = new();
        public CharacterInfo SecretOwner { get; }

        public CharacterID ID => SecretOwner.ID;
        public CharacterType CharacterType => SecretOwner.CharacterType;
    }
}