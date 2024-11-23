using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(NPCHumanCharacterInfo))]
public class CharacterSecretKnowledge : MonoBehaviour
{
    private List<Secret> _secrets = new();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        CharacterSecretKnowledgeBB.Instance.Register(ID, this);
    }

    private void Update()
    {
        
    }

    public NPCHumanCharacterID ID => GetComponent<NPCHumanCharacterInfo>().NPCHumanCharacterID;
    public IReadOnlyList<Secret> Secrets => _secrets.ToList();

    public void AddSecrets(IEnumerable<Secret> secrets) => _secrets.AddRange(secrets);
    public void AddSecret(Secret secret) => _secrets.Add(secret);

    public bool TryGetMurderSecret(CharacterID murderer, CharacterID victim, out MurderSecret murderSecret)
    {
        murderSecret = Secrets.OfType<MurderSecret>().FirstOrDefault(x => x.SecretTarget == murderer && x.AdditionalCharacter == victim);
        return murderSecret != null;
    }

    public List<Secret> GetSecrets(CharacterID target, CharacterID additionalCharacter)
    {
        return Secrets.Where(x => x.HasAdditionalCharacter && x.HasSecretTarget)
            .Where(x => x.SecretTarget == target && x.AdditionalCharacter == additionalCharacter)
            .ToList();
    }
}
