using UnityEngine;

[RequireComponent(typeof(KnowledgeBase))]
public class NPCCharacterInfo : MonoBehaviour
{
    [SerializeField] string _name = "Undefined";
    [SerializeField] Color _npcColor;
    [SerializeField] PrivacyLevel _unlockedPrivacyLevel = PrivacyLevel.Public;

    
    public string Name => _name;
    public Color NPCColor => _npcColor;
    public PrivacyLevel UnlockedPrivacyLevel => _unlockedPrivacyLevel;
    public KnowledgeBase KnowledgeBase => GetComponent<KnowledgeBase>();
}
