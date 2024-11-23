using BehaviorDesigner.Runtime;

public enum PriorityTier
{
    Tier1,
    Tier2,
    Tier3,
    Tier4,
    Max
}

public class SharedPriorityTierVariable : SharedVariable<PriorityTier>
{
    public static implicit operator SharedPriorityTierVariable(PriorityTier value) { return new SharedPriorityTierVariable { mValue = value }; }
}

public static class PriorityTierHelper
{
    public static float Priority(this PriorityTier priority)
    {
        return priority switch
        {
            PriorityTier.Max => 10f,
            PriorityTier.Tier4 => 4f,
            PriorityTier.Tier3 => 3f,
            PriorityTier.Tier2 => 2f,
            PriorityTier.Tier1 => 1f,
            _ => 0f
        };
    }
}