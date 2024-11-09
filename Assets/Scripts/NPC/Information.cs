public abstract class Information
{
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

    public RelationshipInformation(string subjectCharacterName, string targetCharacterName, RelationshipType relationshipType)
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
    public FactualInformation() { }

    public override string ToString()
    {
        return "This is a fact";
    }
}
