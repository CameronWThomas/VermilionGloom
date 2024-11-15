using UnityEngine;

public class RevealButtonControllerUI : MonoBehaviour
{
    public void Reveal()
    {
        var possibleRevealable = transform.parent;
        do
        {
            if (possibleRevealable.TryGetComponent<IRevealableUI>(out var revealableUI))
            {
                revealableUI.Reveal();
                break;
            }

            possibleRevealable = possibleRevealable.parent;

        } while (possibleRevealable != null);
    }
}
