using NUnit.Framework;
using System.Collections.Generic;

public class CharacterReputation
{
    private CharacterInfo _characterInfo;
    private List<Secret> _secrets = new();

    public CharacterReputation(CharacterInfo characterInfo, List<Secret> secrets)
    {
        _characterInfo = characterInfo;
        _secrets = secrets;
    }
}