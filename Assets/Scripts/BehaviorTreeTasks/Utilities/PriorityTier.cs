public enum PriorityTier
{
    Tier1,
    Max
}

public static class PriorityTierHelper
{
    public static float Priority(this PriorityTier priority)
    {
        return priority switch
        {
            PriorityTier.Max => 10f,
            PriorityTier.Tier1 => 1f,
            _ => 0f
        };
    }
}