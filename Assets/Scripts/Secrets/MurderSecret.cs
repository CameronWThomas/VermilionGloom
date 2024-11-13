using System;

public class MurderSecret : Secret
{
    private SecretLevel _level;
    private CharacterInfo _murderer = null;
    private CharacterInfo _victim = null;

    //Need to also communicate some identifier of a person who commited and is a victim of the murderer.
    private MurderSecret(SecretLevel level, CharacterInfo murderer, CharacterInfo victim)
    {
        _level = level;
        _murderer = murderer;
        _victim = victim;
    }

    /// <summary>
    /// For when the character themself murdered someone. If <paramref name="victim"/> is null, the victim will be treated like it is unknown or irrelevant.
    /// </summary>
    public static MurderSecret PersonalMurder(SecretLevel level, CharacterInfo victim = null)
    {
        return new MurderSecret(level, null, victim);
    }

    /// <summary>
    /// For knowing someone else murdered someone. If <paramref name="victim"/> is null, the victim will be treated like it is unknown or irrelevant.
    /// </summary>
    public static MurderSecret OtherMurder(SecretLevel level, CharacterInfo murderer, CharacterInfo victim = null)
    {
        return new MurderSecret(level, murderer, victim);
    }

    public override SecretLevel Level => _level;

    public override SecretIconIdentifier Identifier => SecretIconIdentifier.Murder;

    public override string Description
        => $"{GetMurdererName()} killed {GetVictimName()}";

    public bool UnknownVictim => _victim == null;

    private string GetMurdererName() => _murderer == null ? "I" : _murderer.Name;

    private string GetVictimName()
    {
        if (UnknownVictim)
            return "someone";
        
        return _victim.Name;
    }
}