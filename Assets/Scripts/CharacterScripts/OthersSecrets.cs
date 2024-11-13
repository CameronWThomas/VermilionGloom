using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

public class OthersSecrets
{
    private CharacterInfo _characterInfo;
    private List<Secret> _secrets = new();

    public OthersSecrets(CharacterInfo characterInfo, List<Secret> secrets)
    {
        _characterInfo = characterInfo;
        _secrets = secrets;
    }

    public IReadOnlyList<Secret> Secrets => _secrets;
    public bool IsAnySecretsRevealed => _secrets.Any(x => x.IsRevealed);

    public void RevealSecret()
    {
        if (!IsAnySecretsRevealed)
        {
            RevealSecretInternal(true);
            RevealSecretInternal(true);
            RevealSecretInternal(true);
        }
        else
        {
            RevealSecretInternal();
        }
    }

    private void RevealSecretInternal(bool mustSucceed = false)
    {
        var unrevealedSecrets = _secrets.Where(x => !x.IsRevealed).ToList();

        if (!unrevealedSecrets.Any())
            return;

        // Find a random secret to try and reveal
        var random = new Random();
        var secret = unrevealedSecrets[random.Next(0, unrevealedSecrets.Count)];

        if (mustSucceed)
            secret.ForceRevealSecret();
        else
        {
            // todo
        }
    }
}