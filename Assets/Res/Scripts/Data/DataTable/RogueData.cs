public class RogueData : AggregationEntity
{
    public string scenarioId;
    /// <summary>
    /// 用于在rogue中战斗的军队
    /// </summary>
    public string rogueLegionId;
    /// <summary>
    /// 完成rogue后奖励的军队
    /// </summary>
    public string rogueRewardLegionId;

    public string firstBattleScenceId;

    public RogueNodeData[] rogueNodeDatas;
}
