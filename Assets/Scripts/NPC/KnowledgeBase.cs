using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(NPCCharacterInfo))]
public class KnowledgeBase : MonoBehaviour
{
    public List<Information> Knowledge { get; private set; } = new();
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Knowledge.Clear();

        Knowledge.Add(new RelationshipInformation("I", "Mark", RelationshipInformation.RelationshipType.Hate));
        Knowledge.Add(new RelationshipInformation("Billy", "Me", RelationshipInformation.RelationshipType.Like));
        Knowledge.Add(new RelationshipInformation("I", "Clair", RelationshipInformation.RelationshipType.Killed));
    }
}
