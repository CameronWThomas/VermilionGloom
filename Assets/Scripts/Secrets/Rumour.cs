using UnityEngine;

public class Rumour
{
    public SecretCollection Secrets { get; private set; }
    public CharacterInfo RumourTarget { get; private set; }

    public Rumour(SecretCollection secrets, CharacterInfo rumourTarget)
    {
        Secrets = secrets;
        RumourTarget = rumourTarget;
    }
}