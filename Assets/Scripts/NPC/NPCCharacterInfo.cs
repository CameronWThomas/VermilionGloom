using UnityEngine;

[RequireComponent(typeof(KnowledgeBase))]
public class NPCCharacterInfo : MonoBehaviour
{
    [SerializeField] string _name = "Undefined";
    [SerializeField] Color _npcColor;

    
    public string Name => _name;
    public Color NPCColor => _npcColor;
    public KnowledgeBase KnowledgeBase => GetComponent<KnowledgeBase>();
}
