using System.Net.Sockets;
using UnityEditor.SceneManagement;

public class VampireSecret : Secret
{
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="currentCharacter">Should always be the new secret owner rather than the original</param>
    public VampireSecret(CharacterID currentCharacter)
        : base(SecretLevel.Vampiric, currentCharacter)
    { }

    private VampireSecret(VampireSecret secret) : base(secret) { }

    public override SecretIconIdentifier Identifier => SecretIconIdentifier.VampireKnowledge;

    public override bool NoCharactersInvolved => true;

    public override Secret Copy() => new VampireSecret(this);

    public override string CreateDescription() => $"{SecretOwner.Name} knows there is a vampire in the manor";

}