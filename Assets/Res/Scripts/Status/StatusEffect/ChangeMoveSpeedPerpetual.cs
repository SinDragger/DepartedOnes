
public class ChangeMoveSpeedPerpetual : StatusEffectTerm
{
    public float changePercent;

    public override void Execution(AggregationEntity target)
    {
        SoldierStatus soldier = target as SoldierStatus;
        soldier.ChangeNowSpeed(changePercent);
    }

    public override void ReverseExecution(AggregationEntity target)
    {
        
    }
}
