public class BodyDeliverySequence : SequenceBase
{
    protected override SequenceRunner GetSequenceRunner()
    {
        return new SequenceRunner()
            .AddWait(5f);
    }

    protected override bool SequencePlayingCondition()
    {
        var triggerVolume = GetComponent<TriggerVolume>();

        if (!triggerVolume.IsPlayerPresent || !triggerVolume.IsNpcBodyPresent)
            return false;

        return true;
    }
}