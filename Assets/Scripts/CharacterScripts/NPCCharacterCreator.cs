using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class NPCCharacterCreator : MonoBehaviour
{
    [SerializeField] private GameObject _humanCharacterPrefab = null;

    public void Start()
    {
        UnityEngine.Random.InitState((int)DateTime.UtcNow.Ticks);
    }

    public List<CharacterInfo> CreateCharacters(int npcHumanCount)
    {
        return new CharacterCreatorTool()
            .CreateGameObjects(npcHumanCount, _humanCharacterPrefab)
            .InitializeCharacterInfo(1)
            .CreateAndInitializeSecrets(3)
            .SpreadSecrets(3)
            .CreateUniqueModels()
            .InitializeRelationships()
            .PlaceCharacters()
            .RegisterCharacters()
            .Build();
    }

    private class CharacterCreatorTool
    {
        private Dictionary<CharacterID, GameObject> _characterInstanceDict = new();

        public CharacterCreatorTool CreateGameObjects(int npcCount, GameObject npcHumanCharacterPrefab)
        {
            for (var i = 0; i < npcCount; i++)
            {
                var instance = Instantiate(npcHumanCharacterPrefab);
                var characterID = instance.GetComponent<CharacterInfo>().ID;
                _characterInstanceDict.Add(characterID, instance);
            }
            return this;
        }

        public CharacterCreatorTool InitializeCharacterInfo(int vanHelsingCount)
        {
            var ownerSet = false;
            foreach (var instance in _characterInstanceDict.Values.Randomize())
            {
                if (instance.TryGetComponent<NPCHumanCharacterInfo>(out var npcHuman))
                {
                    var characterType = CharacterType.Generic;
                    if (!ownerSet)
                    {
                        characterType = CharacterType.Owner;
                        ownerSet = true;
                    }
                    else if (vanHelsingCount-- > 0)
                        characterType = CharacterType.VanHelsing;

                    npcHuman.Initialize(characterType);
                }
            }

            return this;
        }

        public CharacterCreatorTool CreateAndInitializeSecrets(int genericSecretCount)
        {
            foreach (var characterInfo in GetCharacterComponent<NPCHumanCharacterInfo>())
            {
                var secrets = characterInfo
                    .Initialize()
                    .TryCreateVampreSecrets()
                    .TryCreateMurderSecrets()
                    .TryCreateRoomSecrets()
                    .CreateGenericSecrets(3)
                    .BuildSecretList();

                var secretKnowledge = characterInfo.gameObject.AddComponent<CharacterSecretKnowledge>();
                secretKnowledge.AddSecrets(secrets);
            }

            return this;
        }

        public CharacterCreatorTool SpreadSecrets(int otherCharactersSecretsPerCharacter)
        {
            foreach (var (characterId, instance) in _characterInstanceDict)
            {
                var secretKnowledge = instance.GetComponent<CharacterSecretKnowledge>();
                var secrets = GetOtherCharactersSecrets(characterId, otherCharactersSecretsPerCharacter);
                secretKnowledge.AddSecrets(secrets);
            }

            return this;
        }

        public CharacterCreatorTool CreateUniqueModels()
        {
            foreach (var characterCustomizer in GetCharacterComponent<CharacterCustomizer>())
                characterCustomizer.DressRandomly();

            return this;
        }

        public CharacterCreatorTool InitializeRelationships()
        {
            //TODO
            return this;
        }

        public CharacterCreatorTool PlaceCharacters()
        {
            var allRooms = FindObjectsByType<Room>(FindObjectsSortMode.None);

            foreach (var characterTransform in GetCharacterComponent<Transform>())
            {
                var randomRoom = allRooms.Randomize().First();
                var randomPoint = randomRoom.GetRandomPointInRoom();
                var agent = characterTransform.GetComponent<NavMeshAgent>();
                agent.Warp(randomPoint);
            }

            return this;
        }

        public CharacterCreatorTool RegisterCharacters()
        {
            //TODO
            return this;
        }

        public List<CharacterInfo> Build() => GetCharacterComponent<CharacterInfo>();

        private List<GameObject> GetCharacterInstances<TCharacterID>() where TCharacterID : CharacterID
            => _characterInstanceDict
            .Where(x => x.Key is TCharacterID)
            .Select(x => x.Value)
            .ToList();

        private List<TComponent> GetCharacterComponent<TComponent>() where TComponent : UnityEngine.Object
            => _characterInstanceDict
            .Select(x => x.Value.GetComponent<TComponent>())
            .Where(x => x != null)
            .ToList();

        private List<TComponent> GetCharacterComponent<TCharacterID, TComponent>() where TCharacterID : CharacterID where TComponent : MonoBehaviour
            => GetCharacterInstances<TCharacterID>()
            .Select(X => X.GetComponent<TComponent>())
            .ToList();

        private IEnumerable<Secret> GetOtherCharactersSecrets(CharacterID characterId, int otherCharactersSecretsPerCharacter)
        {
            // Select only others secrets that don't have us as the owner and randomize the order
            var secretKnowledges = _characterInstanceDict
                .Where(x => x.Key != characterId)
                .Select(x => x.Value.GetComponent<CharacterSecretKnowledge>())
                .SelectMany(x => x.Secrets)
                .Where(x => x.SecretOwner != characterId)
                .Randomize()
                .ToList();

            var returnCount = 0;
            var returnMax = Mathf.Min(otherCharactersSecretsPerCharacter, secretKnowledges.Count);

            // Keep looping through the secrets selecting by chance
            while (true)
                foreach (var secret in secretKnowledges)
                {
                    if (secret.Level.RandomChance())
                    {
                        var secretCopy = secret.Copy();
                        yield return secretCopy;
                        returnCount++;
                    }

                    if (returnCount >= returnMax)
                        yield break;
                }
        }        
    }
}