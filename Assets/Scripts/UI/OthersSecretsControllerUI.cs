using UnityEngine;

public class OthersSecretsControllerUI : MonoBehaviour, IRevealableUI
{
    [SerializeField] private GameObject _revealBlocker;

    private SecretCollection _othersSecrets;

    public void Reveal()
    {
        // TODO tell somethign to reveal

        _othersSecrets.RevealSecret();

        _revealBlocker.SetActive(false);
    }

    public void Initialize(SecretCollection othersSecrets)
    {
        _othersSecrets = othersSecrets;

        _revealBlocker.SetActive(!_othersSecrets.IsAnySecretsRevealed);
    }
}
