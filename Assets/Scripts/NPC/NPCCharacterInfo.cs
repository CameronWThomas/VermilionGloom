using UnityEngine;

[RequireComponent(typeof(KnowledgeBase))]
public class NPCCharacterInfo : MonoBehaviour
{
    [SerializeField] string _name = "Undefined";
    
    public string Name => _name;
    public KnowledgeBase KnowledgeBase => GetComponent<KnowledgeBase>();
}
