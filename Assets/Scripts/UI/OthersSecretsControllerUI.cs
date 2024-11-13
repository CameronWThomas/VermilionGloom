using UnityEngine;

public class OthersSecretsControllerUI : MonoBehaviour, IRevealableUI
{
    [SerializeField] private GameObject _revealBlocker;

    private OthersSecrets _othersSecrets;

    public void Reveal()
    {
        // TODO tell somethign to reveal

        _othersSecrets.RevealSecret();

        _revealBlocker.SetActive(false);
    }

    public void Initialize(OthersSecrets othersSecrets)
    {
        _othersSecrets = othersSecrets;

        _revealBlocker.SetActive(!_othersSecrets.IsAnySecretsRevealed);
    }
}
