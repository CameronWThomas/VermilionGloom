public enum PrivacyLevel
{
    Public = 0,
    Private = 1,
    Secret = 2,
}

public abstract class Information
{
    protected PrivacyLevel _privacyLevel;
    protected Information(PrivacyLevel privacyLevel)
    {
        _privacyLevel = privacyLevel;
    }

    public PrivacyLevel PrivacyLevel => _privacyLevel;

    public override abstract string ToString();
}

public class RelationshipInformation : Information
{
    public enum RelationshipType
    {
        Like, Hate, Killed, Aware
    }

    private string _subjectCharacterName;
    private string _targetCharacterName;
    private RelationshipType _relationshipType;

    public RelationshipInformation(string subjectCharacterName,
        string targetCharacterName,
        RelationshipType relationshipType,
        PrivacyLevel privacyLevel)
        : base(privacyLevel)
    {
        _subjectCharacterName = subjectCharacterName;
        _targetCharacterName = targetCharacterName;
        _relationshipType = relationshipType;
    }

    public override string ToString()
    {
        return $"{_subjectCharacterName} {_relationshipType} {_targetCharacterName}";
    }
}

public class FactualInformation : Information
{
    public FactualInformation(PrivacyLevel privacyLevel)
        : base(privacyLevel)
    { }

    public override string ToString()
    {
        return "This is a fact";
    }
}
