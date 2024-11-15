using UnityEngine;

public class UICharacterInteraction : MonoBehaviour
{
    public void Open(NpcBrain brain)
    {
        GetComponentInChildren<UICharacterInteractionMenuManager>(true).Initialize(brain);

        gameObject.SetActive(true);
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }
}