using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(CharacterInfo))]
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

    public CharacterID ID => GetComponent<CharacterInfo>().ID;
    public IReadOnlyList<Secret> Secrets => _secrets.ToList();

    public void AddSecrets(IEnumerable<Secret> secrets) => _secrets.AddRange(secrets);
    public void AddSecret(Secret secret) => _secrets.Add(secret);
}
